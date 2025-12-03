using System.Security.Claims;

namespace DAL.Core.Interface;

public interface ICurrentUser
{
    string? Name { get; }

    Guid GetUserId();

    //string? GetUserEmail();

    bool IsAuthenticated();

    bool IsInRole(string role);

    IEnumerable<Claim>? GetUserClaims();
}