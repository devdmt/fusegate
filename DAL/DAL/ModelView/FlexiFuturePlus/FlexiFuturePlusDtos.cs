using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using DAL.Model;
using DAL.Model.FlexiFuture;
using DAL.ModelView.FlexiFuture;
using DAL.ModelView.Settings;

namespace DAL.ModelView.FlexiFuturePlus;

public static class FlexiFuturePartnerHealthQuestionContexts
{
    public const string Main = "Main";
    public const string Spouse1 = "Spouse1";
    public const string Spouse2 = "Spouse2";
}

public class FlexiFuturePartnerHealthAnswerDto
{
    [Required]
    [JsonPropertyName("questionKey")]
    public string QuestionKey { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("answer")]
    public JsonElement Answer { get; set; }
}

public class FlexiFuturePartnerHealthQuestionsInputDto
{
    [JsonPropertyName("main")]
    public List<FlexiFuturePartnerHealthAnswerDto> Main { get; set; } = new();

    [JsonPropertyName("spouse1")]
    public List<FlexiFuturePartnerHealthAnswerDto> Spouse1 { get; set; } = new();

    [JsonPropertyName("spouse2")]
    public List<FlexiFuturePartnerHealthAnswerDto> Spouse2 { get; set; } = new();

    /// <summary>Free-text details when any main life or spouse health answer is Yes — maps to internal <c>nature</c> question.</summary>
    [JsonPropertyName("narration")]
    public string? Narration { get; set; }
}

public class FlexiFuturePartnerHealthAnswersByContextDto
{
    [JsonPropertyName("main")]
    public List<FlexiFuturePartnerHealthAnswerDto> Main { get; set; } = new();

    [JsonPropertyName("spouse1")]
    public List<FlexiFuturePartnerHealthAnswerDto> Spouse1 { get; set; } = new();

    [JsonPropertyName("spouse2")]
    public List<FlexiFuturePartnerHealthAnswerDto> Spouse2 { get; set; } = new();

    [JsonPropertyName("narration")]
    public string? Narration { get; set; }
}

public class FlexiFutureHealthQuestionLibraryItemDto
{
    [JsonPropertyName("questionKey")]
    public string QuestionKey { get; set; } = string.Empty;

    [JsonPropertyName("question")]
    public string Question { get; set; } = string.Empty;

    [JsonPropertyName("allowedAnswers")]
    public string AllowedAnswers { get; set; } = FlexiFuturePartnerHealthAnswerTypes.YesNo;
}

public class FlexiFuturePartnerHazardousFieldCatalogDto
{
    [JsonPropertyName("question")]
    public string Question { get; set; } = string.Empty;

    [JsonPropertyName("allowedAnswers")]
    public string AllowedAnswers { get; set; } = FlexiFuturePartnerHealthAnswerTypes.YesNo;
}

public class FlexiFuturePartnerHazardousQuestionsDto
{
    [JsonPropertyName("hazardousIntent")]
    public FlexiFuturePartnerHazardousFieldCatalogDto HazardousIntent { get; set; } = new();

    [JsonPropertyName("narration")]
    public FlexiFuturePartnerHazardousFieldCatalogDto Narration { get; set; } = new();
}

public class FlexiFuturePartnerHazardousDto
{
    [JsonPropertyName("hazardousIntent")]
    public JsonElement HazardousIntent { get; set; }

    /// <summary>Free-text details when hazardous intent is Yes — maps to internal <c>hazardousIntentExplanation</c>.</summary>
    [JsonPropertyName("narration")]
    public string? Narration { get; set; }
}

public class FlexiFuturePlusConsentDto
{
    /// <summary>OTP code from <c>POST /onboarding/send-otp</c>. Mutually exclusive with <see cref="Signature"/>.</summary>
    [StringLength(32)]
    [JsonPropertyName("otp")]
    public string? Otp { get; set; }

    /// <summary>Signature upload. Mutually exclusive with <see cref="Otp"/>.</summary>
    [JsonPropertyName("signature")]
    [JsonConverter(typeof(FileUploadDtoJsonConverter))]
    public FileUploadDTO? Signature { get; set; }
}

