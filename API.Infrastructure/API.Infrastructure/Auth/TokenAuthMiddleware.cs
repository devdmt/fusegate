

//using FCB.Application.Identity.Token;
using API.Infrastructure.Auth.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace API.Infrastructure.Auth
{
    public class TokenAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtSettings _jwtSettings;
        //private readonly ITokenServices _tokenServices;
        private readonly IConfiguration _configuration;
        public TokenAuthMiddleware(RequestDelegate next, IOptions<JwtSettings> jwtSettings, IConfiguration configuration)

        {
            _next = next;
            _jwtSettings = jwtSettings.Value;
            _configuration = configuration;
            
        }

        public async Task Invoke(HttpContext context)
        {
            //var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            //var deviceId = context.Request.Headers["DeviceId"].FirstOrDefault();
            //context.Items["UserCount"] = deviceId;
            //if (token != null && deviceId !=null)
                //Validate the token
              //  attachUserToContext(context, _tokenServices, token, deviceId);
            await _next(context);
        }
       
        //private void attachUserToContext(HttpContext context, ITokenServices _tokenServices, string token,string deviceId)
        //{
        //    try
        //    {
        //        var tokenHandler = new JwtSecurityTokenHandler();
        //        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        //        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
        //        tokenHandler.ValidateToken(token, new TokenValidationParameters
        //        {
        //            ValidateIssuerSigningKey = true,
        //            ValidateAudience = true,
        //            ValidateLifetime = true,
        //            IssuerSigningKey = authSigningKey,
        //            ValidIssuer = _jwtSettings.ValidIssuer,
        //            ValidAudience = _jwtSettings.ValidAudience,
        //            // set clockskew to zero so tokens expire exactly at token expiration time.
        //            ClockSkew = TimeSpan.Zero
        //        }, out SecurityToken validatedToken);
        //        var jwtToken = (JwtSecurityToken)validatedToken;
        //        var userId = jwtToken.Claims.First(x => x.Type == "userid").Value;
        //        // attach user to context on successful jwt validation
        //        context.Items["UserCount"] = _tokenServices.ValidateTokenUser(userId,deviceId);
        //    }
        //    catch (Exception ex)
        //    {

        //        // log this with invalid token
        //        // user is not attached to context so request won't have access to secure routes
        //    }
        //}
    }
}
