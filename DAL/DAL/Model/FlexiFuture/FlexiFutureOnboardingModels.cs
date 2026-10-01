namespace DAL.Model.FlexiFuture;

public static class FlexiFutureOnboardingSteps
{
    public const string KYC = "KYC";
    public const string FamilyMembers = "FamilyMembers";
    public const string HealthDeclaration = "HealthDeclaration";
    public const string OccupationHazards = "OccupationHazards";
    public const string Beneficiaries = "Beneficiaries";
    public const string Complete = "Complete";
}

public static class FlexiFuturePolicyStatuses
{
    public const string Draft = "Draft";
    public const string Submitted = "Submitted";
    public const string PendingApproval = "PendingApproval";
    public const string Approved = "Approved";
    public const string Active = "Active";
    public const string Rejected = "Rejected";
    public const string Cancelled = "Cancelled";
}

public static class FlexiFutureHealthContexts
{
    public const string Customer = "Customer";
    public const string FamilyMember = "FamilyMember";
}

public static class FlexiFutureHealthQuestionTypes
{
    public const string Checkbox = "Checkbox";
    public const string Text = "Text";
    public const string Group = "Group";

    // Legacy values retained for older seeded rows until script 10 remaps them.
    public const string YesNo = "YesNo";
    public const string YesNoFollowUp = "YesNoFollowUp";
}

public static class FlexiFutureHealthQuestionCategories
{
    public const string HealthCheck = "healthCheck";
    public const string Vitals = "vitals";
    public const string HealthScan = "healthScan";
    public const string Hazardous = "hazardous";
}

public static class FlexiFutureHealthQuestionKeys
{
    public const string WeightStationary = "weightStationary";
    public const string WeightTrend = "weightTrend";
    public const string Nature = "nature";
    public const string HazardousIntent = "hazardousIntent";
    public const string HazardousIntentExplanation = "hazardousIntentExplanation";
}

public static class FlexiFuturePartnerHealthAnswerTypes
{
    public const string YesNo = "YesNo";
    public const string Open = "Open";
    public const string WeightTrend = "Increasing|Decreasing";
}

public static class FlexiFutureIdValidationStatuses
{
    public const int Pending = 0;
    public const int Valid = 1;
    public const int Failed = 2;
}

public static class FlexiFutureOnboardingCompletionMethods
{
    public const string Signature = "Signature";
    public const string Otp = "Otp";
}

public class FlexiFuturePolicy
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid QuoteId { get; set; }
    public string PolicyStatus { get; set; } = FlexiFuturePolicyStatuses.Draft;
    public string OnboardingStep { get; set; } = FlexiFutureOnboardingSteps.KYC;
    public string? PolicyNo { get; set; }
    public string? ExternalRefId { get; set; }
    public DateOnly? EffectiveDate { get; set; }
    public bool IsApproved { get; set; }
    public string? ApprovedBy { get; set; }
    public bool InitialPaymentComplete { get; set; }
    public string? Signature { get; set; }
    public string? SignaturePath { get; set; }
    public string? CompletionMethod { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int Source { get; set; } = 3;
    public string? PartnerCode { get; set; }
    public string? CallbackUrl { get; set; }

    public FlexiFutureQuote? Quote { get; set; }
    public ICollection<FlexiFutureFamilyMember> FamilyMembers { get; set; } = new List<FlexiFutureFamilyMember>();
    public ICollection<FlexiFutureBeneficiary> Beneficiaries { get; set; } = new List<FlexiFutureBeneficiary>();
    public ICollection<FlexiFutureHealthInfo> HealthInfo { get; set; } = new List<FlexiFutureHealthInfo>();
    public ICollection<FlexiFutureOccupationHazard> OccupationHazards { get; set; } = new List<FlexiFutureOccupationHazard>();
}

public class FlexiFutureFamilyMember
{
    public Guid Id { get; set; }
    public Guid PolicyId { get; set; }
    public string OtherNames { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string? Gender { get; set; }
    public string? EmailAddress { get; set; }
    public string? PhoneNumber { get; set; }
    public string? IDNumber { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? KRAPinNo { get; set; }
    public string? Nationality { get; set; }
    public string? Residency { get; set; }
    public string? USAddress { get; set; }
    public string? Relationship { get; set; }
    public string? TIN { get; set; }
    public int IsValidIDNumber { get; set; } = FlexiFutureIdValidationStatuses.Pending;
    public string? IPRSFullName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? AddedBy { get; set; }

    public FlexiFuturePolicy? Policy { get; set; }
}

public class FlexiFutureBeneficiary
{
    public Guid Id { get; set; }
    public Guid PolicyId { get; set; }
    public string OtherNames { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string? Gender { get; set; }
    public string? EmailAddress { get; set; }
    public string? PhoneNumber { get; set; }
    public string? IDNumber { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? Relationship { get; set; }
    public decimal PercentageShare { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public FlexiFuturePolicy? Policy { get; set; }
    public ICollection<FlexiFutureGuardian> Guardians { get; set; } = new List<FlexiFutureGuardian>();
}

public class FlexiFutureGuardian
{
    public Guid Id { get; set; }
    public Guid BeneficiaryId { get; set; }
    public string OtherNames { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string? Gender { get; set; }
    public string? EmailAddress { get; set; }
    public string? PhoneNumber { get; set; }
    public string? IDNumber { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? Relationship { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public FlexiFutureBeneficiary? Beneficiary { get; set; }
}

public class FlexiFutureHealthInfo
{
    public Guid Id { get; set; }
    public Guid PolicyId { get; set; }
    public Guid QuestionId { get; set; }
    public string Question { get; set; } = string.Empty;
    public string QuestionKey { get; set; } = string.Empty;
    public string? Answer { get; set; }
    public string Context { get; set; } = FlexiFutureHealthContexts.Customer;
    public Guid ContextId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public FlexiFuturePolicy? Policy { get; set; }
    public HealthQuestionsLibrary? HealthQuestion { get; set; }
}

public class FlexiFutureOccupationHazard
{
    public Guid Id { get; set; }
    public Guid PolicyId { get; set; }
    public Guid QuestionId { get; set; }
    public string Question { get; set; } = string.Empty;
    public string QuestionKey { get; set; } = string.Empty;
    public string? Answer { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public FlexiFuturePolicy? Policy { get; set; }
    public HealthQuestionsLibrary? HealthQuestion { get; set; }
}

public class HealthQuestionsLibrary
{
    public Guid Id { get; set; }
    public string? ClientQuestionCode { get; set; }
    public string Question { get; set; } = string.Empty;
    public string QuestionKey { get; set; } = string.Empty;
    public string QuestionType { get; set; } = FlexiFutureHealthQuestionTypes.Checkbox;
    public string Category { get; set; } = FlexiFutureHealthQuestionCategories.HealthCheck;
    public string? Options { get; set; }
    public string? Validations { get; set; }
    public int QuestionOrder { get; set; }
    public Guid? ParentQuestionId { get; set; }
    public string? Description { get; set; }
    public string? Placeholder { get; set; }
    public string? ConditionalLogic { get; set; }
    public bool IsPerMember { get; set; }
    public bool IsRequired { get; set; } = true;
    public string? SpouseQuestion { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public HealthQuestionsLibrary? ParentQuestion { get; set; }
    public ICollection<HealthQuestionsLibrary> ChildQuestions { get; set; } = new List<HealthQuestionsLibrary>();
}
