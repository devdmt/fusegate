namespace API.Infrastructure.RateLimit;

public class RateLimitSettings
{
    public bool Enabled { get; set; } = true;
    
    /// <summary>
    /// Global rate limit policy settings
    /// </summary>
    public GlobalPolicy? GlobalPolicy { get; set; }
    
    /// <summary>
    /// Per-endpoint rate limit policies
    /// </summary>
    public List<EndpointPolicy>? EndpointPolicies { get; set; }
    
    /// <summary>
    /// Fixed window rate limit settings
    /// </summary>
    public FixedWindowPolicy? FixedWindow { get; set; }
    
    /// <summary>
    /// Sliding window rate limit settings
    /// </summary>
    public SlidingWindowPolicy? SlidingWindow { get; set; }
    
    /// <summary>
    /// Token bucket rate limit settings
    /// </summary>
    public TokenBucketPolicy? TokenBucket { get; set; }
    
    /// <summary>
    /// Concurrency rate limit settings
    /// </summary>
    public ConcurrencyPolicy? Concurrency { get; set; }
}

public class GlobalPolicy
{
    public int PermitLimit { get; set; } = 100;
    public int WindowSeconds { get; set; } = 60;
    public int QueueLimit { get; set; } = 0;
    public bool AutoReplenishment { get; set; } = true;
}

public class EndpointPolicy
{
    public string Endpoint { get; set; } = string.Empty;
    public string PolicyName { get; set; } = string.Empty;
    public int PermitLimit { get; set; } = 10;
    public int WindowSeconds { get; set; } = 60;
}

public class FixedWindowPolicy
{
    public int PermitLimit { get; set; } = 100;
    public int WindowSeconds { get; set; } = 60;
    public int QueueLimit { get; set; } = 0;
    public bool AutoReplenishment { get; set; } = true;
}

public class SlidingWindowPolicy
{
    public int PermitLimit { get; set; } = 100;
    public int WindowSeconds { get; set; } = 60;
    public int SegmentsPerWindow { get; set; } = 1;
    public int QueueLimit { get; set; } = 0;
}

public class TokenBucketPolicy
{
    public int TokenLimit { get; set; } = 100;
    public int TokensPerPeriod { get; set; } = 10;
    public int ReplenishmentPeriodSeconds { get; set; } = 60;
    public bool AutoReplenishment { get; set; } = true;
    public int QueueLimit { get; set; } = 0;
}

public class ConcurrencyPolicy
{
    public int PermitLimit { get; set; } = 10;
    public int QueueLimit { get; set; } = 0;
}
