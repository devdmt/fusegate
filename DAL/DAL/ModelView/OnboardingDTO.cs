using DAL.Model;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text.Json.Serialization;

namespace DAL.ModelView
{   
        
     public class CreditLifeDTO //: OnboardingDTO
    {
        public string? PartnerCode { get; set; }
        [Required(ErrorMessage = "Customer Name is required.")]
        public string CustomerName { get; set; }
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Date of birth should be in the format of dd/mm/yyyy")]
        public string? DateOfBirth { get; set; }
        [Required(ErrorMessage = "ID Number is required.")]
        [MinLength(6, ErrorMessage = "ID Number must be at least 6 characters long.")]
        [MaxLength(9, ErrorMessage = "ID Number must be a maximum of 9 characters long.")]
        public string IDNumber { get; set; }
        [Required(ErrorMessage = "ID Number is required.")]
        [MinLength(10, ErrorMessage = "ID Number must be at least 10 characters long.")]
        public string PhoneNumber { get; set; }
        public string? Gender { get; set; }
        [Required]
        public double PremiumAmount { get; set; }
        [Required]
        public double SumAssured { get; set; }
        public int Loanterm { get; set; }
         [Required(ErrorMessage = "Loan Reference is required.")]
         public string? RequestId { get; set; }
        public int? RepaymentPeriod { get; set; }
        [Required(ErrorMessage = "Loan Reference is required.")]
        public string LoanReference { get; set; }
        public string? EmailAddress { get; set; }
    }
   public class OnboardingFuneralRequestDTO
    {
        public required string Id { get; set; }
        public required string TransactionId { get; set; }
        public virtual Partners Partner { get; set; }
        public int PartnerId { get; set; }
        public virtual PartnersProducts Product { get; set; }
        public int? ProductId { get; set; }
        public string? CustomerName { get; set; }
        public string? DateOfBirth { get; set; }
        public string? IDNumber { get; set; } //Reg No
        public Gender? Gender { get; set; }
        public double? Premium { get; set; }
        public benefitOption? BenefitOption { get; set; }
        public string? BeneficiaryName { get; set; } //Institution Name
        public string? RegNumber { get; set; } //Registration Number
        public string? BeneficiaryMobileNumber { get; set; }
        public DateTime CreatedOn { get; set; }
        public Boolean Processed { get; set; }
        public string Status { get; set; }
    }

    public class OnboardingDTO  
    {
        public string? PartnerCode { get; set; }
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
        //public IDType ID_Type
        //{
        //    get; set;
        //} = IDType.NationalID;

        [Required(ErrorMessage = "Gender is required. Please add either female or male")]
        public Gender? Gender { get; set; }
      
        [Required(ErrorMessage = "PhoneNumber is required.")]
        //Registration Number
        //[MinLength(9, ErrorMessage = "The Beneficiary Mobile Number must be at least 9 characters long.")]
        //[MaxLength(13, ErrorMessage = "The Beneficiary Mobile Number must be at most 13 characters long.")]
        public string PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Occupation { get; set; }
        public string? Nationality { get; set; }
        public string? Residency { get; set; }
        public string? RequestDate { get; set; }

    }
     

     public enum RegistrationChannel 
    {
        Web,USSD,App,API
    }
    }
