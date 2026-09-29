using System.Text;
using DAL;
using DAL.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API.Infrastructure.BackgroundServices;

/// <summary>
/// Polls pending callback responses and posts payloads to the configured callback URL.
/// </summary>
public sealed class CallBackResponseProcessingBackgroundService : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(10);
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<CallBackResponseProcessingBackgroundService> _logger;

    public CallBackResponseProcessingBackgroundService(
        IServiceScopeFactory scopeFactory,
        IHttpClientFactory httpClientFactory,
        ILogger<CallBackResponseProcessingBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Callback response processor started. Poll interval: {Seconds}s", PollInterval.TotalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingCallbacksAsync(stoppingToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error in callback response processor loop.");
            }

            try
            {
                await Task.Delay(PollInterval, stoppingToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }

        _logger.LogInformation("Callback response processor stopped.");
    }

    private async Task ProcessPendingCallbacksAsync(CancellationToken cancellationToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var httpClient = _httpClientFactory.CreateClient(nameof(CallBackResponseProcessingBackgroundService));

        var pending = await db.callBackResponse
            .Where(x => !x.CallbackProcessed && x.ProcessResponse)
            .OrderBy(x => x.CreatedAt)
            .Take(100)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        if (pending.Count == 0)
        {
            return;
        }

        _logger.LogInformation("Processing {Count} callback response record(s).", pending.Count);

        foreach (var item in pending)
        {
            await ProcessSingleAsync(db, httpClient, item, cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task ProcessSingleAsync(
        ApplicationDbContext db,
        HttpClient httpClient,
        CallBackResponse item,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!Uri.TryCreate(item.CallbackUrl, UriKind.Absolute, out var callbackUri))
            {
                item.Status = "Failed";
                item.StatusMessage = "Invalid callback URL.";
                item.Callbackerror = "CallbackUrl is missing or not a valid absolute URL.";
                item.Responded = true;
                item.ProcessResponse = false;
                item.CallbackProcessed = true;
                item.RespondedAT = DateTime.UtcNow;
                await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                return;
            }

            var payload = item.Response ?? string.Empty;
            using var request = new HttpRequestMessage(HttpMethod.Post, callbackUri)
            {
                Content = new StringContent(payload, Encoding.UTF8, "application/json")
            };

            using var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var boundedBody = Truncate(responseBody, 1000);

            item.Status = response.IsSuccessStatusCode ? "Success" : "Failed";
            item.StatusMessage = $"{(int)response.StatusCode} {response.ReasonPhrase}".Trim();
            item.Callbackerror = response.IsSuccessStatusCode ? null : boundedBody;
            item.Responded = true;
            item.ProcessResponse = false;
            item.CallbackProcessed = true;
            item.RespondedAT = DateTime.UtcNow;

            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException e) when (cancellationToken.IsCancellationRequested)
        {
             _logger.LogError(e, "Failed to persist callback processing result for {Id}.", item.Id);
            throw;
        }
        catch (Exception ex)
        {
            item.Status = "Failed";
            item.StatusMessage = ex.Message;
            item.Callbackerror = Truncate(ex.ToString(), 1000);
            item.Responded = true;
            item.ProcessResponse = false;
            item.CallbackProcessed = true;
            item.RespondedAT = DateTime.UtcNow;

            try
            {
                await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception saveEx)
            {
                _logger.LogError(saveEx, "Failed to persist callback processing result for {Id}.", item.Id);
            }

            _logger.LogError(ex, "Failed to process callback response item {Id}.", item.Id);
        }
    }

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
        {
            return value;
        }

        return value[..maxLength];
    }
}
