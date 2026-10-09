using API.Infrastructure.Interface;
using DAL;
using DAL.ModelView;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Infrastructure.Auth;
using API.Infrastructure.Common.Services;

namespace API.Infrastructure.Application.Account
{
    public class AuthCore :IAuthenticate
    {
        readonly Isettings _isettings;
        //readonly ApplicationDbContext _db;
        readonly IPartnerManager _partnerManager;
        public AuthCore(Isettings isettings, IPartnerManager partnerManager) 
        {
        _isettings = isettings;
          
            _partnerManager = partnerManager;
        }

       public async  Task<AuthResponse> GenerateToken(string Key)
        {
            // Expecting header like: "Basic base64(clientKey:secret)"
            if (string.IsNullOrWhiteSpace(Key))
            {
                return new AuthResponse();
            }

            var parts = Key.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length != 2 || !parts[0].Equals("Basic", StringComparison.OrdinalIgnoreCase))
            {
                return new AuthResponse();
            }

            string base64 = parts[1];
            string decoded;
            try
            {
                var bytes = Convert.FromBase64String(base64);
                decoded = Encoding.UTF8.GetString(bytes);
            }
            catch(Exception e)
            {
                _isettings.LogRequests(e.Message, "GenerateToken-Convert.FromBase64String", RequestType.Error);
                return new AuthResponse();
            }

            // decoded format: clientKey:secret
            var sepIndex = decoded.IndexOf(":");
            if (sepIndex <= 0 || sepIndex == decoded.Length - 1)
            {
                return new AuthResponse();
            }

            var clientKey = decoded.Substring(0, sepIndex);
            var secret = decoded.Substring(sepIndex + 1);

            var dto = new UserLoginDTO
            {
                consumerKey = clientKey,
                consumersecret = secret
            };

            var auth = await _partnerManager.AuthenticatePartner(dto);
            return auth;
        }
    }
}
