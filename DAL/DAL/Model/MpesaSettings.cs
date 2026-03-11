using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
      public class MpesaSettings //: AuditableEntity
    {
        [Required]
        public int Id { get; set; }
        public string? AppName { get; set; }
        public string? MpesaUsername { get; set; }
        public string? PaybillPass { get; set; }
        public string? PaybillId { get; set; }       
        public string? ConsumerKey { get; set; }
        public string? Shortcode { get; set; }
        public string? UniqueURLCode { get; set; }
        public string? Phonenumber { get; set; }
        public string? Email { get; set; }
        public bool? Registered { get; set; }
        public string? ConsumerSecret { get; set; }
        public string? AuthCode { get; set; }
        public string? SecurityCredential { get; set; }
        public string? GrantType { get; set; }
        public string? PaybillName { get; set; }
        public string? ReceiverPartyIdentifierType { get; set; }    
        public string? TransactionType { get; set; }
        public bool Active { get; set; }
        public string? B2CUtilityAccountAvailableFunds { get; set; }
        public string? B2CWorkingAccountAvailableFunds { get; set; }
        
        public string? DeletedById { get; set; }
        public string? PassKey { get; set; }
        //public string? PPassword { get; set; }
        public string? SaltKey { get; set; }
        public bool? Deleted { get; set; }
        public bool DefaultPaybill { get; set; } = false;
        public PaybillType paybillType { get; set; }
        public DateTime? DeletedOn { get; set; }

            
    }
    public enum EndPointType
    {
      
        Mpesa_Token = 0,       
        Mpesa_STK_CallbackUrl = 1,
        Mpesa_Pension_STK_CallbackUrl = 3,
        Mpesa_STK_RequestUrl = 2,
        Mpesa_Ratiba_CallBack=4,
        Mpesa_Ratiba_Request=5
    }
    public enum PaybillType
    {
        Paybill, Tillnumber,Ratiba
    }

    public class MpesaToken
    {
        [Key]
        public Guid Id { get; set; }= Guid.NewGuid();
        public string Access_token { get; set; }
        public int Expires_in { get; set; } = 1;
        public virtual MpesaSettings Mpesa { get; set; }
        public int MpesaId { get; set; }
        public string paybillid { get; set; }
        public DateTime createdon { get; set; }

    }
}
