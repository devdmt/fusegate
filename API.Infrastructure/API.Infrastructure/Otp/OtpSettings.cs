namespace API.Infrastructure.Otp;

/// <summary>
/// OTP configuration. Bind from "Otp" in appsettings.json.
/// Settings keys: Otp:Length (default 6), Otp:ExpiryMinutes (default 5).
/// </summary>
public class OtpSettings
{
    public const string SectionName = "Otp";

    /// <summary>OTP length in digits. Default 6.</summary>
    public int Length { get; set; } = 6;

    /// <summary>OTP expiry in minutes from creation. Default 5.</summary>
    public int ExpiryMinutes { get; set; } = 5;
}
