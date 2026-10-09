using API.Infrastructure.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace API.Infrastructure.BackgroundServices;

/// <summary>
/// Background service that runs IComunication.ProcessSMS on a configurable interval.
/// Uses a scope per execution so scoped dependencies (e.g. DbContext) are resolved correctly.
/// Prevents overlapping runs and respects CancellationToken for graceful shutdown.
/// </summary>
public sealed class SmsProcessingBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptions<SmsProcessingOptions> _options;
    private readonly ILogger<SmsProcessingBackgroundService> _logger;
    private readonly SemaphoreSlim _runLock = new(1, 1);
    private int _consecutiveFailures;

    public SmsProcessingBackgroundService(
        IServiceScopeFactory scopeFactory,
        IOptions<SmsProcessingOptions> options,
        ILogger<SmsProcessingBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var opts = _options.Value;
        if (!opts.Enabled)
        {
            _logger.LogInformation("SMS processing background service is disabled. Exiting.");
            return;
        }

        _logger.LogInformation("SMS processing background service started. Interval: {IntervalSeconds}s", opts.IntervalSeconds);

        var interval = TimeSpan.FromSeconds(Math.Max(1, opts.IntervalSeconds));
        using var timer = new PeriodicTimer(interval);

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken).ConfigureAwait(false))
            {
                if (stoppingToken.IsCancellationRequested)
                    break;

                await RunOnceAsync(stoppingToken).ConfigureAwait(false);

                if (opts.FailureCountBeforeBackoff > 0 && _consecutiveFailures >= opts.FailureCountBeforeBackoff)
                {
                    var backoffDelay = GetBackoffDelay(opts);
                    _logger.LogWarning(
                        "SMS processing had {Count} consecutive failures. Backing off for {BackoffMs}ms.",
                        _consecutiveFailures, backoffDelay.TotalMilliseconds);
                    await Task.Delay(backoffDelay, stoppingToken).ConfigureAwait(false);
                }
            }
        }
        catch (OperationCanceledException x)
        {
            // Expected on shutdown
        }
        finally
        {
            _logger.LogInformation("SMS processing background service stopped.");
        }
    }

    private async Task RunOnceAsync(CancellationToken stoppingToken)
    {
        if (!_runLock.Wait(0))
        {
            _logger.LogDebug("Skipping SMS processing run: previous run still in progress.");
            return;
        }

        try
        {
            _logger.LogDebug("Starting SMS processing run.");
            stoppingToken.ThrowIfCancellationRequested();

            await using (var scope = _scopeFactory.CreateAsyncScope())
            {
                var communication = scope.ServiceProvider.GetRequiredService<IComunication>();
                await communication.ProcessSMS().ConfigureAwait(false);
            }
            Thread.Sleep(10000);
            _consecutiveFailures = 0;
            _logger.LogDebug("SMS processing run completed.");
        }
        catch (OperationCanceledException)
        {
            _logger.LogDebug("SMS processing run cancelled.");
            throw;
        }
        catch (Exception ex)
        {
            _consecutiveFailures++;
            _logger.LogError(ex, "SMS processing run failed (consecutive failures: {Count}).", _consecutiveFailures);
        }
        finally
        {
            _runLock.Release();
        }
    }

    private static TimeSpan GetBackoffDelay(SmsProcessingOptions opts)
    {
        if (opts.FailureCountBeforeBackoff <= 0 || opts.BackoffFactor <= 0)
            return TimeSpan.Zero;

        var delaySeconds = opts.IntervalSeconds * Math.Pow(opts.BackoffFactor, opts.FailureCountBeforeBackoff);
        var capped = Math.Min(delaySeconds, opts.MaxBackoffSeconds);
        return TimeSpan.FromSeconds(capped);
    }

    public override void Dispose()
    {
        _runLock.Dispose();
        base.Dispose();
    }
}
