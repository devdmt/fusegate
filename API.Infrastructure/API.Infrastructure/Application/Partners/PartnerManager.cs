using DAL;
using DAL.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapster;
using DAL.Model;
//using API.Infrastructure.Common.Services;
using Microsoft.EntityFrameworkCore;
using API.Infrastructure.Auth.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Dapper;
using System.Runtime.InteropServices;
using YamlDotNet.Core;
using API.Infrastructure.Interface;

namespace API.Infrastructure.Auth.Services
{
    public class PartnerManager : IPartnerManager
    {
        private readonly ApplicationDbContext _db;
        private readonly IEncryptionService _enc;
        private readonly SecuritySettings _security;
        readonly Isettings _isettings;
        public PartnerManager(ApplicationDbContext db, IEncryptionService service, IOptions<SecuritySettings> options, Isettings isettings)
        {
            _db = db;
            _enc = service;
            _security = options.Value;
            _isettings = isettings;
        }

        public async Task<AuthResponse> AuthenticatePartner(UserLoginDTO userLogin)
        {
            var response = new AuthResponse();
            try
            {
                //byte[] encConsumerKey = Convert.FromBase64String(userLogin.consumerKey);

                string consumerKey =  Convert.ToBase64String(Encoding.UTF8.GetBytes(userLogin.consumerKey));
               

                //var authuser = await _db.user.Where(a => a.ConsumerKey ==consumerKey).FirstOrDefaultAsync();

                var authuser = await _db.Connection.QueryFirstOrDefaultAsync<APIUSER>("select Convert(nvarchar(50),[Id]) as Id ,[FullName] ,[Configuration] ,[PartnerId] ,[ConsumerKey]" +
                    " ,[ConsumerSecret] ,[Salt] ,[IsEnabled] ,[IpAddress] ,[HostUrl] ,[HostPort] ,[CreatedBy] ,[UpdatedBy] ,[CreatedDate] ,[UpdatedDate] ,[UserName] ,[NormalizedUserName] ,[Email] ,[NormalizedEmail] ,[EmailConfirmed] ,[PasswordHash] ,[SecurityStamp] ,[ConcurrencyStamp] ,[PhoneNumber] ,[PhoneNumberConfirmed] ,[TwoFactorEnabled] ,[LockoutEnd]" +
                    " ,[LockoutEnabled] ,[AccessFailedCount] ,[AccountType]  FROM [dbo].[APIUSERS]" +
                    " where ConsumerKey='"+ consumerKey +"'");
                if (authuser != null)
                {

                    string partnercode = (string)_db.Connection.ExecuteScalar("select [PartnerCode] as partnercode " +
                        "from [dbo].[Partners] where Id='" + authuser.PartnerId + "' ");
                     _isettings.LogRequests(partnercode, "Partner code AuthenticatePartner", RequestType.Error);
                    //check the password
                    byte[] encsalt = Convert.FromBase64String(authuser.Salt);
                     string salt =  Encoding.UTF8.GetString(encsalt);

                   
                    string password = _enc.EncryptText(userLogin.consumersecret, salt);
                    if (password == authuser.ConsumerSecret)
                    {
                        var authClaims = new List<Claim>
                {
                    
                    new Claim("partnerId",partnercode),
                    new Claim("UserVal",authuser.Id),
                   // new Claim("info",customer.CustomerInfoCustId),
                   new Claim("Name",authuser.FullName),
                        };
                        string token = GenerateEncryptedToken(GetSigningCredentials(), authClaims);
                        response.ExpireTime = _security.jwtSettings.TokenExpirationInMinutes;
                        response.Token = token;
                        response.partnerCode = partnercode;
                       response.RefreshToken = "";
                    }   
                }
                
            }
            catch (Exception ex)
            {
 _isettings.LogRequests(ex.Message, "AuthenticatePartner", RequestType.Error);
            }
            return response;
        }



