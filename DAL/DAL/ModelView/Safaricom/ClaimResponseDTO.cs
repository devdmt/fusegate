using DAL.Model.Safaricom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DAL.ModelView.Safaricom
{
    public class ClaimResponseDetails
    {
        public PhoneCustomerDTO phoneCustomer { get; set; }
        public List<ClaimsDetailsDTO> claimsDetails { get; set; }
    }
    public class ClaimsDetailsListDTO
    {
        public string? IDNumber { get; set; }
        public string? IMEINO { get; set; }
        public string ClaimType { get; set; }
        public string? DamagePart { get; set; }
        public double? ReplacementCost { get; set; }
        public string IncidentDate { get; set; }
        public string CustomerId { get; set; }
        public string ClaimDate { get; set; }
        public string CustomerPhone { get; set; }
        // public string ClaimDate { get; set; }
        //public FileDetails? imagePhoneUpload { get; set; }
        //public FileDetails? imageIMEIUpload { get; set; }
        //public FileDetails? medicalReportUpload { get; set; }
        //public FileDetails? policeAbstractUpload { get; set; }
    }
    public class ClaimsDetailsDTO
    {
        public string? IDNumber { get; set; }
        public List<string>? IMEINO { get; set; }
        public ClaimType ClaimType { get; set; }
        public string? DamagePart { get; set; }
        public string? RequestId { get; set; }
        public double? ReplacementCost { get; set; }
        public string IncidentDate { get; set; }
        public string CustomerId { get; set; }
        public PolicyStatus policyStatus { get; set; }
        public ClaimStatus claimStatus { get; set; }
        public string? imagePhoneUpload { get; set; }
        public string? imageIMEIUpload { get; set; }
        public string? medicalReportUpload { get; set; }
        public string? policeAbstractUpload { get; set; }
    }

    //[JsonConverter(typeof(JsonStringEnumConverter))]
    //public enum PolicyStatus
    //{
    //    [EnumMember(Value = "pending")] pending,
    //    [EnumMember(Value = "active")] active,
    //    [EnumMember(Value = "expired")] expired
    //}

    //[JsonConverter(typeof(JsonStringEnumConverter))]
    //public enum ClaimStatus
    //{
    //    [EnumMember(Value = "pending")] pending, [EnumMember(Value = "closed")] closed, [EnumMember(Value = "declined")] declined
    //}
    //public class PhoneCustomerDTO
    //{
    //    public string? CustomerName { get; set; }
    //    public string? PhoneNumber { get; set; }
    //    public string? IdNumber { get; set; }
    //    // public string? CustomerAddress { get; set; }
    //    public string? Nextofkinname { get; set; }
    //    public string? NextofkinId { get; set; }
    //    //  public string? CreatedBy { get; set; }
    //}
}
