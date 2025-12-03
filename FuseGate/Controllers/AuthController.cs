using API.Infrastructure.Interface;
using API.Infrastructure.Middleware;
using API.Infrastructure.Common.Exceptions;
using EsbJson.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using DAL.ModelView;

namespace IPP.EsbJson.API.Controllers
{
    public class AuthController : VersionNeutralApiController
    {
        private readonly IAuthenticate _auth;
        public AuthController(IAuthenticate auth)
        {
            _auth=auth; 
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
        public async Task<IActionResult> GenerateToken()
        {
            var authHeader = Request.Headers[HeaderNames.Authorization].FirstOrDefault();
            
            // Guard: Check if authorization header is present
            if (string.IsNullOrEmpty(authHeader))
            {
                throw new UnauthorizedException("Authorization header is required");
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
