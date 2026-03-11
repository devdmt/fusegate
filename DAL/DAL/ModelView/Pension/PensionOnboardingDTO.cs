using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DAL.ModelView.Pension
{

    /// <summary>
    /// Request DTO for initiating a balance request. Used by BalanceRequest.
    /// Config: Otp:Length (default 6), Otp:ExpiryMinutes (default 5).
    /// </summary>
    
     public class BalanceDTORequest
    {
        [Required(ErrorMessage = "Member number is required")]
        public string MemberNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Partner code is required")]
        public string PartnerCode { get; set; } = string.Empty;

        /// <summary>Optional agent code for audit; defaults to empty if not provided.</summary>
        public string? AgentCode { get; set; }

        public ProductTypes ProductType { get; set; }
    }
      public class ViewBalanceDTORequest
    {
           [Required(ErrorMessage ="OTP required")]
          public string OTPCode { get; set; }
        [Required(ErrorMessage ="Unique request Id required")]
          public string UniqueRequestId { get; set; } 


    }

    public class contributeDTO
    {
      
        public string MemberNo { get; set; }
        public ProductTypes ProductType { get; set; }
        public double? Amount { get; set; }
        //public PaymentMode paymentMode { get; set; } = PaymentMode.Mpesa;  
        public string? phoneNumber { get; set; }
        public string? PaymentReference { get; set; }
        public string callbackUrl { get; set; }

    }

    public class WithdrawalDTO
    {
        public string PartnerCode { get; set; }
        public string CustomerId { get; set; }
        public double? Amount { get; set; }
         

    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PaymentMode
    {
        [EnumMember(Value = "Mpesa")] Mpesa, [EnumMember(Value = "Bank")] Bank
    }
        public class BalanceDTO:ResponseDTO
    {
      //  public string Savings { get; set; }
        public List<ProductTypes> products { get; set; }
        public string Interest { get; set; }
        public string TotalBalance { get; set; }  
        public string MonthlyReturn { get; set; }   
       // public string Bonus { get; set; }   
    }
    public class BeneficiaryResponseDTO : ResponseDTO
    {
        public string BeneficiaryNo { get; set; }
    }
    public class PensionBeneficiaryDTO
    {
       // public List<ProductTypes> productType { get; set; } 
        public string memberNo { get; set; } 
        public List<BeneficiaryDetailsDTO> beneficiaryDetails { get; set; }

    }
  
    //public class TransferDTO
    //{
    //    public string MemberNo { get; set; }
    //    public string? EmployerName { get; set; }    
    //    public string? HrEmail { get; set; }
    //    public string? PensionProvider { get; set; }
    //    public string YearOfEmployment { get; set; }
    //    public string? AdditionalInformation { get; set; }
    //    public string callbackUrl { get; set; }
    //}
     public class ActivatePensionDTO
    {
        public string CustomerId { get; set; }
        public List<ProductTypes> ProductType { get; set; }
    }
    public class ContactResponseDTO
    {
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
    }
    public class ContactUsRequest
    {
        [Required(ErrorMessage = "Name is required.")]
        public string CustomerId { get; set; }
        [Required(ErrorMessage = "Phonenumber is required.")]
       // [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Phonenumber { get; set; }
        [Required(ErrorMessage = "Message is required.")]
        public string Message { get; set; }
    }
        public class BeneficiaryDetailsDTO
    {
        public string? BeneficiaryCode { get; set; }
        [Required(ErrorMessage = "Firstname is required.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Othernames required.")]
        public string OtherNames { get; set; }
        public string? IDNumber { get; set; }
        public string? Relationship { get; set; }
        [Required(ErrorMessage = "Date Of Birth is required.")]
        public string Date_Of_Birth { get; set; }
        public string? phoneNumber { get; set; }
        public double Percentage { get; set; }
       public GuardianDetailsDTO? guardianDetailsDTO { get; set; }
    }
     public class GuardianDetailsDTO
    {
        [Required(ErrorMessage = "Firstname is required.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Othernames required.")]
        public string OtherNames { get; set; }
           [Required(ErrorMessage = "Othernames required.")]
        public string IDNumber { get; set; }
            [Required(ErrorMessage = "Othernames required.")]
        public string Date_Of_Birth { get; set; }
        public string? Relationship { get; set; }
      public string? GuardianCode { get; set; } 
       // [Required(ErrorMessage = "PhoneNumber is required.")]
        //public string phoneNumber { get; set; }
    
    }
    public class PensionQuoteResponse
    {
        public string DateOfBirth { get; set; }
        public int CurrentAge { get; set; }
        public string RetirementAge { get; set; }
        public double TotalPot { get; set; }
        public double DesiredRetirementIncome { get; set; }
        public int Age { get; set; }
        public bool Success { get; set; }
        public string Error { get; set; }

    }
    public class PensionCalculatorDTO
    {
        [Required]
        public string Phonenumber { get; set; }
        [Required]
        public string DateOfBirth { get; set; }
        public interestGrowthRate InterestGrowthRate { get; set; } = interestGrowthRate.guaranteed;
        public ContributionFrequency frequency { get; set; }
        public double StartingContribution { get; set; } = 0;
        public double MonthlyContribution { get; set; } =0;
        public int RetireAge { get; set; } = 1;
        public double RetireIncome { get; set; } = 0;

    }
public enum interestGrowthRate
    {
        guaranteed,moderate,aggressive
    }
    public enum ContributionFrequency
    {
       daily, weekly, monthly,once
    }
        public class PensionOnboardingDTO 
    {
        
        [Required(ErrorMessage = "Customer Name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Customer Name is required.")]
        public string OtherNames { get; set; }
        //Format dd/mm/yyyy

        [StringLength(20, MinimumLength = 8, ErrorMessage = "Date of birth should be in the format of dd/mm/yyyy")]
        public string? DateOfBirth { get; set; }
        [Required(ErrorMessage = "ID Number is required.")]
        [MinLength(6, ErrorMessage = "ID Number must be at least 6 characters long.")]
        [MaxLength(9, ErrorMessage = "ID Number must be a maximum of 9 characters long.")]
        public string IDNumber { get; set; } //Reg No
     

        [Required(ErrorMessage = "Gender is required. Please add either female or male")]
        public Gender Gender { get; set; }
      
        [Required(ErrorMessage = "PhoneNumber is required.")]
     
        public string PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Occupation { get; set; }
        public string? Nationality { get; set; }
        public string? Residency { get; set; }
        public string? RequestDate { get; set; }
        public string? Agent_code { get; set; }
         public List<ProductTypes> Product_Type
        {
            get; set;
        }
      public string callbackurl { get; set; }
        
        
    }

    public class OnboardResponse: ResponseDTO
    {
        public string MemberNo{ get; set; }
    }
    public class GroupDTO
    {
        public string Org_name { get; set; }
        public string Org_Email { get; set; } 
        public string Org_Phone{ get; set; } 
        public string Contact_Name { get; set; }
        public string Contact_Email { get; set; }
        public string Contact_Phone { get; set; }
    }

     [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CustomerType
    {
       [EnumMember(Value = "Individual")]  Individual,
       [EnumMember(Value = "Group")]  Group
    }
      [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum     ProductTypes
    {
       [EnumMember(Value = "IPP")] IPP,
      // [EnumMember(Value = "Umbrella")] Umbrella,
       [EnumMember(Value = "PRMF")] PRMF,
       [EnumMember(Value = "NSSF")] NSSF,
    }

}
