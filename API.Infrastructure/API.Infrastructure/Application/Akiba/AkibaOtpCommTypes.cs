namespace API.Infrastructure.Application.Akiba;

public enum AkibaOtpCommType
{
    SMS,
    Email,
    All
}

public sealed class AkibaOtpAddRequest
{
    public Guid CustomerId { get; init; }
    public string? UserId { get; init; }
    public string Firstname { get; init; } = string.Empty;
    public string? EmailDisplayName { get; init; }
    public AkibaOtpCommType CommType { get; init; } = AkibaOtpCommType.All;
}
