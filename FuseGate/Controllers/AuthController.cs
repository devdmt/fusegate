using API.Infrastructure.Common.Exceptions;
using API.Infrastructure.Interface;
using API.Infrastructure.Middleware;
using DAL.ModelView;
using EsbJson.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;

namespace IPP.EsbJson.API.Controllers
{
    public class AuthController : VersionNeutralApiController
    {
        private readonly IAuthenticate _auth;
         private readonly IConfiguration _configuration;
        readonly Isettings _settings;
        public AuthController(IAuthenticate auth, IConfiguration configuration,Isettings isettings)
        {
            _auth = auth;
            _configuration = configuration;
            _settings = isettings;

        }

        /// <summary>
        /// Generates a new authentication token for a valid request using the Authorization header.
        /// </summary>
        /// <remarks>
        /// Expects a Basic or Bearer Authorization header with valid credentials.
        /// </remarks>
        /// <response code="200">Returns the authentication token and additional information.</response>
        /// <response code="401">Returned when authentication fails. Response includes error details.</response>
        [HttpPost("GenerateToken")]
        [ProducesResponseType(typeof(AuthResponse), 200)]
        [ProducesResponseType(typeof(ErrorResult), 401)]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateToken()
        {
            var authHeader = Request.Headers[HeaderNames.Authorization].FirstOrDefault();
            
            // Guard: Check if authorization header is present
            if (string.IsNullOrEmpty(authHeader))
            {

                throw new UnauthorizedException("Authorization header is required");
            }
             var remoteIp = HttpContext.Connection.RemoteIpAddress;
        if (remoteIp == null)
            return Unauthorized("Unable to determine source IP.");

        var clientIp = remoteIp.MapToIPv4().ToString();

        var whitelist = _configuration
            .GetSection("IpWhitelist")
            .Get<string[]>() ?? Array.Empty<string>();

        if (!whitelist.Contains("*") && !whitelist.Contains(clientIp, StringComparer.OrdinalIgnoreCase))
            {
                _settings.LogRequests($"Unauthorized access attempt from IP: {clientIp}","GenerateToken", RequestType.Info);
                return Unauthorized($"Your IP {clientIp} is not whitelisted.");
            }
         
            var result = await _auth.GenerateToken(authHeader);
            
            // Guard: Check if token generation was successful
            if (result.Token == null)
            {
                throw new UnauthorizedException("Invalid credentials provided");
            }
            
            return Ok(result);
        }
    }
}
