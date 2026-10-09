using System.ComponentModel.DataAnnotations;
using DAL.Model;
using DAL.ModelView.FlexiFuturePlus;

namespace DAL.Model.FlexiContributions;

public enum PaymentInstructionStatus
{
    Initiated = 0,
    Submitted = 1,
    Approved = 2
}

public class PaymentInstruction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CustomerId { get; set; }
    public double Total_Contribution { get; set; }
    public PaymentInstructionStatus ProcessingStatus { get; set; } = PaymentInstructionStatus.Initiated;

    [StringLength(1000)]
    public string? Naration { get; set; }

    public Guid? FlexiFuturePolicyId { get; set; }
    public Guid? FlexiFundId { get; set; }
    public string? UploadDocumentsPath { get; set; }
    public string? UploadDocumentsName { get; set; }
    public string? UploadDocumentsBase64 { get; set; }
    public bool PaymentAcknowledged { get; set; }
    public PaymentStatus? PaymentStatus { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedDate { get; set; }
    public string? RejectedReason { get; set; }
    public PaymentMode? FlexiContributionType { get; set; }
    public DateTimeOffset? ApprovedOn { get; set; }

    [StringLength(50)]
    public string? ApprovedBy { get; set; }

    [StringLength(50)]
    public string? ApprovedIP { get; set; }

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
