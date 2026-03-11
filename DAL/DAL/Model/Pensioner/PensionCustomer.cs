using DAL.Common.Contract;
using DAL.Models.Interfaces;
using DAL.ModelView;
using DAL.ModelView.Pension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DAL.Model.Pensioner
{
    public class PensionCustomer
    {
        
      
      public Guid Id { get; set; }
            [Required]
        [StringLength(500)]
        public string Fullname { get; set; }
          
        [StringLength(50)]
        public string? Firstname { get; set; }
        [StringLength(50)]
        public string? Lastname { get; set; }
         public string? Surname { get; set; }
        public CustomerType CustomerType { get; set; }
        [StringLength(100)]

        public string? DateOfBirth { get; set; }
      
        public string? UserId { get; set; }
       
        public Guid? GroupId { get; set; }
       

        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }
        [Required]
        [StringLength(15)]
        public string PhoneNumber { get; set; }
         [Required]
        [StringLength(15)]
        public string CountryCode { get; set; }

        [StringLength(15)]
        public string? KRAPin { get; set; }
          public bool? Confirmed { get; set; } 
        public DateTimeOffset? ConfirmedOn { get; set; }
        public string? ConfirmedBy { get; set; }
        // Address Information
        [StringLength(200)]
        public string? AddressLine1 { get; set; }

        [StringLength(200)]
        public string? AddressLine2 { get; set; }

        [StringLength(100)]
        public string? City { get; set; }
        public string? Citizenship { get; set; }

        [StringLength(100)]
        public string? State { get; set; }
        [StringLength(100)]
        public string? County { get; set; }

        [StringLength(20)]
        public string? PostalCode { get; set; }

        [StringLength(100)]
        public string? Country { get; set; } 
        [StringLength(100)]
        public string? Residency { get; set; } 
        [StringLength(500)]
        public string? Comments { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.pending!;
      
         public EmployementStatus? EmployementStatus { get; set; }
        [StringLength(100)]
        public string? Occupation { get; set; }
        public Gender Gender { get; set; }
        public string? Idnumber { get; set; }
      
        public string? NSSFCode { get; set; }
        // Additional Information

        [StringLength(20)]
        public string? MaritalStatus { get; set; } 
  
        public string? Signature { get; set; }
                [StringLength(20)]
        public string? MemberNumber { get; set; }
                [StringLength(20)]
        public bool? RegistrationComplete { get; set; }
        public bool? Pensionerstatus { get; set; }
        //public PaymentMode PaymentMode { get; set; }
        public bool? Approved { get; set; }
        [MaxLength(50)]
        public string? MemberJoinDate { get; set; }
         [MaxLength(50)]
       public string? StaffNumber { get; set; }
        public Validate_IPRS_Status? Validate_IPRS_Status { get; set; } 
        public string? IPRS_FullNames { get; set; }
        [StringLength(20)]
        public string? RefferalCode { get; set; }

    }

           [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EmployementStatus
    {
        [EnumMember(Value = "Employed")] Employed,
        [EnumMember(Value = "Business")] Business,

    }
     [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Gender
    {
        [EnumMember(Value = "Male")] Male,
        [EnumMember(Value = "Female")] Female   

    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Validate_IPRS_Status
    {
        [EnumMember(Value = "Pending")] Pending,
        [EnumMember(Value = "Validated")] Validated,
        [EnumMember(Value = "Rejected")] Rejected
    }
    
    // [JsonConverter(typeof(JsonStringEnumConverter))]
    //public enum CustomerType
    //{
    //   [EnumMember(Value = "Individual")]  Individual,
    //   [EnumMember(Value = "Group")]  Group,
    //   //[EnumMember(Value = "Partners")]  Partners
    //}
    [JsonConverter(typeof(JsonStringEnumConverter))]
     public enum CustomerType
    {
       [EnumMember(Value = "Individual")]  Individual,
       [EnumMember(Value = "Group")]  Group,
      [EnumMember(Value = "Partners")]  Partners,
      [EnumMember(Value = "Agent")]  Agent
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ContributionType
    {
       [EnumMember(Value = "individual")]  individual,
       [EnumMember(Value = "umbrella")]  umbrella
    }

     [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ApprovalStatus
    {
       [EnumMember(Value = "pending")] pending, [EnumMember(Value = "approved")] approved, [EnumMember(Value = "declined")] declined
    }
    }

