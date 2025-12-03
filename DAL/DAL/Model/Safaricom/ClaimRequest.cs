using DAL.ModelView.Safaricom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DAL.Model.Safaricom
{
    public class ClaimRequest
    {
        public long Id { get; set; }    
        public string? PartnerID { get; set; }
        public virtual PhoneInsuranceCustomers PhoneInsuranceCustomer { get; set; }
        public int PhoneInsuranceCustomerId { get; set; } = 0;
        public string PhoneId { get; set; } 
        public string PartnerCode {  get; set; }    
        public string RequestId { get; set; }
        public string? ProductID { get; set; }
        public string CustomerName { get; set; }
        public string ClaimRefNumber { get; set; }
        public string IDNumber { get; set; }
        public string? IMEINumber1 { get; set; }
        public string? IMEINumber2 { get; set; }
        public ClaimType? ClaimType { get; set; }
        public string? DamagePart { get; set; }
        public double PartCost { get; set; } = 0;
        public int? PartId { get; set; }=0;
        public double? ReplacementCost { get; set; }
        public double LabourCost { get; set; } = 0;
        public string? Narration { get; set; } = "";
        public DateTime IncidentDate { get; set; }
        public DateTime ClaimDate { get; set; }
        public string? Abstract { get; set; }
        public bool Processed { get; set; }
        public bool Dispatched { get; set; }
        public ClaimStatus claimStatus { get; set; } = ClaimStatus.pending;
        public DateTime? CreatedOn { get; set; }
        public string? Comments { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ClaimStatus
    {
        [EnumMember(Value = "pending")] pending, [EnumMember(Value = "approved")] approved, [EnumMember(Value = "declined")] declined, [EnumMember(Value = "replace")] replace
    }
}
