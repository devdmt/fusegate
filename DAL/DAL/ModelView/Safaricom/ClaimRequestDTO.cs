using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using DAL.Model.Safaricom;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DAL.ModelView.Safaricom
{
    public class ValidateClaimDTO
    {
        public string? Idnumber { get; set; }
        public string PhoneId { get; set; }
    }
    public class ValidateClaimRespose
    {
        public ValidateClaimRespose()
        {
            ClaimStatus = ClaimStatus.pending;
        }
        public string Response { get; set; }
        public ClaimStatus ClaimStatus { get; set; }
    }
    public class ClaimRequestDTOSaveForLaterDetails
    {
         public string? Partsid { get; set; }
        public long? Id { get; set; }
        public string? phoneUpload { get; set; }
        public string? abstractAttachment { get; set; }
        public string? imeiUpload { get; set; }
         public string UserId { get; set; }
        public string ShopId { get; set; }
        public int shopType { get; set; }
        public string? PartnerCode { get; set; }
        public string? CustomerName { get; set; }
        public string PhoneId { get; set; }
        // public string? ClaimRefNumber { get; set; }
        public SourceOfClaim? sourceOfClaim { get; set; }
        public string? IDNumber { get; set; }
        // public List<string>? IMEINO { get; set; }
        public ClaimType? ClaimType { get; set; }
        // public string? DamagePart { get; set; }
        public List<int>? Partid { get; set; }
        public double? PartCost { get; set; } = 0;
        public double? ReplacementCost { get; set; }
        public string? IncidentDate { get; set; }
        public double? LabourCost { get; set; } = 0;
        public string? Narration { get; set; }
        public string? AlternativeContact { get; set; }
        
    }
    public class ClaimRequestDTOSaveForLater:ClaimRequestDTOPortal
    { 
        public string? Partsid { get; set; }
        public long? Id { get; set; }

    }
     [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SourceOfClaim
    {
        [EnumMember(Value = "store")] store, [EnumMember(Value = "dealer")] dealer, [EnumMember(Value = "walkin")] walkin
    }
    public class ClaimRequestDTOPortal
    {
        public string UserId { get; set; }
        public string ShopId { get; set; }
        public int shopType { get; set; }
        public string? PartnerCode { get; set; }
        public string? CustomerName { get; set; }
        public string PhoneId { get; set; }
        // public string? ClaimRefNumber { get; set; }
        public SourceOfClaim? sourceOfClaim { get; set; }
        public string? IDNumber { get; set; }
        // public List<string>? IMEINO { get; set; }
        public ClaimType ClaimType { get; set; }
        // public string? DamagePart { get; set; }
        public List<int>? Partid { get; set; }
        public double? PartCost { get; set; } = 0;
        public double? ReplacementCost { get; set; }
        public string? IncidentDate { get; set; }
        public double? LabourCost { get; set; } = 0;
        public string? Narration { get; set; }
        public string? AlternativeContact { get; set; }
        
        public FileDetails? AbstractAttachment { get; set; }

        public FileDetails? phoneUpload { get; set; }
        public FileDetails? ImeiUpload { get; set; }
        public FileDetails? deactivationProof { get; set; }
        public long? SaveForLaterId { get; set; }

    }
    public class ReplaceRequestDeviceDTO
    {
        public string? response_code { get; set; }
        public string? response_message { get; set; }
        public string? claim_ref { get; set; }
         public double devicecost {  get; set; }
         public List<string>? IMEINO { get; set; }
         public string? replaceDate {  get; set; }
         public string? merchantId {  get; set; }
        public string? primary_imei {  get; set; }
        public string? transactionRef {  get; set; }

    }
     public class ReplaceClaimDTO
    {
        public string? PartnerCode { get; set; }
        public string? CustomerName { get; set; }
        public string? ClaimRefNumber { get; set; }
        public string? RequestId { get; set; }
        public string? IDNumber { get; set; }
        public List<string>? IMEINO { get; set; }
        public ClaimType? ClaimType { get; set; }
        public string? DamagePart { get; set; }
        public int? Partid { get; set; } = 0;
        public double PartCost { get; set; } = 0;
        public double? ReplacementCost { get; set; }
        public string IncidentDate { get; set; }
        public double LabourCost { get; set; } = 0;
        public string? Narration { get; set; }
        public FileDetails? AbstractAttachment { get; set; }
        public FileDetails? phoneUpload { get; set; }
        public FileDetails? ImeiUpload { get; set; }  

    }
    public class ClaimRequestDTO
    {
        public string? PartnerCode { get; set; }
        public string? CustomerName { get; set; }
       // public string PhoneId { get; set; }
        public string? ClaimRefNumber { get; set; }
        public string? RequestId { get; set; }
        public string? IDNumber { get; set; }
        public List<string>? IMEINO { get; set; }
        public ClaimType? ClaimType { get; set; }
        public string? DamagePart { get; set; }
        public int? Partid { get; set; } = 0;
        public double PartCost { get; set; } = 0;
        public double? ReplacementCost { get; set; }
        public string IncidentDate { get; set; }
        public double LabourCost { get; set; } = 0;
        public string? Narration { get; set; }
        //public string loanRefNo { get; set; }
        //public string loanBal { get; set; }
        // public string ClaimDate {  get; set; }
        public FileDetails? AbstractAttachment { get; set; }
        //public FileDetails? MedicalAttachment {  get; set; }
        //public FileDetails? policeAbstractUpload {  get; set; }
        public FileDetails? phoneUpload { get; set; }
        public FileDetails? ImeiUpload { get; set; }
        //public  IFormFile?  Abstract {  get; set; }
        //public string? AbstractName { get; set; }   

    }
    public class Approvedispatch
    {
       
        public string? ClaimId { get; set; }
        public string? ShopId { get; set; }
        public string? UserId { get; set; }
        public ClaimType? claimType { get; set; }
    }
    public class CreditLifeApproveUpload
    {
        public string? UserId { get; set; }
        public string? Id { get; set; }
    }

    public class SendNotificationRequest
    {
        public string UserId { get; set; }
        public string ShopId { get; set; }
        public string ClaimId { get; set; }
    }
    public class CreditLifeUpload
    {
        public string? UserId { get; set; }
        public string? Ip { get; set; }
        public string? Browser { get; set; }
        public FileDetails? fileDetails { get; set; }
    }
    public class ClaimSearchSaveForLaterDTO
    {
        public string? Id { get; set; }  
        public string? request { get; set; }
        public string shopId { get; set; }
        public string UserId { get; set; }
        public ClaimStatus? ClaimStatus { get; set; }

    }
    public class ClaimSearchDTO
    {
        public string? request { get; set; }
        public string shopId { get; set; }
        public string UserId { get; set; }
        public ClaimStatus? ClaimStatus { get; set; }
        public bool? Dispatch { get; set; }=false;

    }
    public class NofiticationCustomerDetails
    {
        public string CustomerName { get; set; }
        public int Id { get; set; }
        public string phonenumber { get; set; }
    }
    public class Notificationadd
    {
        public Notificationadd()
        {
            Code = "";
        }
        public string RequestId { get; set; }
        public string ShopId { get; set; }
        public string UserId { get; set; }
        //public string CustomerId { get; set;}
        public NotificationType notificationType { get; set; }
        public string? Code { get; set; }
        //public string Message { get; set;}
        //public string phonennumber   { get; set;}
    }
    public class FileDetails
    {
        public string name { get; set; }

        public string extension { get; set; }
        public string data { get; set; }
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum extension
    {
        //  [Description(".jpg,.png,.jpeg")]
        [EnumMember(Value = ".jpg")]
        jpg,
        [EnumMember(Value = ".png")]
        png,
        [EnumMember(Value = ".jpeg")]
        jpeg,

        [EnumMember(Value = ".PDF")]
        PDF,
        [EnumMember(Value = ".docx")]
        doc
    }


    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ClaimType
    {
        [EnumMember(Value = "damage")] damage, [EnumMember(Value = "theft")] theft, [EnumMember(Value = "creditlife")] creditlife
    }

    public class ClaimResponseDTO
    {
        public ClaimStatus claimStatus { get; set; }
        public string? Comments { get; set; }
    }
}
