using System.Text;
using API.Infrastructure.Interface;
using API.Infrastructure.OpenApi;
using DAL;
using DAL.Core.Interface;
using DAL.ModelView;
using Microsoft.AspNetCore.Http;

namespace API.Infrastructure.Middleware;

public class ApiRequestAuditMiddleware : IMiddleware
{
    private const int MaxBodyLength = 64 * 1024;
    private static readonly PathString ApiPrefix = new("/api");

    private readonly Isettings _settings;
    private readonly ICurrentUser _currentUser;
    private readonly ApplicationDbContext _db;

    public ApiRequestAuditMiddleware(Isettings settings, ICurrentUser currentUser, ApplicationDbContext db)
    {
        _settings = settings;
        _currentUser = currentUser;
        _db = db;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!ShouldAudit(context.Request))
        {
            await next(context);
            return;
        }

        var payload = await ReadRequestBodyAsync(context.Request);
        var originalBody = context.Response.Body;
        await using var responseBuffer = new MemoryStream();
        context.Response.Body = responseBuffer;

        try
        {
            await next(context);
        }
        finally
        {
            string responseBody;
            try
            {
                responseBuffer.Seek(0, SeekOrigin.Begin);
                responseBody = await new StreamReader(responseBuffer, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true)
                    .ReadToEndAsync();
                responseBuffer.Seek(0, SeekOrigin.Begin);
                await responseBuffer.CopyToAsync(originalBody);
            }
            catch
            {
                responseBody = string.Empty;
            }
            finally
            {
                context.Response.Body = originalBody;
            }

            await PersistAuditAsync(context, payload, responseBody);
        }
    }

    private static bool ShouldAudit(HttpRequest request)
    {
        var path = request.Path.Value ?? string.Empty;
        if (!request.Path.StartsWithSegments(ApiPrefix, StringComparison.OrdinalIgnoreCase))
            return false;

        if (path.Contains("/swagger", StringComparison.OrdinalIgnoreCase)
            || path.Contains("/health", StringComparison.OrdinalIgnoreCase)
            || path.Contains("/tokens", StringComparison.OrdinalIgnoreCase)
            || path.Contains("/authenticate", StringComparison.OrdinalIgnoreCase)
            || path.Contains("/auth/", StringComparison.OrdinalIgnoreCase))
            return false;

        return true;
    }

    private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        var path = request.Path.Value ?? string.Empty;
        if (path.Contains("tokens", StringComparison.OrdinalIgnoreCase)
            || path.Contains("authenticate", StringComparison.OrdinalIgnoreCase))
            return "[Redacted] Contains Sensitive Information.";

        if (string.IsNullOrEmpty(request.ContentType)
            || !request.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
            return string.Empty;

        request.EnableBuffering();
        using var reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 4096, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;
        return Truncate(body);
    }

    private async Task PersistAuditAsync(HttpContext context, string payload, string responseBody)
    {
        try
        {
            var partnerCode = ResolvePartnerCode(context);
            string? partnerName = null;
            if (!string.IsNullOrWhiteSpace(partnerCode))
            {
                var partner = await _db.GetPartnerAsync(partnerCode);
                if (partner != null)
                {
                    partnerCode = partner.PartnerCode;
                    partnerName = partner.PartnerName;
                }
            }

            var apiName = $"{context.Request.Method} {context.Request.Path}{context.Request.QueryString}";
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            await _settings.AddRequest(new ApiRequestsDTO
            {
                RequestName = "ApiRequestAudit",
                RequestType = (int)ApiRequestType.Query,
                ApiName = Truncate(apiName, 500),
                PayLoad = payload,
                IP = Truncate(ip, 100),
                PartnerCode = Truncate(partnerCode, 50),
                PartnerName = Truncate(partnerName, 200),
                Response = Truncate(responseBody),
                ResponseCode = context.Response.StatusCode
            });
        }
        catch (Exception ex)
        {
            _settings.LogRequests(ex.Message + "|" + ex.StackTrace, nameof(ApiRequestAuditMiddleware), RequestType.Error);
        }
    }

    private string? ResolvePartnerCode(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(PartnerCodeHeaderAttribute.PartnerCode, out var headerValues))
        {
            var fromHeader = headerValues.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(fromHeader))
                return fromHeader.Trim();
        }

        try
        {
            return _currentUser.PartnerCode()?.Trim();
        }
        catch
        {
            return null;
        }
    }

    private static string Truncate(string? value, int maxLength = MaxBodyLength)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;
        return value.Length <= maxLength ? value : value[..maxLength] + "...[truncated]";
    }
}
