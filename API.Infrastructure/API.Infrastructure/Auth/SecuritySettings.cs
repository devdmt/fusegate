namespace API.Infrastructure.Auth;

public class SecuritySettings
{
    public string? Provider { get; set; }
    public bool RequireConfirmedAccount { get; set; }
    public JwtSettings jwtSettings { get; set; }
}
public class JwtSettings
{
    public string ValidAudience { get; set;}
    public string ValidIssuer { get; set;}
    public string Secret { get; set;}
    public string key { get; set; }
    public string? KeyEnvironmentVariable { get; set; }
    public int TokenExpirationInMinutes { get; set; }
    public int RefreshTokenExpirationInDays { get; set; }

}