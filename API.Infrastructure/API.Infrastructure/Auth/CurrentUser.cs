using System.Security.Claims;
using DAL.Core.Interface;
namespace API.Infrastructure.Auth;

public class CurrentUser : ICurrentUser, ICurrentUserInitializer
{
    /// <summary>JWT claim type for partner id (from token payload).</summary>
    public const string ClaimTypePartnerId = "partnerId";
  

    /// <summary>JWT claim type for display name (from token payload).</summary>
    public const string ClaimTypeName = "Name";
    public const string UserId = "UserVal";

    private ClaimsPrincipal? _user;

    /// <summary>Display name from JWT "Name" claim, or Identity.Name.</summary>
    public string? Name => GetClaimValue(ClaimTypeName) ?? _user?.Identity?.Name;

    private Guid _userId = Guid.Empty;

    /// <summary>Partner code from JWT "partnerId" claim.</summary>
    public string? PartnerCode() => GetPartnerId();

    /// <summary>Gets the partner id from the JWT "partnerId" claim.</summary>
    public string? GetPartnerId() => GetClaimValue(ClaimTypePartnerId);

    /// <summary>Gets the value of a claim by type (e.g. "partnerId", "Name", "exp", "iss", "aud").</summary>
    public string? GetClaimValue(string claimType)
    {
        if (_user?.Claims == null || string.IsNullOrEmpty(claimType)) return null;
        // Try exact match first (works when MapInboundClaims = false)
        var exact = _user.FindFirst(claimType)?.Value;
        if (!string.IsNullOrEmpty(exact)) return exact;
        // Fallback: claim type may have been mapped to a URI (e.g. .../name or .../partnerid)
        var match = _user.Claims.FirstOrDefault(c =>
            string.Equals(c.Type, claimType, StringComparison.OrdinalIgnoreCase) ||
            c.Type.EndsWith("/" + claimType, StringComparison.OrdinalIgnoreCase));
        return match?.Value;
    }
    public Guid GetUserId() =>
        IsAuthenticated()
            ? Guid.Parse(GetClaimValue(UserId) ?? Guid.Empty.ToString())
            : _userId;

    //public string? GetUserEmail() =>
    //    IsAuthenticated()
    //        ? _user!.GetEmail()
    //        : string.Empty;

    public bool IsAuthenticated() =>
        _user?.Identity?.IsAuthenticated is true;

    public bool IsInRole(string role) =>
        _user?.IsInRole(role) is true;

    public IEnumerable<Claim>? GetUserClaims() =>
        _user?.Claims;

  

    public void SetCurrentUser(ClaimsPrincipal user)
    {
        if (_user != null)
        {
            throw new Exception("Method reserved for in-scope initialization");
        }

        _user = user;
    }

    public void SetCurrentUserId(string userId)
    {
        if (_userId != Guid.Empty)
        {
            throw new Exception("Method reserved for in-scope initialization");
        }

        if (!string.IsNullOrEmpty(userId))
        {
            _userId = Guid.Parse(userId);
        }
    }
}