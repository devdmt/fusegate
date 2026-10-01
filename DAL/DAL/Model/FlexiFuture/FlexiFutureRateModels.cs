namespace DAL.Model.FlexiFuture;

public class FlexiFutureProductSettings
{
    public Guid Id { get; set; }
    public decimal MinSumAssured { get; set; }
    public decimal MaxCoverPerLife { get; set; }
    public decimal SpouseSaCap { get; set; }
    public decimal ApprovalRequiredAboveSa { get; set; }
    public int MinTerm { get; set; }
    public int MaxTerm { get; set; }
    public decimal SaBandLow { get; set; }
    public decimal SaBandMid { get; set; }
    public int MinTermBelowSaBandLow { get; set; }
    public int MinTermBelowSaBandMid { get; set; }
    public int MinEntryAge { get; set; }
    public int MaxEntryAge { get; set; }
    public decimal PhclRate { get; set; }
    public decimal CiCoverPct { get; set; }
    public decimal TaxReliefRate { get; set; }
    public decimal TaxReliefAnnualCap { get; set; }
    public int TaxReliefMinTerm { get; set; }
    public decimal MaturityEscalationRate { get; set; }
    public decimal ChildLastExpenseSa { get; set; }
    public int MinMaturityPayments { get; set; }
    public int MaxMaturityPayments { get; set; }
    public int MaxSpouses { get; set; } = 2;
    public int MaxChildren { get; set; } = 6;
    public string RateSetVersion { get; set; } = "v1.0.0";
    public bool Active { get; set; } = true;
    public DateOnly? EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
}

public class FlexiFutureFrequencyDiscount
{
    public Guid Id { get; set; }
    public string Frequency { get; set; } = string.Empty;
    public decimal DiscountRate { get; set; }
    public int MonthFactor { get; set; }
    public bool UsesElevenTwelfths { get; set; }
    public int SortOrder { get; set; }
    public bool Active { get; set; } = true;
}

public class FlexiFutureRateTable
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string PremiumMode { get; set; } = "Regular";
    public string Name { get; set; } = string.Empty;
    public string LookupKind { get; set; } = string.Empty;
    public string RateUnit { get; set; } = "Per1000";
    public decimal? DefaultCoverFactor { get; set; }
    public int? MinEntryAge { get; set; }
    public int? MaxCoverageAge { get; set; }
    public bool IsRider { get; set; }
    public bool AvailableForSinglePremium { get; set; } = true;
    public bool RequiresMainDeathCover { get; set; }
    public bool AppliesToMain { get; set; } = true;
    public bool AppliesToSpouse { get; set; }
    public int SortOrder { get; set; }
    public bool Active { get; set; } = true;

    public ICollection<FlexiFutureRate> Rates { get; set; } = new List<FlexiFutureRate>();
}

public class FlexiFutureRate
{
    public Guid Id { get; set; }
    public Guid RateTableId { get; set; }
    public decimal BandMin { get; set; }
    public decimal? BandMax { get; set; }
    public int? TermYears { get; set; }
    public int? PolicyYear { get; set; }
    public decimal Rate { get; set; }
    public bool Active { get; set; } = true;

    public FlexiFutureRateTable? RateTable { get; set; }
}

public static class FlexiFutureRateCodes
{
    public const string Savings = "SAVINGS";
    public const string WopDeathLoading = "WOP_DEATH_LOADING";
    public const string Life = "LIFE";
    public const string Ci = "CI";
    public const string Ptd = "PTD";
    public const string WopCi = "WOP_CI";
    public const string WopPtd = "WOP_PTD";
    public const string WopRetrench = "WOP_RETRENCH";
    public const string ChildLe = "CHILD_LE";
    public const string Surrender = "SURRENDER";
}

public static class FlexiFuturePremiumModes
{
    public const string Regular = "Regular";
    public const string Single = "Single";
}

public static class FlexiFutureLookupKinds
{
    public const string SaBandTerm = "SaBandTerm";
    public const string AgeTerm = "AgeTerm";
    public const string AgeOnly = "AgeOnly";
    public const string PolicyYear = "PolicyYear";
}

public static class FlexiFutureFrequencies
{
    public const string Monthly = "Monthly";
    public const string Quarterly = "Quarterly";
    public const string SemiAnnual = "Semi-Annual";
    public const string Annual = "Annual";
    public const string Single = "Single";
}

public static class FlexiFutureCalculationModes
{
    public const string PremiumToSa = "PremiumToSa";
    public const string SaToPremium = "SaToPremium";
}

public static class FlexiFutureLifeRoles
{
    public const string Main = "Main";
    public const string Spouse1 = "Spouse1";
    public const string Spouse2 = "Spouse2";
    public const string All = "All";
}

public static class FlexiFutureQuoteStatuses
{
    public const string Quoted = "Quoted";
    public const string Cancelled = "Cancelled";
}