public class FlexiFuturePlusConsentStateDto
{
    [JsonPropertyName("completionMethod")]
    public string? CompletionMethod { get; set; }

    [JsonPropertyName("completedAt")]
    public DateTime? CompletedAt { get; set; }
}

/// <summary>
/// Persisted as int on FlexiContributions.PaymentMode — ordinals must match PensionCore / Akiba:
/// 0=Mpesa, 1=Bank, 2=Cheque, 3=Checkoff, 4=Ratiba.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PaymentMode
{
    Mpesa = 0,
    Bank = 1,
    Cheque = 2,
    Checkoff = 3,
    Ratiba = 4
}

/// <summary>
/// Spouse rider selection for quote recalculation — DOB/gender come from the persisted quote.
/// </summary>
public class FlexiFutureQuoteRecalculateSpouseDto
{
    [JsonPropertyName("spouseIndex")]
    public int SpouseIndex { get; set; } = 1;

    [JsonPropertyName("selectedRiders")]
    public List<string> SelectedRiders { get; set; } = new();
}

/// <summary>
/// Recalculate an existing quote during onboarding — no client or family KYC fields.
/// </summary>
public class FlexiFutureQuoteRecalculateDto
{
    [Required]
    [JsonPropertyName("quoteId")]
    public Guid QuoteId { get; set; }

    [Required]
    [JsonPropertyName("calculationMode")]
    public string CalculationMode { get; set; } = "PremiumToSa";

    [Required]
    [JsonPropertyName("policyTerm")]
    public int PolicyTerm { get; set; }

    [Required]
    [JsonPropertyName("frequency")]
    public string Frequency { get; set; } = "Monthly";

    [JsonPropertyName("deathBenefitPct")]
    public decimal DeathBenefitPct { get; set; } = 0.5m;

    [JsonPropertyName("maturityBenefitPayments")]
    public int MaturityBenefitPayments { get; set; } = 1;

    [JsonPropertyName("targetPremium")]
    public decimal? TargetPremium { get; set; }

    [JsonPropertyName("targetSumAssured")]
    public decimal? TargetSumAssured { get; set; }

    [JsonPropertyName("refferalCode")]
    public string? RefferalCode { get; set; }

    [JsonPropertyName("selectedRiders")]
    public List<string> SelectedRiders { get; set; } = new();

    [JsonPropertyName("spouses")]
    public List<FlexiFutureQuoteRecalculateSpouseDto> Spouses { get; set; } = new();
}

public class FlexiFutureQuoteSummaryDto
{
    public Guid QuoteId { get; set; }
    public string QuoteNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal SumAssured { get; set; }
    public decimal TotalPremiumPayable { get; set; }
    public int PolicyTerm { get; set; }
    public string Frequency { get; set; } = string.Empty;
    public DateOnly IssueDate { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? IdNumber { get; set; }
    public string? PartnerCode { get; set; }
}

/// <summary>
/// Full persisted quote (same shape as POST /quotes) plus resume/onboarding context for search results.
/// </summary>
public class FlexiFutureQuoteSearchResultDto : FlexiFutureQuoteResultDto
{
    public string? PartnerCode { get; set; }

    [JsonPropertyName("refferalCode")]
    public string? RefferalCode { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? PolicyId { get; set; }
    public string? PolicyStatus { get; set; }
    public string? OnboardingStep { get; set; }
    public bool CustomerDetailsComplete { get; set; }
    public bool InitialPaymentComplete { get; set; }
    public string? CallbackUrl { get; set; }
}

public class FlexiFuturePlusCustomerDetailsDto
{
    [Required]
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("idNumber")]
    public string IdNumber { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("dateOfBirth")]
    public DateOnly DateOfBirth { get; set; }

    [Required]
    [JsonPropertyName("gender")]
    public string Gender { get; set; } = string.Empty;

