
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using ILogger = Serilog.ILogger;
using DAL;
//using DAL.Helpers;
using System.Data;
using API.Infrastructure.Common.Contract;
using DAL.Core.Application.Persistence;

namespace API.Infrastructure.Persistence
{
    internal static class Startup
    {
        private static readonly ILogger _logger = Log.ForContext(typeof(Startup));

        internal static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration config)
        {
            services.AddOptions<DatabaseSettings>()
                .BindConfiguration(nameof(DatabaseSettings))
                .PostConfigure(databaseSettings =>
                {
                    _logger.Information("Current DB Provider: {dbProvider}", databaseSettings.DBProvider);
                })
                .ValidateDataAnnotations()
                .ValidateOnStart();
            return services
           .AddDbContext<ApplicationDbContext>((p, m) =>
           {
             var databaseSettings = p.GetRequiredService<IOptions<DatabaseSettings>>().Value;
             databaseSettings.HydrateFromEnvironment();
             //  string converteddata =
            System.Text.ASCIIEncoding.ASCII.GetString(System.Convert.FromBase64String(databaseSettings.Cypher));
               //  string encryptedconstring = EncryptDecrypt.Decrypt(databaseSettings.ConnectionString, converteddata);
               string encryptedconstring = databaseSettings.ESBConnectionstring;

               m.UseSqlServer(encryptedconstring,
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure()
                       .MigrationsAssembly("FuseGate");
                    });
               
           }).AddRepositories();
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // Add Repositories
            services.AddScoped(typeof(IRepository<>), typeof(ApplicationDbRepository<>));

            foreach (var aggregateRootType in
                typeof(IAggregateRoot).Assembly.GetExportedTypes()
                    .Where(t => typeof(IAggregateRoot).IsAssignableFrom(t) && t.IsClass)
                    .ToList())
            {
                // Add ReadRepositories.
                services.AddScoped(typeof(IReadRepository<>).MakeGenericType(aggregateRootType), sp =>
                    sp.GetRequiredService(typeof(IRepository<>).MakeGenericType(aggregateRootType)));

                // Decorate the repositories with EventAddingRepositoryDecorators and expose them as IRepositoryWithEvents.
                //services.AddScoped(typeof(IRepositoryWithEvents<>).MakeGenericType(aggregateRootType), sp =>
                //    Activator.CreateInstance(
                //        typeof(EventAddingRepositoryDecorator<>).MakeGenericType(aggregateRootType),
                //        sp.GetRequiredService(typeof(IRepository<>).MakeGenericType(aggregateRootType)))
                //    ?? throw new InvalidOperationException($"Couldn't create EventAddingRepositoryDecorator for aggregateRootType {aggregateRootType.Name}"));
            }

