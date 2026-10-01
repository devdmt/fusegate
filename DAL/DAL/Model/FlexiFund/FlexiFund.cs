using System.ComponentModel.DataAnnotations;

namespace DAL.Model.FlexiFund;

public class FlexiFund
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(40)]
    public string PolicyId { get; set; } = string.Empty;

    [Required]
    [StringLength(32)]
    public string Status { get; set; } = string.Empty;

    [Required]
    [StringLength(32)]
    public string CalculationMode { get; set; } = string.Empty;

    public DateOnly IssueDate { get; set; }
    public int PolicyTerm { get; set; }

    [Required]
    [StringLength(32)]
    public string Frequency { get; set; } = string.Empty;

    public decimal DeathBenefitPct { get; set; }
    public int MaturityBenefitPayments { get; set; }
    public decimal? TargetPremium { get; set; }
    public decimal? TargetSumAssured { get; set; }
    public decimal SumAssured { get; set; }
    public decimal MonthlyEquivalent { get; set; }
    public decimal SavingsPremium { get; set; }
    public decimal RidersTotal { get; set; }
    public decimal Phcl { get; set; }
    public decimal TotalPremiumPayable { get; set; }
    public decimal TotalFunds { get; set; }
    public bool RequiresApproval { get; set; }
    public DateOnly MaturityDate { get; set; }

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