        private SigningCredentials GetSigningCredentials()
        {
            if (string.IsNullOrEmpty(_security.jwtSettings.key))
            {
                throw new InvalidOperationException("No Key defined in JwtSettings config.");
            }

            byte[] secret = Encoding.UTF8.GetBytes(_security.jwtSettings.key);
            return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
        }
        private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
        {
            var token = new JwtSecurityToken(
                 issuer: _security.jwtSettings.ValidIssuer,
           audience: _security.jwtSettings.ValidAudience,
               claims: claims,
               expires: DateTime.UtcNow.AddHours(4),
               signingCredentials: signingCredentials);
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
        public async Task<ResponseDTO> CreatePartner(PartnerDTO userLogin)
        {
            var response = new ResponseDTO();
            try
            {
                var existpartner = await _db.Partners.Where(a => a.PartnerName == userLogin.PartnerName).FirstOrDefaultAsync();
                if (existpartner != null)
                {
                    response.ErrorMsg = "Partner already exists";
                    response.Success = false;
                    return response;
                }
                if (existpartner == null)
                {
                    var partner = userLogin.Adapt<Partners>();
                    partner.Active = true;
                    partner.PartnerCode = userLogin.PartnerName.ToUpper().Substring(0, 2) + GenerateRandomNumber(5);

                    _db.Partners.Add(partner);
                    await _db.SaveChangesAsync();
                    response.Success = true;
                    response.ErrorMsg = "";
                    response.ResponseId = partner.Id.ToString();
                }

            }
            catch (Exception ex)
            {

            }
            return response;
        }
        public async Task<ResponseDTO> CreatePartnerUser(PartnerUserDTO partnerUser)
        {
            var response = new ResponseDTO();
            try
            {
                string createdBy = "";
                string consumerKey = GenerateRandomNumber(14, true);
                string saltKey = _enc.CreateSaltKey(16);
                string consumerSecret = GenerateRandomNumber(26, true);

                //  consumerSecret= 
                var newUser = new APIUSER()
                {
                     Id=Guid.NewGuid().ToString(),
                    ConsumerKey =  Convert.ToBase64String(Encoding.UTF8.GetBytes(consumerKey)),
                    Salt = Convert.ToBase64String(Encoding.UTF8.GetBytes(saltKey)),
                    ConsumerSecret = _enc.EncryptText(consumerSecret, saltKey),//consumerSecret,
                    PasswordHash = _enc.EncryptText(consumerSecret, saltKey),
                    FullName = partnerUser.FullName,
                    IpAddress = partnerUser.IpAddress,
                    HostPort = partnerUser.HostPort,
                    HostUrl = partnerUser.HostUrl,
                    IsEnabled = partnerUser.IsEnabled,
                    PartnerId = partnerUser.PartnerId,
                    CreatedDate = DateTime.Now,
                    CreatedBy = createdBy
                };
                _db.user.Add(newUser);
                await _db.SaveChangesAsync();

                response.Success = true;
                response.ErrorMsg = "";
                //response.ResponseId = newUser.Id.ToString();
            }
            catch (Exception ex)
            {
                
            }
            return response;
        }
        string GenerateRandomNumber(int length, bool lower = false)
        {
            var randomNo = "";
            var builder = new StringBuilder(length);
            Random random = new Random();
            // Unicode/ASCII Letters are divided into two blocks
            // (Letters 65–90 / 97–122):
            // The first group containing the uppercase letters and
            // the second group containing the lowercase.  

            // char is a single Unicode character  
            string offset = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            if (lower)
            {
                offset = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            }
            // A...Z or a..z: length=26  

            randomNo = new string(Enumerable.Repeat(offset, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());

            //for (var i = 0; i < 3; i++)
            //{
            //    var @char = (char)_random.Next(offset, offset + lettersOffset);
            //    builder.Append(@char);
            //}
            //builder.Append(_random.Next(1, 9));
            return randomNo;
        }

    }
}
