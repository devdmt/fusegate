using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DAL.ModelView.Safaricom
{

  public class PhoneCustomerDTO
    {
        public string? CustomerName { get; set; }
        public string? PhoneNumber { get; set;}
        public string? IdNumber { get; set;}
        public string? dateOfBirth {  get; set;}
        public string? CustomerAddress { get; set; }
        public string? SecondaryContactName { get; set;}   
        public string? SecondaryContact { get; set; }
        public string? CreatedBy {  get; set; } 
    }
    public class PhoneInsuranceRequest
    {
     
        public string? PartnerCode { get; set; }
        public string? ProductID {  get; set; }
        public string? Phonenumber {  get; set; }
        public string? CustomerName {  get; set; }
        public string? SecondaryContactName { get; set; }
        public string? SecondaryContact { get; set; }
        public string? Idnumber {  get; set; }
        public string? dateOfBirth {  get; set; }
        public string? PhoneModel {  get; set; }
        public string? RequestId { get; set; }  
        public List<string>? IMEINumber {  get; set; }
        public string? SerialNumber { get; set; }
        public string? PhoneCost {  get; set; }
        public string? MpesaRef {  get; set; }
        public ModeOfPurchase? ModeOfPurchase {  get; set; }
        public LoanPurchase? LoanPurchase { get; set; } = null;
        public string? PurchaseDate {  get; set; }
        public PolicyStatus? PolicyStatus {  get; set; }

    }
    public class LoanPurchase
    {
        public LoanPurchase() {
            LoanRefNumber = "";
            RepaymentTerms = "";
            LoanAmount = 0;
            InterestRate = 0;
            PremiumPaid = 0;

        }
      public string? LoanRefNumber { get; set; }
    public string? RepaymentTerms { get; set; }
    public double? LoanAmount { get; set; } = 0;
    public double? InterestRate { get; set; } = 0;
    public double? PremiumPaid { get; set; } = 0;
   }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public  enum ModeOfPurchase
    {
        [EnumMember(Value = "cash")] cash, [EnumMember(Value = "credit")] credit
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PolicyStatus
    {
        [EnumMember(Value = "pending")] pending, 
        [EnumMember(Value = "active")] active, 
        [EnumMember(Value = "expired")] expired
    }
}