    [Required]
    [Phone]
    [JsonPropertyName("phoneNumber")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(15)]
    [JsonPropertyName("kraPin")]
    public string KraPin { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("citizenship")]
    public string Citizenship { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("residency")]
    public string Residency { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("occupation")]
    public string Occupation { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("monthlyIncome")]
    public string MonthlyIncome { get; set; } = string.Empty;

    [JsonPropertyName("employerName")]
    public string? EmployerName { get; set; }

    [JsonPropertyName("businessName")]
    public string? BusinessName { get; set; }

    [JsonPropertyName("usAddress")]
    public string? UsAddress { get; set; }

    [JsonPropertyName("taxIdNumber")]
    public string? TaxIdNumber { get; set; }

    [JsonPropertyName("taxRegisteredOtherCountry")]
    public bool? TaxRegisteredOtherCountry { get; set; }

    [JsonPropertyName("refferalCode")]
    public string? RefferalCode { get; set; }
}

public class FlexiFuturePlusFamilyMemberDto
{
    [Required]
    [JsonPropertyName("context")]
    public string Context { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("otherNames")]
    public string OtherNames { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("surname")]
    public string Surname { get; set; } = string.Empty;

    [JsonPropertyName("gender")]
    public string? Gender { get; set; }

    [EmailAddress]
    [JsonPropertyName("emailAddress")]
    public string? EmailAddress { get; set; }

    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("idNumber")]
    public string? IdNumber { get; set; }

    [JsonPropertyName("dateOfBirth")]
    public DateOnly? DateOfBirth { get; set; }

    [Required]
    [JsonPropertyName("relationship")]
    public string Relationship { get; set; } = string.Empty;
}

public class FlexiFuturePlusGuardianDto
{
    [Required]
    [JsonPropertyName("otherNames")]
    public string OtherNames { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("surname")]
    public string Surname { get; set; } = string.Empty;

    [JsonPropertyName("gender")]
    public string? Gender { get; set; }

    [EmailAddress]
    [JsonPropertyName("emailAddress")]
    public string? EmailAddress { get; set; }

    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("idNumber")]
    public string? IdNumber { get; set; }

    [JsonPropertyName("dateOfBirth")]
    public DateOnly? DateOfBirth { get; set; }

    [JsonPropertyName("relationship")]
    public string? Relationship { get; set; }
}

public class FlexiFuturePlusBeneficiaryDto
{
    [Required]
    [JsonPropertyName("otherNames")]
    public string OtherNames { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("surname")]
    public string Surname { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("dateOfBirth")]
    public DateOnly DateOfBirth { get; set; }

    [JsonPropertyName("gender")]
    public string? Gender { get; set; }

    [EmailAddress]
    [JsonPropertyName("emailAddress")]
    public string? EmailAddress { get; set; }

    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("idNumber")]
    public string? IdNumber { get; set; }

    [JsonPropertyName("relationship")]
    public string? Relationship { get; set; }

    [Range(0.01, 100)]
    [JsonPropertyName("percentageShare")]
    public decimal PercentageShare { get; set; }

    [JsonPropertyName("guardian")]
    public FlexiFuturePlusGuardianDto? Guardian { get; set; }
}

public class FlexiFuturePlusOnboardingRequestDto
{
    [Required]
    [JsonPropertyName("quoteId")]
    public Guid QuoteId { get; set; }

    [JsonPropertyName("callbackUrl")]
    public string? CallbackUrl { get; set; }

    [JsonPropertyName("refferalCode")]
    public string? RefferalCode { get; set; }

    [JsonPropertyName("customerDetails")]
    public FlexiFuturePlusCustomerDetailsDto? CustomerDetails { get; set; }

    [JsonPropertyName("familyMembers")]
    public List<FlexiFuturePlusFamilyMemberDto> FamilyMembers { get; set; } = new();

    [JsonPropertyName("beneficiaries")]
    public List<FlexiFuturePlusBeneficiaryDto> Beneficiaries { get; set; } = new();

    [JsonPropertyName("healthQuestions")]
    public FlexiFuturePartnerHealthQuestionsInputDto? HealthQuestions { get; set; }

