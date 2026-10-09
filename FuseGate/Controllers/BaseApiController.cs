using MediatR;
using Microsoft.AspNetCore.Mvc;
using API.Infrastructure.OpenApi;
using Microsoft.AspNetCore.Authorization;
using API.Infrastructure.Auth;
namespace EsbJson.Controllers;

[ApiController]

public class BaseApiController : ControllerBase
{
    private ISender _mediator = null!;

    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    /// <summary>
    /// Gets the PartnerCode value from the request headers.
    /// </summary>
    /// <returns>The PartnerCode value if present, otherwise null.</returns>
    protected string? GetPartnerCode()
    {
        if (Request.Headers.TryGetValue(PartnerCodeHeaderAttribute.PartnerCode, out var partnerCode))
        {
            return partnerCode.FirstOrDefault();
        }
        return null;
    }
}