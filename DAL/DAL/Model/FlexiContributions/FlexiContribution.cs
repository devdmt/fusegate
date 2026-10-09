using System.ComponentModel.DataAnnotations;
using DAL.Model;
using DAL.ModelView.FlexiFuturePlus;

namespace DAL.Model.FlexiContributions;

public class FlexiContribution
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CustomerId { get; set; }
    public double Total_Contribution { get; set; }

    [StringLength(1000)]
    public string? Naration { get; set; }

    [StringLength(1000)]
    public string? Reference { get; set; }

    public Guid? FlexiFuturePolicyId { get; set; }
    public Guid? FlexiFundId { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string? Bank { get; set; }
    public string? BankReference { get; set; }
    public string? Branch { get; set; }
    public bool PaymentAcknowledged { get; set; }
    public PaymentStatus? PaymentStatus { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedDate { get; set; }
    public string? RejectedReason { get; set; }
    public string? FullName { get; set; }
    public string? Idnumber { get; set; }

    [StringLength(100)]
    public string? BatchReference { get; set; }

    public bool? Approved { get; set; }
    public PaymentMode? PaymentMode { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public DateTimeOffset? ApprovedOn { get; set; }

    [StringLength(100)]
    public string? ThirdpartyRef { get; set; }

    public DateTimeOffset? Created { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
    public string? DeletedBy { get; set; }

    [StringLength(100)]
    public string? CreatedFromIP { get; set; }

    [StringLength(1000)]
    public string? CreatedFromBrowser { get; set; }
}