    [JsonPropertyName("hazardous")]
    public FlexiFuturePartnerHazardousDto? Hazardous { get; set; }

    [JsonPropertyName("consent")]
    public FlexiFuturePlusConsentDto? Consent { get; set; }
}

public class FlexiFuturePlusOnboardingStateDto
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
    public string? PartnerCode { get; set; }
    public string? CallbackUrl { get; set; }
    public FlexiFuturePlusCustomerDetailsDto? CustomerDetails { get; set; }

    [JsonPropertyName("familyMembers")]
    public List<FlexiFuturePlusFamilyMemberDto> FamilyMembers { get; set; } = new();

    public FlexiFuturePartnerHealthAnswersByContextDto? HealthQuestions { get; set; }

    [JsonPropertyName("hazardous")]
    public FlexiFuturePartnerHazardousDto? Hazardous { get; set; }

    [JsonPropertyName("beneficiaries")]
    public List<FlexiFuturePlusBeneficiaryDto> Beneficiaries { get; set; } = new();

    [JsonPropertyName("consent")]
    public FlexiFuturePlusConsentStateDto? Consent { get; set; }

    public FlexiFuturePlusSectionStatusDto SectionStatus { get; set; } = new();
}

public class FlexiFuturePlusSectionStatusDto
{
    public string CustomerDetails { get; set; } = "NotStarted";
    public string FamilyMembers { get; set; } = "NotStarted";
    public string HealthQuestions { get; set; } = "NotStarted";
    public string Hazardous { get; set; } = "NotStarted";
    public string Beneficiaries { get; set; } = "NotStarted";
    public string Consent { get; set; } = "NotStarted";
}

public class FlexiFuturePlusOnboardingQuestionsDto
{
    public Guid QuoteId { get; set; }

    [JsonPropertyName("requiredContexts")]
    public List<string> RequiredContexts { get; set; } = new();

    [JsonPropertyName("main")]
    public List<string> Main { get; set; } = new();

    [JsonPropertyName("spouse1")]
    public List<string> Spouse1 { get; set; } = new();

    [JsonPropertyName("spouse2")]
    public List<string> Spouse2 { get; set; } = new();

    [JsonPropertyName("questionLibrary")]
    public List<FlexiFutureHealthQuestionLibraryItemDto> QuestionLibrary { get; set; } = new();

    [JsonPropertyName("hazardous")]
    public FlexiFuturePartnerHazardousQuestionsDto Hazardous { get; set; } = new();
}

public class FlexiFuturePlusSendOtpRequestDto
{
    [Required]
    public Guid QuoteId { get; set; }
}

public class FlexiFuturePlusAddPaymentRequestDto
{
    [JsonPropertyName("quoteId")]
    public Guid? QuoteId { get; set; }

    [JsonPropertyName("quoteNumber")]
    public string? QuoteNumber { get; set; }

    public double Amount { get; set; }

    [Required]
    public PaymentMode PaymentMode { get; set; }

    public string? PhoneNumber { get; set; }

    [JsonPropertyName("paymentReference")]
    public string? PaymentReference { get; set; }

    public string? Narration { get; set; }

    [JsonPropertyName("callbackUrl")]
    public string? CallbackUrl { get; set; }
}

public class FlexiFuturePlusPaymentResultDto
{
    public Guid ContributionId { get; set; }
    public PaymentMode PaymentMode { get; set; }
    public string PaymentStatus { get; set; } = DAL.Model.PaymentStatus.Pending.ToString();
    public string? StkTransactionId { get; set; }
    public string? CorrelationId { get; set; }
}

public class FlexiFuturePlusStkPollResultDto
{
    public string StkTransactionId { get; set; } = string.Empty;
    public bool Finalized { get; set; }
    public bool Processed { get; set; }
    public string? ResponseCode { get; set; }
    public string? Amount { get; set; }
    public string? MpesaReceiptNumber { get; set; }
    public string? ContributionTrnId { get; set; }
}
