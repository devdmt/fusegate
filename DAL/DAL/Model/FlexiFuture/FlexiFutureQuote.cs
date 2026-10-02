using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Model.FlexiFuture;

public class FlexiFutureQuote
{
    public Guid Id { get; set; }
    public string QuoteNumber { get; set; } = string.Empty;
    public string Status { get; set; } = "Quoted";
    public string CalculationMode { get; set; } = string.Empty;
    public DateOnly IssueDate { get; set; }
    public int PolicyTerm { get; set; }
    public string Frequency { get; set; } = string.Empty;
    public decimal DeathBenefitPct { get; set; }
    public int MaturityBenefitPayments { get; set; }
    public decimal? TargetPremium { get; set; }
    public decimal? TargetSumAssured { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public int Anb { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? IdNumber { get; set; }

    [Column("ReferralId")]
    public string? RefferalCode { get; set; }

    public int Source { get; set; } = 3;
    public string? PartnerCode { get; set; }
    public decimal SumAssured { get; set; }
    public decimal MonthlyEquivalent { get; set; }
    public decimal SavingsPremium { get; set; }
    public decimal RidersTotal { get; set; }
    public decimal Phcl { get; set; }
    public decimal TotalPremiumPayable { get; set; }
    public bool RequiresApproval { get; set; }
    public DateOnly MaturityDate { get; set; }
    public string? RiderBreakdownJson { get; set; }
    public string? MaturityScheduleJson { get; set; }
    public string? SurrenderScheduleJson { get; set; }
    public string? TaxReliefJson { get; set; }
    public string? SpouseResultsJson { get; set; }
    public string? ChildrenResultsJson { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedOn { get; set; }
    public int RetryCount { get; set; }
    public DateTime? LastRetryDate { get; set; }
    public string? IpAddress { get; set; }
    public string? Browser { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    public ICollection<FlexiFutureQuoteSpouse> Spouses { get; set; } = new List<FlexiFutureQuoteSpouse>();
    public ICollection<FlexiFutureQuoteChild> Children { get; set; } = new List<FlexiFutureQuoteChild>();
    public ICollection<FlexiFutureQuoteRider> Riders { get; set; } = new List<FlexiFutureQuoteRider>();

    // Share rows live in the shared, polymorphic DAL.Models.Shared.QuotesShare table and
    // so cannot carry a navigation back to one specific quote type. Query it directly.
}

public class FlexiFutureQuoteSpouse
{
    public Guid Id { get; set; }
    public Guid QuoteId { get; set; }
    public int SpouseIndex { get; set; }
    public string? Name { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public int? Anb { get; set; }
    public decimal SumAssured { get; set; }
    public decimal PremiumTotal { get; set; }
    public FlexiFutureQuote? Quote { get; set; }
}

public class FlexiFutureQuoteChild
{
    public Guid Id { get; set; }
    public Guid QuoteId { get; set; }
    public string? Name { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public int? Anb { get; set; }
    public decimal SumAssured { get; set; }
    public decimal Premium { get; set; }
    public FlexiFutureQuote? Quote { get; set; }
}

public class FlexiFutureQuoteRider
{
    public Guid Id { get; set; }
    public Guid QuoteId { get; set; }
    public string LifeRole { get; set; } = FlexiFutureLifeRoles.Main;
    public string RiderCode { get; set; } = string.Empty;
    public bool Selected { get; set; }
    public decimal Premium { get; set; }
    public FlexiFutureQuote? Quote { get; set; }
}
