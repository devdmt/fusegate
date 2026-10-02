using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DAL.ModelView.FlexiFuture;

public class FlexiFutureClientDto
{
    [Required]
    public string ClientName { get; set; } = string.Empty;

    [Required]
    public DateOnly DateOfBirth { get; set; }

    [Required]
    public string Gender { get; set; } = string.Empty;

    [Required]
    public string PhoneNumber { get; set; } = string.Empty;

    public string? Email { get; set; }
    public string? IdNumber { get; set; }
}

public class FlexiFutureSpouseDto
{
    public int SpouseIndex { get; set; } = 1;
    public string? Name { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public List<string> SelectedRiders { get; set; } = new();
}

public class FlexiFutureChildDto
{
    public string? Name { get; set; }
    public DateOnly? DateOfBirth { get; set; }
}

public class FlexiFutureComputeRequestDto
{
    [Required]
    public string CalculationMode { get; set; } = "PremiumToSa";

    [Required]
    public DateOnly IssueDate { get; set; }

    [Required]
    public int PolicyTerm { get; set; }

    [Required]
    public string Frequency { get; set; } = "Monthly";

    public decimal DeathBenefitPct { get; set; } = 0.5m;
    public int MaturityBenefitPayments { get; set; } = 1;
    public decimal? TargetPremium { get; set; }
    public decimal? TargetSumAssured { get; set; }

    [Required]
    public FlexiFutureClientDto Client { get; set; } = new();

    public List<string> SelectedRiders { get; set; } = new();
    public List<FlexiFutureSpouseDto> Spouses { get; set; } = new();
    public List<FlexiFutureChildDto> Children { get; set; } = new();
}

public class FlexiFutureQuoteUpsertDto : FlexiFutureComputeRequestDto
{
    public Guid? QuoteId { get; set; }

    [JsonPropertyName("refferalCode")]
    public string? RefferalCode { get; set; }

    [Obsolete("Use RefferalCode")]
    public string? ReferralId { get => RefferalCode; set => RefferalCode = value; }

    [JsonIgnore]
    public string? IpAddress { get; set; }

    [JsonIgnore]
    public string? Browser { get; set; }
}

public class FlexiFutureQuoteSearchDto
{
    public Guid? QuoteId { get; set; }
    public string? QuoteNumber { get; set; }
    public string? IdNumber { get; set; }
    public string? PhoneNumber { get; set; }
}

public class FlexiFutureShareQuoteDto
{
    [Required]
    public Guid QuoteId { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [JsonIgnore]
    public string? IpAddress { get; set; }

    [JsonIgnore]
    public string? Browser { get; set; }
}

public class FlexiFutureCheckRegistrationStatusDto
{
    [Required]
    public Guid QuoteId { get; set; }

    [Required]
    public string IdNumber { get; set; } = string.Empty;
}

public class FlexiFutureValidateOtpDto
{
    [Required]
    public Guid QuoteId { get; set; }

