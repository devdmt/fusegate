using DAL.ModelView;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Model
{
   
    public class Customers
    {
    public string Id { get; set; }
  //  public required string TransactionId { get; set; }
    public  string PartnerId { get; set; }      
    public int? ProductId { get; set; }    
    public string? Firstname { get; set; }
    public string? OtherNames { get; set; }
    public string? Fullname { get; set; } // Optional derived field
    public string? DateOfBirth { get; set; }
    public string? IDNumber { get; set; }
    public IDType? IdType { get; set; }
    public Gender? Gender { get; set; }
    public string? MemberNo { get; set; } = string.Empty;
    // Contact
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    // Demographics
    public string? Nationality { get; set; }
    public string? Residency { get; set; }
    public string? Occupation { get; set; }
    public string? EmploymentTerms { get; set; }

    // Tax & Compliance
    public string? TaxIdNumber { get; set; }
    public string? Pin { get; set; }
    public string? USAddress { get; set; }

    // Business / Employer Info
    public string? EmployerName { get; set; }
    public string? BusinessName { get; set; }
    public string? NatureOfBusiness { get; set; }
    public string? RoleInBusiness { get; set; }
    public string? Role { get; set; }
    // Income
    public string? SourceOfIncome { get; set; }
    public string? AdditionalSourceOfIncome { get; set; }
    public double? AverageIncome { get; set; }
    // Workflow / Process State
    public int? Stage { get; set; }
    public int? Step { get; set; }
    public bool? Complete { get; set; }
    public bool? Processed { get; set; }
    public string? Status { get; set; }

    // Validation & Retry
    public bool? Validated { get; set; }
    public int? ValidationErrorCount { get; set; }
    public int? RetryCount { get; set; }
    public string? ValidationErrors { get; set; }
    public DateTime? ValidationDate { get; set; }

    // Group / Membership
    public bool? IsGroupMember { get; set; }
    public bool? MemberNumber { get; set; }
    // Documents & Signatures
    public string? Signature { get; set; }
    public string? SignaturePath { get; set; }
    public string? PolicyPath { get; set; }
    // Communication
    public bool? MailSent { get; set; }
    public DateTime? MailSentOn { get; set; }

    // Audit & Lifecycle
    public bool? RecordProcessed { get; set; }
    public bool? Modified { get; set; }
    public DateTime? LastModified { get; set; }
    public DateTime? CreatedOn { get; set; }     // maps to DateCreated
    public string? RequestDate { get; set; }

    }

    public class CustomerProduct
{
    
    public long Id { get; set; }

    public string CustomerId { get; set; }

    public int Product { get; set; }

    [MaxLength(50)]
    public string? RefNo { get; set; }

    public bool Complete { get; set; }

    // DB default: 0
    public bool? Processed { get; set; } = false;

    // DB default: 0
    public bool? IsPicked { get; set; } = false;

    [Column("MailSentOn")]
    public DateTime? MailSentOn { get; set; }

    // DB default: getdate()
    public DateTime? RequestCreatedOn { get; set; }

    public int? AgentId { get; set; }

    public bool? PaymentComplete { get; set; }

    public DateTime? PaymentCompletedOn { get; set; } // maps datetime2(7)

    public int? RequestSource { get; set; }

    [MaxLength(1000)]
    public string? Filelocation { get; set; }

    public string? Filebytes { get; set; }

    public int? GroupId { get; set; }

    public bool? Activated { get; set; }
public bool? Validated { get; set; }
    public int? ValidationErrorCount { get; set; }

    public int? RetryCount { get; set; }

    public string? ValidationErrors { get; set; }

    public DateTime? ValidationDate { get; set; }

    public DateTime? NextRetryPeriod { get; set; }

    /// <summary>Activation mode: 1 = Signature, 2 = OTP (SMS).</summary>
    public int? ActivationMode { get; set; }
}
}