using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Infrastructure.BackgroundServices;

internal static class Startup
{
    internal static IServiceCollection AddBackgroundServices(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<SmsProcessingOptions>(config.GetSection(SmsProcessingOptions.SectionName));
        services.AddHostedService<SmsProcessingBackgroundService>();
        return services;
    }
}
