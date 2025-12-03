using System.Security.Claims;
using System.Text;
using API.Infrastructure.Common.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Hosting;

namespace API.Infrastructure.Auth.Jwt;

public class ConfigureJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly JwtSettings _jwtSettings;
    private readonly IHostEnvironment _env;

    public ConfigureJwtBearerOptions(IOptions<JwtSettings> jwtSettings, IHostEnvironment env)
    {
        _jwtSettings = jwtSettings.Value;
        _env = env;
    }

    public void Configure(JwtBearerOptions options)
    {
        Configure(string.Empty, options);
    }

    public void Configure(string name, JwtBearerOptions options)
    {
        if (name != JwtBearerDefaults.AuthenticationScheme)
        {
            return;
        }

        byte[] key = Encoding.UTF8.GetBytes(ResolveSigningKey());

        options.RequireHttpsMetadata = !_env.IsDevelopment();
        options.IncludeErrorDetails = _env.IsDevelopment();
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateAudience = true,
            ValidIssuer = _jwtSettings.ValidIssuer,
            ValidAudience = _jwtSettings.ValidAudience,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse();
                if (!context.Response.HasStarted)
                {
                    throw new UnauthorizedException("Authentication Failed.");
                }

                return Task.CompletedTask;
            }
        };
    }

    private string ResolveSigningKey()
    {
        if (!string.IsNullOrWhiteSpace(_jwtSettings.Key))
        {
            return _jwtSettings.Key;
        }

        if (!string.IsNullOrWhiteSpace(_jwtSettings.KeyEnvironmentVariable))
        {
            var keyFromEnvironment = Environment.GetEnvironmentVariable(_jwtSettings.KeyEnvironmentVariable);
            if (!string.IsNullOrWhiteSpace(keyFromEnvironment))
            {
                return keyFromEnvironment;
            }

            throw new InvalidOperationException($"Environment variable '{_jwtSettings.KeyEnvironmentVariable}' for JWT signing key is not set.");
        }

        throw new InvalidOperationException("JWT signing key is not configured.");
    }
}