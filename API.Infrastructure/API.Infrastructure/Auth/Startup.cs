using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Core.Interface;
using API.Infrastructure.Auth.Jwt;
namespace API.Infrastructure.Auth
{
    internal static class  Startup
    {
        internal static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration config)
        {
            services
           .AddCurrentUser();
           //.AddPermissions()

            //.AddIdentity();
            services.Configure<SecuritySettings>(config.GetSection(nameof(SecuritySettings)));
            return  services.AddJwtAuth(config);

        }
        internal static IApplicationBuilder UseCurrentUser(this IApplicationBuilder app) =>
        app.UseMiddleware<CurrentUserMiddleware>();

        //internal static IApplicationBuilder UseCustomAuth(this IApplicationBuilder app) =>
        //app.UseMiddleware<TokenAuthMiddleware>();
        private static IServiceCollection AddCurrentUser(this IServiceCollection services) =>
            services
                .AddScoped<CurrentUserMiddleware>()
                .AddScoped<ICurrentUser, CurrentUser>()
                .AddScoped(sp => (ICurrentUserInitializer)sp.GetRequiredService<ICurrentUser>());

        //private static IServiceCollection AddPermissions(this IServiceCollection services) =>
        //    services
        //        .AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
        //        .AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
    }
}
