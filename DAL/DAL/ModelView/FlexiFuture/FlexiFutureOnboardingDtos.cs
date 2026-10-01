using System.ComponentModel.DataAnnotations;

namespace DAL.ModelView.FlexiFuture;

public class FlexiFutureOnboardingQuoteRequestDto
{
    [Required]
    public Guid QuoteId { get; set; }
}

public class FlexiFutureFamilyMemberDto
{
    public Guid? Id { get; set; }

    /// <summary>Partner context: Spouse1, Spouse2, Child1–Child6 (§7.2).</summary>
    [Required]
    [StringLength(32)]
    public string Context { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string OtherNames { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Surname { get; set; } = string.Empty;

    [StringLength(16)]
    public string? Gender { get; set; }

    [EmailAddress]
    [StringLength(200)]
    public string? EmailAddress { get; set; }

    [StringLength(40)]
    public string? PhoneNumber { get; set; }

    [StringLength(64)]
    public string? IDNumber { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    [StringLength(100)]
    public string? Nationality { get; set; }

    [StringLength(100)]
    public string? Residency { get; set; }

    [StringLength(500)]
    public string? USAddress { get; set; }

    [StringLength(64)]
    public string? Relationship { get; set; }

    [StringLength(64)]
    public string? TIN { get; set; }
}

public class FlexiFutureGuardianDto
{
    public Guid? Id { get; set; }

    [Required]
    [StringLength(200)]
    public string OtherNames { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Surname { get; set; } = string.Empty;

    [StringLength(16)]
    public string? Gender { get; set; }

    [EmailAddress]
    [StringLength(200)]
    public string? EmailAddress { get; set; }

    [StringLength(40)]
    public string? PhoneNumber { get; set; }

    [StringLength(64)]
    public string? IDNumber { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    [StringLength(64)]
    public string? Relationship { get; set; }
}

public class FlexiFutureBeneficiaryDto
{
    public Guid? Id { get; set; }

    [Required]
    [StringLength(200)]
    public string OtherNames { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Surname { get; set; } = string.Empty;

    [StringLength(16)]
    public string? Gender { get; set; }

    [EmailAddress]
    [StringLength(200)]
    public string? EmailAddress { get; set; }

    [StringLength(40)]
    public string? PhoneNumber { get; set; }

    [StringLength(64)]
    public string? IDNumber { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    [StringLength(64)]
    public string? Relationship { get; set; }

    [Range(0.01, 100)]
    public decimal PercentageShare { get; set; }

    public List<FlexiFutureGuardianDto> Guardians { get; set; } = new();
}

public class FlexiFutureFamilyMembersSaveDto
{
    [Required]
    public Guid QuoteId { get; set; }

    public List<FlexiFutureFamilyMemberDto> FamilyMembers { get; set; } = new();
}

public class FlexiFutureBeneficiariesSaveDto
{
    [Required]
    public Guid QuoteId { get; set; }

    public List<FlexiFutureBeneficiaryDto> Beneficiaries { get; set; } = new();
}

public class FlexiFutureHealthAnswerDto
{
    [Required]
    public Guid QuestionId { get; set; }

    [Required]
    [StringLength(32)]
    public string Context { get; set; } = "Customer";

    [Required]
    public Guid ContextId { get; set; }

    /// <summary>JSON-serialized answer payload (string, Yes/No, or follow-up object).</summary>
    public string? Answer { get; set; }
}

public class FlexiFutureHealthDeclarationSaveDto
{
    [Required]
    public Guid QuoteId { get; set; }

    public List<FlexiFutureHealthAnswerDto> Answers { get; set; } = new();
}

public class FlexiFutureOccupationHazardAnswerDto
{
    [Required]
    public Guid QuestionId { get; set; }

    /// <summary>JSON-serialized answer payload (string, Yes/No, or follow-up object).</summary>
    public string? Answer { get; set; }
}

public class FlexiFutureOccupationHazardsSaveDto
{
    [Required]
    public Guid QuoteId { get; set; }

    public List<FlexiFutureOccupationHazardAnswerDto> Answers { get; set; } = new();
}

public class FlexiFutureOtpDto
{
    public bool Sent { get; set; }
    public string? Message { get; set; }
    public string? ResponseCode { get; set; }
}

public class FlexiFutureOnboardingStatusDto
{
    public Guid PolicyId { get; set; }
    public Guid QuoteId { get; set; }
    public Guid CustomerId { get; set; }
    public string PolicyStatus { get; set; } = string.Empty;
    public string OnboardingStep { get; set; } = string.Empty;
    public bool CustomerDetailsComplete { get; set; }
    public string? PolicyNo { get; set; }
    public bool IsApproved { get; set; }
    public bool InitialPaymentComplete { get; set; }
    public FlexiFutureOtpDto? Otp { get; set; }
}

public class FlexiFutureFamilyMembersSaveResultDto : FlexiFutureOnboardingStatusDto
{
    public List<FlexiFutureFamilyMemberResultDto> FamilyMembers { get; set; } = new();
}

public class FlexiFutureBeneficiariesSaveResultDto : FlexiFutureOnboardingStatusDto
{
    public List<FlexiFutureBeneficiaryResultDto> Beneficiaries { get; set; } = new();
}

public class FlexiFutureHealthDeclarationSaveResultDto : FlexiFutureOnboardingStatusDto
{
    public List<FlexiFutureHealthInfoResultDto> HealthInfo { get; set; } = new();
}

public class FlexiFutureOccupationHazardsSaveResultDto : FlexiFutureOnboardingStatusDto
{
    public List<FlexiFutureOccupationHazardResultDto> OccupationHazards { get; set; } = new();
}

public class FlexiFutureHealthQuestionOptionDto
{
    public string Value { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
}

public class FlexiFutureHealthQuestionDto
{
    public Guid Id { get; set; }
    public string? ClientQuestionCode { get; set; }
    public string Question { get; set; } = string.Empty;
    public string QuestionKey { get; set; } = string.Empty;
    public string QuestionType { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Placeholder { get; set; }
    public bool IsPerMember { get; set; }
    public bool Required { get; set; } = true;
    public string? SpouseQuestion { get; set; }
    public int QuestionOrder { get; set; }
    public List<FlexiFutureHealthQuestionOptionDto> Options { get; set; } = new();
    public object? Validation { get; set; }
    public object? Condition { get; set; }
    public List<FlexiFutureHealthQuestionDto> ChildQuestions { get; set; } = new();
}

public class FlexiFutureHealthInfoResultDto
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public string Question { get; set; } = string.Empty;
    public string QuestionKey { get; set; } = string.Empty;
    public string? Answer { get; set; }
    public string Context { get; set; } = string.Empty;
    public Guid ContextId { get; set; }
}

public class FlexiFutureOccupationHazardResultDto
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public string Question { get; set; } = string.Empty;
    public string QuestionKey { get; set; } = string.Empty;
    public string? Answer { get; set; }
}

public class FlexiFutureFamilyMemberResultDto : FlexiFutureFamilyMemberDto
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class FlexiFutureGuardianResultDto : FlexiFutureGuardianDto
{
}

public class FlexiFutureBeneficiaryResultDto : FlexiFutureBeneficiaryDto
{
    public new List<FlexiFutureGuardianResultDto> Guardians { get; set; } = new();
}

public class FlexiFutureOnboardingDetailDto : FlexiFutureOnboardingStatusDto
{
    public FlexiFuturePolicyQuoteDto Quote { get; set; } = new();
    public List<FlexiFutureFamilyMemberResultDto> FamilyMembers { get; set; } = new();
    public List<FlexiFutureBeneficiaryResultDto> Beneficiaries { get; set; } = new();
    public List<FlexiFutureHealthInfoResultDto> HealthInfo { get; set; } = new();
    public List<FlexiFutureOccupationHazardResultDto> OccupationHazards { get; set; } = new();
}

public class FlexiFutureOnboardingSpouseRiderDto
{
    public int SpouseIndex { get; set; } = 1;
    public List<string> SelectedRiders { get; set; } = new();
}

public class FlexiFutureOnboardingQuoteUpdateDto
{
    [Required]
    public Guid QuoteId { get; set; }

    [Required]
    public string CalculationMode { get; set; } = "PremiumToSa";

    [Required]
    public int PolicyTerm { get; set; }

    [Required]
    public string Frequency { get; set; } = "Monthly";

    public decimal DeathBenefitPct { get; set; } = 0.5m;
    public int MaturityBenefitPayments { get; set; } = 1;
    public decimal? TargetPremium { get; set; }
    public decimal? TargetSumAssured { get; set; }
    public string? ReferralId { get; set; }
    public List<string> SelectedRiders { get; set; } = new();
    public List<FlexiFutureOnboardingSpouseRiderDto> Spouses { get; set; } = new();
}

public class FlexiFutureOnboardingCompleteSendOtpDto
{
    [Required]
    public Guid QuoteId { get; set; }
}

public class FlexiFutureOnboardingCompleteDto
{
    [Required]
    public Guid QuoteId { get; set; }

    /// <summary>
    /// Signature upload. Accepts { name, extension, data } or a raw base64 string.
    /// The data-URI header (data:image/png;base64,) is optional — the API adds it.
    /// Mutually exclusive with Otp.
    /// </summary>
    [System.Text.Json.Serialization.JsonConverter(typeof(DAL.ModelView.Settings.FileUploadDtoJsonConverter))]
    public DAL.ModelView.Settings.FileUploadDTO? Signature { get; set; }

    /// <summary>OTP code. Mutually exclusive with Signature.</summary>
    [StringLength(32)]  
    public string? Otp { get; set; }
}
    
public class FlexiFutureMyQuoteDto
{
    public Guid QuoteId { get; set; }
    public string QuoteNumber { get; set; } = string.Empty;
    public decimal SumAssured { get; set; }
    public decimal TotalPremiumPayable { get; set; }
    public int PolicyTerm { get; set; }
    public string Frequency { get; set; } = string.Empty;
    public DateOnly IssueDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? OnboardingStep { get; set; }
    public int CompletionPercent { get; set; }
}

public class FlexiFutureMyPolicyDto
{
    public Guid PolicyId { get; set; }
    public Guid QuoteId { get; set; }
    public string QuoteNumber { get; set; } = string.Empty;
    public string? PolicyNo { get; set; }
    public string PolicyStatus { get; set; } = string.Empty;
    public DateOnly? EffectiveDate { get; set; }
    public string Frequency { get; set; } = string.Empty;
    public int PolicyTerm { get; set; }
    public decimal TotalPremiumPayable { get; set; }
    public DateOnly MaturityDate { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public decimal SumAssured { get; set; }
}

public class FlexiFuturePolicyQuoteDto
{
    public string QuoteNumber { get; set; } = string.Empty;
    public decimal SumAssured { get; set; }
    public string Frequency { get; set; } = string.Empty;
    public int Term { get; set; }
    public decimal Premium { get; set; }
}

public class FlexiFundBalanceAccountDto
{
    public Guid Id { get; set; }
    public Guid QuoteId { get; set; }
    public Guid PolicyId { get; set; }
    public string PolicyStatus { get; set; } = string.Empty;
    public string QuoteNumber { get; set; } = string.Empty;
    public string? PolicyNumber { get; set; }
    public decimal SumAssured { get; set; }
    public decimal MaturityPayout { get; set; }
    public decimal TotalContribution { get; set; }
    public string Frequency { get; set; } = string.Empty;
    public int Term { get; set; }
    public decimal PremiumPayable { get; set; }
    public decimal TotalPremiumPayable { get; set; }
    public List<FlexiFundSelectedCoverDto> SelectedCovers { get; set; } = new();
}

public class FlexiFundSelectedCoverDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class FlexiFundBalancesDto
{
    public decimal TotalContributions { get; set; }
    public decimal SumAssured { get; set; }
    public decimal MaturityPayout { get; set; }
    public List<FlexiFundBalanceAccountDto> FlexiFundAccounts { get; set; } = new();
}

public static class FlexiFuturePaymentStatuses
{
    public const string Paid = "Paid";
    public const string Pending = "Pending";
}