            return services;
        } 
        
        internal static IServiceCollection AddAkibaAppPersistence(this IServiceCollection services, IConfiguration config)
        {
            services.AddOptions<DatabaseSettings>()
                .BindConfiguration(nameof(DatabaseSettings))
                .PostConfigure(databaseSettings =>
                {
                    _logger.Information("Current DB Provider: {dbProvider}", databaseSettings.DBProvider);
                })
                .ValidateDataAnnotations()
                .ValidateOnStart();
            return services
           .AddDbContext<AkibappDbContext>((p, m) =>
           {
             var databaseSettings = p.GetRequiredService<IOptions<DatabaseSettings>>().Value;
             databaseSettings.HydrateFromEnvironment();
             //  string converteddata =
            System.Text.ASCIIEncoding.ASCII.GetString(System.Convert.FromBase64String(databaseSettings.Cypher));
               //  string encryptedconstring = EncryptDecrypt.Decrypt(databaseSettings.ConnectionString, converteddata);
               string encryptedconstring = databaseSettings.Akibaconnectionstring;

               m.UseSqlServer(encryptedconstring,
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure()
                       .MigrationsAssembly("FuseGate");
                    });
               
           }).AddAppRepositories();
        }

        private static IServiceCollection AddAppRepositories(this IServiceCollection services)
        {
            // Add Repositories
            services.AddScoped(typeof(IRepository<>), typeof(AppDbRepository<>));

            foreach (var aggregateRootType in
                typeof(IAggregateRoot).Assembly.GetExportedTypes()
                    .Where(t => typeof(IAggregateRoot).IsAssignableFrom(t) && t.IsClass)
                    .ToList())
            {
                // Add ReadRepositories.
                services.AddScoped(typeof(IReadRepository<>).MakeGenericType(aggregateRootType), sp =>
                    sp.GetRequiredService(typeof(IRepository<>).MakeGenericType(aggregateRootType)));

                // Decorate the repositories with EventAddingRepositoryDecorators and expose them as IRepositoryWithEvents.
                //services.AddScoped(typeof(IRepositoryWithEvents<>).MakeGenericType(aggregateRootType), sp =>
                //    Activator.CreateInstance(
                //        typeof(EventAddingRepositoryDecorator<>).MakeGenericType(aggregateRootType),
                //        sp.GetRequiredService(typeof(IRepository<>).MakeGenericType(aggregateRootType)))
                //    ?? throw new InvalidOperationException($"Couldn't create EventAddingRepositoryDecorator for aggregateRootType {aggregateRootType.Name}"));
            }

            return services;
        }
      
         internal static IServiceCollection AddMainDBPersistence(this IServiceCollection services, IConfiguration config)
        {
            services.AddOptions<DatabaseSettings>()
                .BindConfiguration(nameof(DatabaseSettings))
                .PostConfigure(databaseSettings =>
                {
                    _logger.Information("Current DB Provider: {dbProvider}", databaseSettings.DBProvider);
                })
                .ValidateDataAnnotations()
                .ValidateOnStart();
            return services
           .AddDbContext<MainDbContext>((p, m) =>
           {
             var databaseSettings = p.GetRequiredService<IOptions<DatabaseSettings>>().Value;
             databaseSettings.HydrateFromEnvironment();
             //  string converteddata =
            System.Text.ASCIIEncoding.ASCII.GetString(System.Convert.FromBase64String(databaseSettings.Cypher));
               //  string encryptedconstring = EncryptDecrypt.Decrypt(databaseSettings.ConnectionString, converteddata);
               string encryptedconstring = databaseSettings.MainConnectionstring;

               m.UseSqlServer(encryptedconstring,
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure();
                    });
               
           }).AddMainDbRepositories();
        }

        private static IServiceCollection AddMainDbRepositories(this IServiceCollection services)
        {
            // Add Repositories
            services.AddScoped(typeof(IRepository<>), typeof(ApplicationDbRepository<>));

            foreach (var aggregateRootType in
                typeof(IAggregateRoot).Assembly.GetExportedTypes()
                    .Where(t => typeof(IAggregateRoot).IsAssignableFrom(t) && t.IsClass)
                    .ToList())
            {
                // Add ReadRepositories.
                services.AddScoped(typeof(IReadRepository<>).MakeGenericType(aggregateRootType), sp =>
                    sp.GetRequiredService(typeof(IRepository<>).MakeGenericType(aggregateRootType)));

                // Decorate the repositories with EventAddingRepositoryDecorators and expose them as IRepositoryWithEvents.
                //services.AddScoped(typeof(IRepositoryWithEvents<>).MakeGenericType(aggregateRootType), sp =>
                //    Activator.CreateInstance(
                //        typeof(EventAddingRepositoryDecorator<>).MakeGenericType(aggregateRootType),
                //        sp.GetRequiredService(typeof(IRepository<>).MakeGenericType(aggregateRootType)))
                //    ?? throw new InvalidOperationException($"Couldn't create EventAddingRepositoryDecorator for aggregateRootType {aggregateRootType.Name}"));
            }

            return services;
        }
    }
}
