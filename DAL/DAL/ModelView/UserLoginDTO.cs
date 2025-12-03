using DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ModelView
{
    public class UserLoginDTO
    {
        public string consumerKey { get; set; }
        public string consumersecret { get; set; }
    }
    public class AuthResponse
    {
        
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public int ExpireTime { get; set; }
       
    }


    public class PartnerUserDTO
    {
        public string FullName { get; set; }
       
        public string PartnerId { get; set; }
      
      
        public bool IsEnabled { get; set; }
    
        public string? IpAddress { get; set; }
      
        public string? HostUrl { get; set; }
      
        public string? HostPort { get; set; }
   
    }
}