    [Required]
    [StringLength(32)]
    public string Otp { get; set; } = string.Empty;
}

public class FlexiFutureDiscardQuoteDto
{
    [Required]
    public Guid QuoteId { get; set; }
}

public class FlexiFuturePremiumLineDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string LifeRole { get; set; } = "Main";
    public bool Selected { get; set; }
    public decimal Premium { get; set; }
}

public class FlexiFutureMaturityPaymentDto
{
    public int PaymentYear { get; set; }
    public DateOnly PaymentDate { get; set; }
    public decimal Amount { get; set; }
}

public class FlexiFutureSurrenderYearDto
{
    public int Year { get; set; }
    public decimal SurrenderValue { get; set; }
}

public class FlexiFutureTaxReliefDto
{
    public decimal GrossAnnualisedContribution { get; set; }
    public decimal TaxReliefAmount { get; set; }
    public decimal PremiumAfterTaxRelief { get; set; }
    public bool Eligible { get; set; }
}

public class FlexiFutureSpouseResultDto
{
    public int SpouseIndex { get; set; }
    public string? Name { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public int? Anb { get; set; }
    public decimal SumAssured { get; set; }
    public decimal PremiumTotal { get; set; }
    public List<FlexiFuturePremiumLineDto> Riders { get; set; } = new();
}

public class FlexiFutureChildResultDto
{
    public string? Name { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public int? Anb { get; set; }
    public decimal SumAssured { get; set; }
    public decimal Premium { get; set; }
}

public class FlexiFutureComputeResultDto
{
    public int Anb { get; set; }
    public decimal SumAssured { get; set; }
    public decimal MonthlyEquivalent { get; set; }
    public decimal SavingsPremium { get; set; }
    public decimal RidersTotal { get; set; }
    public decimal Phcl { get; set; }
    public decimal TotalPremiumPayable { get; set; }
    public bool RequiresApproval { get; set; }
    public DateOnly MaturityDate { get; set; }
    public string? Warning { get; set; }
    public List<FlexiFuturePremiumLineDto> RiderBreakdown { get; set; } = new();
    /// <summary>
    /// Per-rider totals across main + spouses (+ CHILD_LE for children). <see cref="FlexiFuturePremiumLineDto.LifeRole"/> is always "All".
    /// </summary>
    public List<FlexiFuturePremiumLineDto> CumulativeRiderBreakdown { get; set; } = new();
    public List<FlexiFutureMaturityPaymentDto> MaturitySchedule { get; set; } = new();
    public List<FlexiFutureSurrenderYearDto> SurrenderSchedule { get; set; } = new();
    public FlexiFutureTaxReliefDto? TaxRelief { get; set; }
    public List<FlexiFutureSpouseResultDto> Spouses { get; set; } = new();
    public List<FlexiFutureChildResultDto> Children { get; set; } = new();
}

/// <summary>
/// Slim client for cover-premiums (pricing only — no name/phone/id).
/// </summary>
public class FlexiFutureCoverPremiumsClientDto
{
    [Required]
    public DateOnly DateOfBirth { get; set; }

    [Required]
    public string Gender { get; set; } = string.Empty;
}

/// <summary>
/// Slim spouse for cover-premiums breakdown.
/// </summary>
public class FlexiFutureCoverPremiumsSpouseDto
{
    public int SpouseIndex { get; set; } = 1;
    public DateOnly? DateOfBirth { get; set; }
    public string? Gender { get; set; }

    /// <summary>Covers already switched on for this spouse; menu prices are marginal on top of these.</summary>
    public List<string> SelectedRiders { get; set; } = new();
}

/// <summary>
/// Menu prices for covers before rider selection. Only pricing inputs are required.
/// </summary>
public class FlexiFutureCoverPremiumsRequestDto
{
    [Required]
    public string CalculationMode { get; set; } = "PremiumToSa";

    [Required]
    public DateOnly IssueDate { get; set; }

    [Required]
    public int PolicyTerm { get; set; }

    [Required]
    public string Frequency { get; set; } = "Monthly";

    public decimal DeathBenefitPct { get; set; } = 0.5m;
    public int MaturityBenefitPayments { get; set; } = 1;
    public decimal? TargetPremium { get; set; }
    public decimal? TargetSumAssured { get; set; }

    [Required]
    public FlexiFutureCoverPremiumsClientDto Client { get; set; } = new();

    /// <summary>
    /// Covers already switched on for the main life. For PremiumToSa the sum assured is solved with
    /// these included, and each menu price is the marginal cost of adding that cover on top.
    /// </summary>
    public List<string> SelectedRiders { get; set; } = new();

    public List<FlexiFutureCoverPremiumsSpouseDto> Spouses { get; set; } = new();
}

public class FlexiFutureCoverSpousePremiumDto
{
    public int SpouseIndex { get; set; }
    public string? Name { get; set; }
    public int? Anb { get; set; }
    public decimal Premium { get; set; }
    public bool Available { get; set; } = true;
    public string? UnavailableReason { get; set; }
}

public class FlexiFutureCoverPremiumDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool Available { get; set; } = true;
    public string? UnavailableReason { get; set; }

    /// <summary>Cover is already switched on in the request.</summary>
    public bool Selected { get; set; }

    /// <summary>
    /// Sum assured that applies with this cover switched on (PremiumToSa: protection is funded from the
    /// premium budget, so adding a cover lowers the savings sum assured).
    /// </summary>
    public decimal SumAssuredIfSelected { get; set; }

