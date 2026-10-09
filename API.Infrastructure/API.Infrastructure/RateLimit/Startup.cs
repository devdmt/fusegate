//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.RateLimiting;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using System.Threading.RateLimiting;

//namespace API.Infrastructure.RateLimit;

//internal static class Startup
//{
//    private const string GlobalPolicyName = "GlobalPolicy";
//    private const string FixedWindowPolicyName = "FixedWindowPolicy";
//    private const string SlidingWindowPolicyName = "SlidingWindowPolicy";
//    private const string TokenBucketPolicyName = "TokenBucketPolicy";
//    private const string ConcurrencyPolicyName = "ConcurrencyPolicy";

//    internal static IServiceCollection AddRateLimit(this IServiceCollection services, IConfiguration config)
//    {
//        var rateLimitSettings = config.GetSection(nameof(RateLimitSettings)).Get<RateLimitSettings>() 
//            ?? new RateLimitSettings();

//        if (!rateLimitSettings.Enabled)
//        {
//            return services;
//        }

//        return services.AddRateLimiter(options =>
//        {
//            // Configure Global Limiter (applies to all requests)
//            var globalPolicy = rateLimitSettings.GlobalPolicy ?? new GlobalPolicy();

//            // Fix the line causing CS0029 by replacing HttpContent with HttpContext
//            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
//            {
//                return RateLimitPartition.GetFixedWindowLimiter(
//                    partitionKey: GetPartitionKey(context),
//                    factory: partition => new FixedWindowRateLimiterOptions
//                    {
//                        PermitLimit = globalPolicy.PermitLimit,
//                        Window = TimeSpan.FromSeconds(globalPolicy.WindowSeconds),
//                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
//                        QueueLimit = globalPolicy.QueueLimit,
//                        AutoReplenishment = globalPolicy.AutoReplenishment
//                    });
//            });
//            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContent, string>(context =>
//            {
//                return RateLimitPartition.GetFixedWindowLimiter(
//                    partitionKey: GetPartitionKey(context),
//                    factory: partition => new FixedWindowRateLimiterOptions
//                    {
//                        PermitLimit = globalPolicy.PermitLimit,
//                        Window = TimeSpan.FromSeconds(globalPolicy.WindowSeconds),
//                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
//                        QueueLimit = globalPolicy.QueueLimit,
//                        AutoReplenishment = globalPolicy.AutoReplenishment
//                    });
//            });

//            // Add Fixed Window Policy (named policy)
//            if (rateLimitSettings.FixedWindow != null)
//            {
//                var fixedWindow = rateLimitSettings.FixedWindow;
//                options.AddFixedWindowLimiter(FixedWindowPolicyName, limiterOptions =>
//                {
//                    limiterOptions.PermitLimit = fixedWindow.PermitLimit;
//                    limiterOptions.Window = TimeSpan.FromSeconds(fixedWindow.WindowSeconds);
//                    limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
//                    limiterOptions.QueueLimit = fixedWindow.QueueLimit;
//                    limiterOptions.AutoReplenishment = fixedWindow.AutoReplenishment;
//                });
//            }

//            // Add Sliding Window Policy (named policy)
//            if (rateLimitSettings.SlidingWindow != null)
//            {
//                var slidingWindow = rateLimitSettings.SlidingWindow;
//                options.AddSlidingWindowLimiter(SlidingWindowPolicyName, limiterOptions =>
//                {
//                    limiterOptions.PermitLimit = slidingWindow.PermitLimit;
//                    limiterOptions.Window = TimeSpan.FromSeconds(slidingWindow.WindowSeconds);
//                    limiterOptions.SegmentsPerWindow = slidingWindow.SegmentsPerWindow;
//                    limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
//                    limiterOptions.QueueLimit = slidingWindow.QueueLimit;
//                });
//            }

//            // Add Token Bucket Policy (named policy)
//            if (rateLimitSettings.TokenBucket != null)
//            {
//                var tokenBucket = rateLimitSettings.TokenBucket;
//                options.AddTokenBucketLimiter(TokenBucketPolicyName, limiterOptions =>
//                {
//                    limiterOptions.TokenLimit = tokenBucket.TokenLimit;
//                    limiterOptions.TokensPerPeriod = tokenBucket.TokensPerPeriod;
//                    limiterOptions.ReplenishmentPeriod = TimeSpan.FromSeconds(tokenBucket.ReplenishmentPeriodSeconds);
//                    limiterOptions.AutoReplenishment = tokenBucket.AutoReplenishment;
//                    limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
//                    limiterOptions.QueueLimit = tokenBucket.QueueLimit;
//                });
//            }

//            // Add Concurrency Policy (named policy)
//            if (rateLimitSettings.Concurrency != null)
//            {
//                var concurrency = rateLimitSettings.Concurrency;
//                options.AddConcurrencyLimiter(ConcurrencyPolicyName, limiterOptions =>
//                {
//                    limiterOptions.PermitLimit = concurrency.PermitLimit;
//                    limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
//                    limiterOptions.QueueLimit = concurrency.QueueLimit;
//                });
//            }

//            // Add endpoint-specific policies
//            if (rateLimitSettings.EndpointPolicies != null && rateLimitSettings.EndpointPolicies.Any())
//            {
//                foreach (var endpointPolicy in rateLimitSettings.EndpointPolicies)
//                {
//                    options.AddFixedWindowLimiter(endpointPolicy.PolicyName, limiterOptions =>
//                    {
//                        limiterOptions.PermitLimit = endpointPolicy.PermitLimit;
//                        limiterOptions.Window = TimeSpan.FromSeconds(endpointPolicy.WindowSeconds);
//                        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
//                        limiterOptions.QueueLimit = 0;
//                        limiterOptions.AutoReplenishment = true;
//                    });
//                }
//            }

//            // Configure rejection response
//            options.DefaultRejectionStatusCode = 429;
//            options.OnRejected = async (context, cancellationToken) =>
//            {
//                context.HttpContext.Response.StatusCode = 429;
//                await context.HttpContext.Response.WriteAsync("Rate limit exceeded. Please try again later.", cancellationToken);
//            };
//        });
//    }

//    internal static IApplicationBuilder UseRateLimit(this IApplicationBuilder app, IConfiguration config)
//    {
//        var rateLimitSettings = config.GetSection(nameof(RateLimitSettings)).Get<RateLimitSettings>() 
//            ?? new RateLimitSettings();

//        if (!rateLimitSettings.Enabled)
//        {
//            return app;
//        }

//        return app.UseRateLimiter();
//    }

//    private static string GetPartitionKey(HttpContent context)
//    {
//        // You can customize this to use IP address, user ID, API key, etc.
//        // For now, using IP address as the partition key
//        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
//    }
//}
