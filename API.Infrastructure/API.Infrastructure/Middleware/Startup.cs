using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Infrastructure.Middleware;

internal static class Startup
{
    internal static IServiceCollection AddExceptionMiddleware(this IServiceCollection services) =>
        services.AddScoped<ExceptionMiddleware>();

    internal static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder app) =>
        app.UseMiddleware<ExceptionMiddleware>();

    internal static IServiceCollection AddRequestLogging(this IServiceCollection services, IConfiguration config)
    {
        var settings = GetMiddlewareSettings(config);
        if (settings.EnableHttpsLogging)
        {
            services.AddSingleton<RequestLoggingMiddleware>();
            services.AddScoped<ResponseLoggingMiddleware>();
        }

        if (settings.EnableApiRequestAudit)
        {
            services.AddScoped<ApiRequestAuditMiddleware>();
        }

        return services;
    }

    internal static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app, IConfiguration config)
    {
        var settings = GetMiddlewareSettings(config);
        if (settings.EnableHttpsLogging)
        {
            app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseMiddleware<ResponseLoggingMiddleware>();
        }

        if (settings.EnableApiRequestAudit)
        {
            app.UseMiddleware<ApiRequestAuditMiddleware>();
        }

        return app;
    }

    private static MiddlewareSettings GetMiddlewareSettings(IConfiguration config) =>
        config.GetSection(nameof(MiddlewareSettings)).Get<MiddlewareSettings>() ?? new MiddlewareSettings();
}