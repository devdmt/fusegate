using DAL.Model;
using DAL.ModelView.LastExpense;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DAL.ModelView.Flex
{
     public class PremiumSettings
    {
        public int Id { get; set; }
        public double PolicyFee { get; set; } = 0;  
        public double CompensationRate { get; set; } = 0;
        public double DiscountMonthly { get; set; } = 0;
        public double DiscountQuartely { get; set; } = 0;
        public double DiscountSemi { get; set; } = 0;
        public double DsicountAnuall { get; set; } = 0;
    }
      public class MainRateRiderResponse : RateResponse
    {
        public List<Benefits>? Benefits { get; set; }
    }

      public class RiderAmount
    {
        public string Rider { get; set; }        
        public double Amount { get; set; }
        public double SumAssured { get; set; }
        public double MaturityBenefit { get; set; } = 0;
    }
      public class LastExpenseCalcDTO
    {
        public LastExpenseCalcDTO()
        {
            //isGroup=false;
        }
        public int OptionId { get; set; }
        public bool hasParent { get; set; } = false;
        public int ChildCount {  get; set; }
        //public bool isGroup { get; set; }

    }

    public class LastExpenseOptions
    {
        public int OptionId { get; set; }
        public string Description { get; set; }
        public string? MainRate { get; set; }
        public string? ExtendedRate { get; set; }
        public bool? HasGroup { get;set; }
    }
    public class RateResponse
    {
        public bool Success { get; set; } = false;
        public bool Processed { get; set; } = false;
        public string? Errormsg { get; set; } = "";
        public double CoverPremium { get; set; } = 0;
        public double PolicyFee { get; set; } = 0;
        public double CompensationLevy { get; set; } = 0;
        public double TotalPremium { get; set; } = 0;
        public double Discount { get; set; } = 0;

    }
    public class LeadsDTO
    {
        public long Id { get; set; } // Identity, DB-generated

        public string? ContributionMode { get; set; }
        public string? CountryCode { get; set; }
        public string? CurrentAge { get; set; }
        public string? DateOfBirth { get; set; }
        public string? DesiredRetirementIncome { get; set; }
        public string? SumAssured { get; set; }
        public string? EmployerContribution { get; set; }
        public string? GrowthRate { get; set; }
        public string? OneTimeContribution { get; set; }
        public string? PersonalContribution { get;  set; }
        public Productenum? Product { get; set; }
        // Required in DB – keep non-nullable in code
        public string PhoneNumber { get; set; } = null!;
        public string FullName { get; set; } = null!;

        public string? PotDepletion { get; set; }
        public string? RetirementAge { get; set; }
        public string? RetirementIncome { get; set; }
        public string? TotalCP { get; set; }
        public string? TotalRetirementPot { get; set; }
        public string? YearItWillLast { get; set; }
        public string OtherDetails { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
   [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RiderType
    {
       [EnumMember(Value = "death")] death=1,
        [EnumMember(Value = "criticalillness")]criticalillness=2,
        [EnumMember(Value = "disability")]disability=3,
        [EnumMember(Value = "waiverretirement")]waiverretirement=4
        //[EnumMember(Value = "waivercriticalillness")]waivercriticalillness=5,
        //[EnumMember(Value = "waiverdisability")]waiverdisability=6
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum BenefitType
    {
       [EnumMember(Value = "NaturalDeath")] NaturalDeath,
        [EnumMember(Value = "AccidentalDeath")]AccidentalDeath,
        [EnumMember(Value = "CriticalIllness")]CriticalIllness,
        [EnumMember(Value = "PermanentTotalDisability")]PermanentTotalDisability,
        [EnumMember(Value = "MaturityBenefit")]MaturityBenefit
        //[EnumMember(Value = "WaiverCriticalIllness")]WaiverCriticalIllness=5,
        //[EnumMember(Value = "WaiverDisability")]WaiverDisability=6
    }
    public class  Benefits
    {
        public string BenefitType { get; set; }
        public double PayImmediately { get; set; }
        public double Paypartial { get; set; }
        public double PaySumAssured { get; set; }
        public double TotalPayout { get; set; }
        public List<PartialPayment>? PartialPayments { get; set; }
    }
    public class PartialPayment
    {
        public string Description { get; set; }
        public double Amount { get; set; }
    }
    public class NaturalDeathBenefits 
    {
        public RiderType Amount { get; set; }
    }
    public class RateSDTO
    {
         
      
        public float Sumassured { get; set; }
        public int Term { get; set; }
        public string DateOfBirth { get; set; }
        //public int MaturityNumber { get; set; }
        public int? DeathBenefits { get;set; }
        ///public List<RiderType>? Riders { get; set; }
       // public int? productId { get; set; }
        public Frequency frequency { get; set; } = Frequency.monthly;
      //  public Productenum  productenum { get; set; }
        public Gender gender { get; set; }
        public string Names { get; set; }
        public string Phonenumber { get; set; }

    }
      [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Frequency
    {
        [EnumMember(Value = "monthly")]
        monthly,
        [EnumMember(Value = "quarterly")]
        quarterly,
        [EnumMember(Value = "halfyearly")]
        halfyearly,
        [EnumMember(Value = "yearly")]
        yearly
    }
}
