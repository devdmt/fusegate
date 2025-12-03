using DAL.ModelView.Safaricom;
using System.ComponentModel.DataAnnotations;
namespace DAL.Model.Safaricom
{
    public class PhoneInsuranceRequest
    {

        public string Id { get; set; }
        [MaxLength(100)]
        public string? PartnerID { get; set; }
        [MaxLength(100)]
        public string? ProductID { get; set; }
        public virtual PhoneInsuranceCustomers PhoneInsuranceCustomer { get; set; }
        public int PhoneInsuranceCustomerId { get; set; }
       
        [MaxLength(200)]
        public string? PhoneModel { get; set; }
        [MaxLength(200)]
        public string? PhoneName { get; set; }
        [MaxLength(200)]
        public string? RequestId { get; set; }
        [MaxLength(200)]
        public string? IMEINumber { get; set; }
        public string? IMEINumber1 { get; set; }
        public string? IMEINumber2 { get; set; }
        public double? PhoneCost { get; set; }
        public ModeOfPurchase? ModeOfPurchase { get; set; }
        [MaxLength(200)]
        public string? LoanRefNumber { get; set; }
        [MaxLength(200)]
        public string? RepaymentTerms { get; set; }
        public double? LoanAmount { get; set; } = 0;
        public double? InterestRate { get; set; } = 0;
        public double? PremiumPaid { get; set; } = 0;
        [MaxLength(200)]
        public string? PurchaseDate { get; set; }
        public bool? Processed { get; set; }
        public bool? Active { get; set; }
        public DateTime? RequestedOn { get; set; }
        public PolicyStatus? PolicyStatus { get; set; }


    }
      public enum NotificationType
    {
        OTP,DispatchedDamage,DispatchedTheft,PhoneReady
    }
    public class Notifications
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public NotificationType notificationType { get; set; }
    }
    public class PhoneInsuranceCustomers
    {
        public int Id { get; set; }
        [MaxLength(200)]
        public string? CustomerName { get; set; }
        [MaxLength(200)]
        public string PhoneNumber { get; set; }
        [MaxLength(200)]
        public string IdNumber { get; set; }
        [MaxLength(200)]
        public string? CustomerAddress { get; set; }
        public string? Nextofkinname { get; set; }
        public string? NextofkinId { get; set; }
    }
}
