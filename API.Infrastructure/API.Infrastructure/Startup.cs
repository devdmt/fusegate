using API.Infrastructure.Persistence;
using API.Infrastructure.OpenApi;
using API.Infrastructure.Common;
using API.Infrastructure.BackgroundServices;
using Microsoft.AspNetCore.HttpOverrides;
using API.Infrastructure.Middleware;
using API.Infrastructure.Cors;
using API.Infrastructure.Auth;
using Microsoft.AspNetCore.Hosting;
using API.Infrastructure.Localization;
using FCB.Infrastructure.Caching;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using API.Infrastructure.RateLimit;

namespace API.Infrastructure
{
    public static class Startup
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            return services
                .AddApiVersioning()
                .AddAuth(config)
                .AddCorsPolicy(config)
                .AddExceptionMiddleware()
                .AddLocalization(config)
                .AddCredWaveSetting(config)
                .AddEndpointsApiExplorer()
                .AddCaching(config)
                .AddOpenApiDocumentation(config)
                .AddRouting(options => options.LowercaseUrls = true)
                //.AddRateLimit(config)
                .AddPersistence(config)
                .AddAkibaAppPersistence(config)
                .AddMainDBPersistence(config)
                .AddRequestLogging(config)
                .AddServices()
                .AddBackgroundServices(config);
        }
        private static IServiceCollection AddApiVersioning(this IServiceCollection services) =>
   services.AddApiVersioning(config =>
   {
       config.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
       config.AssumeDefaultVersionWhenUnspecified = true;
       config.ReportApiVersions = true;
   });

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder, IWebHostEnvironment env, IConfiguration config)
        {
            //if (!env.IsDevelopment())
            //{
            //    builder.UseHttpsRedirection();
            //}

            builder
                .UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });

            if (!env.IsDevelopment())
            {
                builder.UseHsts();
            }

            builder.UseHttpsRedirection();

            builder
               .UseOpenApiDocumentation(config)
              .UseExceptionMiddleware()
                .UseRouting()
                 .UseCorsPolicy()
               // .UseRateLimit(config)
                .UseAuthentication()
                .UseAuthorization()
                .UseCurrentUser()
                 .UseRequestLogging(config);


            return builder;


        }
     }
}
