using DAL.Common.Contract;
using DAL.Models.Interfaces;
using DAL.ModelView.Pension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class Contributions
    {
   
       public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? GroupId { get; set; }
        public Guid? PensionFundId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
         public string? Idnumber { get; set; }
        public string? Naration { get; set; }
        public string? FullName { get; set; }
        public double? EE_Opening_Balance_Registered { get; set; } = default(double);
        public double? EE_Opening_Balance_UnRegistered { get; set; } = default(double);
        public string? MonthName { get; set; }
        public double? Ee_Contribution { get; set; } = 0;
        public double? Er_Contribution { get; set; } = 0;
          public double? EVC_Contribution { get; set; } = 0;
        public double Ee_UnRegistered { get; set; } = 0;
        public double? Er_UnRegistered { get; set; } = 0;
        public double Ee_Registered { get; set; } = 0;
        public double Er_Registered { get; set; } = 0;
        public double? Ee_CorpTax { get; set; } = 0;
        public double? Er_CorpTax { get; set; } = 0;
        public double Total_Contribution { get; set; } = 0;
        public string? Reference { get; set; }
        public string? TaxFeeDeferred { get; set; }
        public string? BatchReference { get; set; } = string.Empty;
        public bool PaymentAcknowledged { get; set; } = false;
        public PaymentStatus? PaymentStatus { get; set; }
        public PaymentMode? PaymentMode { get; set; }
        public CustomerType? AccountType { get; set; }
        public ContributionType contributionType { get; set; } = ContributionType.individual;
        public string? Bank
        { get; set; }
        public string? Branch { get; set; }
        public string? BankReference { get; set; }
        public string? ApprovedBy { get; set; }
        public bool? Approved { get; set; } = false;
        public DateTime? ApprovedDate { get; set; }
        public string? RejectedBy { get; set; }
        public DateTime? RejectedDate { get; set; }
        public string? RejectedReason { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;

    }
           [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ContributionType
    {
       [EnumMember(Value = "individual")]  individual,
       [EnumMember(Value = "umbrella")]  umbrella
    }
    public enum PaymentStatus
    {
        Pending,
        Approved,
        Rejected
    }
    
     public class ContributionSettings
    {
        public  int Id { get; set; }
        public double MaximumRegistered { get; set; } = default(double);
        public double MaximumEeContribution { get; set; } = default(double);
        public double MaximumErContribution { get; set; } = default(double);
        public int Year { get; set; } = default(int);
        public bool Active { get; set; } = default(bool);

    }
}
