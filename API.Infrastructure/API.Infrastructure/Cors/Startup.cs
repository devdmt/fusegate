using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;


namespace API.Infrastructure.Cors;

internal static class Startup
{
    private const string CorsPolicy = nameof(CorsPolicy);

    internal static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration config)
    {
        var corsSettings = config.GetSection(nameof(CorsSettings)).Get<CorsSettings>();
        var origins = new List<string>();
        if (corsSettings.Angular is not null)
            origins.AddRange(corsSettings.Angular.Split(';', StringSplitOptions.RemoveEmptyEntries));
        if (corsSettings.Blazor is not null)
            origins.AddRange(corsSettings.Blazor.Split(';', StringSplitOptions.RemoveEmptyEntries));
        if (corsSettings.App is not null)
            origins.AddRange(corsSettings.App.Split(';', StringSplitOptions.RemoveEmptyEntries));
        if (corsSettings.React is not null)
            origins.AddRange(corsSettings.React.Split(';', StringSplitOptions.RemoveEmptyEntries));

        return services.AddCors(opt =>
            opt.AddPolicy(CorsPolicy, policy =>
            {
                policy.AllowAnyHeader()
                    .AllowAnyMethod();

                if (origins.Count > 0)
                {
                    policy.WithOrigins(origins.ToArray())
                        .AllowCredentials();
                }
                else
                {
                    policy.AllowAnyOrigin();
                }

                policy.SetPreflightMaxAge(TimeSpan.FromHours(1));
            }));
    }

    internal static IApplicationBuilder UseCorsPolicy(this IApplicationBuilder app) =>
        app.UseCors(CorsPolicy);
}