    public decimal MainPremium { get; set; }
    public List<FlexiFutureCoverSpousePremiumDto> Spouses { get; set; } = new();
    public decimal SpousesPremiumTotal { get; set; }
    /// <summary>MainPremium + SpousesPremiumTotal for this cover.</summary>
    public decimal CumulativePremium { get; set; }
}

public class FlexiFutureCoverPremiumsResultDto
{
    public int Anb { get; set; }
    public decimal SumAssured { get; set; }
    public decimal SavingsPremium { get; set; }
    public List<FlexiFutureCoverPremiumDto> Covers { get; set; } = new();
    /// <summary>Sum of each cover's CumulativePremium (menu total if all selected).</summary>
    public decimal CoversCumulativeTotal { get; set; }
}
public class FlexFutureCustomerExistsDTO
{
    public bool Exists { get; set; }
    public bool RequiresOTP { get; set; } = false;
    public string? Message { get; set; }
    public Guid QuoteId { get; set; }
    public FlexiFutureRegistrationStatus RegistrationStatus { get; set; }
        = FlexiFutureRegistrationStatus.NotRegistered;
    public Guid? OtpReference { get; set; }
    public GroupCustomerResponseDto? GroupCustomer { get; set; }
}

public enum FlexiFutureRegistrationStatus
{
    NotRegistered = 0,
    ExistingCustomer = 1,
    GroupCustomerPendingOtp = 2
}

public class GroupCustomerResponseDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string IdNumber { get; set; } = string.Empty;
    public string KraPin { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}

public class FlexiFutureQuoteResultDto : FlexiFutureComputeResultDto
{
    public Guid QuoteId { get; set; }
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
    public string? ReferralId { get; set; }
    public FlexiFutureClientDto Client { get; set; } = new();
}

public class FrequencyOptionDto
{
    public string Frequency { get; set; } = string.Empty;
    public decimal DiscountRate { get; set; }
    public string Label { get; set; } = string.Empty;
    public bool IsSingle { get; set; }
}

public class BenefitAgeLimitDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int? MinEntryAge { get; set; }
    public int? MaxCoverageAge { get; set; }
}

public class RiderOptionDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? MaxCoverageAge { get; set; }
    public decimal? DefaultCoverFactor { get; set; }
    public bool AvailableForSinglePremium { get; set; }
    public bool RequiresMainDeathCover { get; set; }
    public bool AppliesToMain { get; set; }
    public bool AppliesToSpouse { get; set; }
    public int SortOrder { get; set; }
}

public class FlexiFutureUiConfigDto
{
    // public int MinTerm { get; set; }
    // public int MaxTerm { get; set; }
    public IReadOnlyList<int> Terms { get; set; } = Array.Empty<int>();
    // public decimal SaBandLow { get; set; }
    // public decimal SaBandMid { get; set; }
    // public int MinTermBelowSaBandLow { get; set; }
    // public int MinTermBelowSaBandMid { get; set; }
    public int MinEntryAge { get; set; }
    public int MaxEntryAge { get; set; }
    public IReadOnlyList<FrequencyOptionDto> Frequencies { get; set; } = Array.Empty<FrequencyOptionDto>();
    // public IReadOnlyList<BenefitAgeLimitDto> BenefitAgeLimits { get; set; } = Array.Empty<BenefitAgeLimitDto>();
    public IReadOnlyList<RiderOptionDto> Riders { get; set; } = Array.Empty<RiderOptionDto>();
    public decimal MinSumAssured { get; set; }
    public decimal MaxSumAssured { get; set; }
    public decimal MaxCoverPerLife { get; set; }
    public decimal SpouseSaCap { get; set; }
    public IReadOnlyList<string> CalculationModes { get; set; } = Array.Empty<string>();
    public IReadOnlyList<decimal> DeathBenefitPercentOptions { get; set; } = Array.Empty<decimal>();
    public int MinMaturityPayments { get; set; }
    public int MaxMaturityPayments { get; set; }
    public int MaxSpouses { get; set; }
    public int MaxChildren { get; set; }
}
