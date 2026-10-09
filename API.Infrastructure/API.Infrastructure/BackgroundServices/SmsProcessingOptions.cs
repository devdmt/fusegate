namespace API.Infrastructure.BackgroundServices;

/// <summary>
/// Configuration for the SMS processing background service.
/// Bind from "SmsProcessing" in appsettings.json.
/// </summary>
public class SmsProcessingOptions
{
    public const string SectionName = "SmsProcessing";

    /// <summary>
    /// Whether the SMS processing background service is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Interval between each ProcessSMS run in seconds.
    /// </summary>
    public int IntervalSeconds { get; set; } = 1;

    /// <summary>
    /// After this many consecutive failures, delay before next run is multiplied by backoff factor.
    /// Zero disables backoff.
    /// </summary>
    public int FailureCountBeforeBackoff { get; set; } = 3;

    /// <summary>
    /// Multiplier for delay when in backoff (e.g. 2 = double the interval each time, capped by MaxBackoffSeconds).
    /// </summary>
    public double BackoffFactor { get; set; } = 2.0;

    /// <summary>
    /// Maximum delay in seconds when using backoff.
    /// </summary>
    public int MaxBackoffSeconds { get; set; } = 60;
}
