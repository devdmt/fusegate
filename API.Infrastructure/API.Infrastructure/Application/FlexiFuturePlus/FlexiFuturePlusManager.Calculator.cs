using DAL.ModelView.FlexiFuture;
using DAL.Model.FlexiFuture;
using DAL.ModelView.FlexiFuturePlus;
using Microsoft.Extensions.Logging;

namespace API.Infrastructure.Application.FlexiFuturePlus;

public partial class FlexiFuturePlusManager
{
    public async Task<FlexiFuturePlusResponse<FlexiFutureComputeResultDto>> ComputeAsync(
        FlexiFutureComputeRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var response = new FlexiFuturePlusResponse<FlexiFutureComputeResultDto>();
        try
        {
            var pack = await GetActiveRatesAsync(cancellationToken);
            var anb = ComputeAnb(request.Client.DateOfBirth, request.IssueDate);
            ValidateProductRules(request, pack, anb, sumAssured: null);

            decimal sumAssured;
            if (string.Equals(request.CalculationMode, FlexiFutureCalculationModes.SaToPremium, StringComparison.OrdinalIgnoreCase))
            {
                if (!request.TargetSumAssured.HasValue || request.TargetSumAssured <= 0)
                    throw new InvalidOperationException("TargetSumAssured is required for SaToPremium.");
                sumAssured = request.TargetSumAssured.Value;
                ValidateSaTermRules(pack.Settings, sumAssured, request.PolicyTerm);
            }
            else
            {
                if (!request.TargetPremium.HasValue || request.TargetPremium <= 0)
                    throw new InvalidOperationException("TargetPremium is required for PremiumToSa.");
                sumAssured = SolveSumAssuredFromPremium(request, pack, anb);
                ValidateSaTermRules(pack.Settings, sumAssured, request.PolicyTerm);
            }

            var result = BuildResult(request, pack, anb, sumAssured);
            if (string.Equals(request.CalculationMode, FlexiFutureCalculationModes.PremiumToSa, StringComparison.OrdinalIgnoreCase)
                && request.TargetPremium.HasValue)
            {
                ApplyPremiumToSaLevyDeduction(result, request.TargetPremium.Value, pack.Settings.PhclRate);
            }
            response.Success = true;
            response.Message = "Successfully calculated FlexiFuture illustration.";
            response.Data = result;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = FlexiFutureErrorHandling.ToClientMessage(ex, _logger, nameof(ComputeAsync));
        }

        return response;
    }

    public async Task<FlexiFuturePlusResponse<FlexiFutureCoverPremiumsResultDto>> ComputeCoverPremiumsAsync(
        FlexiFutureCoverPremiumsRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var response = new FlexiFuturePlusResponse<FlexiFutureCoverPremiumsResultDto>();
        try
        {
            var pack = await GetActiveRatesAsync(cancellationToken);
            var anb = ComputeAnb(request.Client.DateOfBirth, request.IssueDate);
            ValidateCoverPremiumRules(request, pack, anb);

            // One solve per distinct cover scenario; reused across the menu rows.
            var solveCache = new Dictionary<string, decimal>(StringComparer.Ordinal);
            var sumAssured = ResolveSumAssuredForCoverPremiums(request, pack, anb, solveCache);
            ValidateSaTermRules(pack.Settings, sumAssured, request.PolicyTerm);

            var premiumMode = RateLookup.ResolvePremiumMode(request.Frequency);
            var freq = RateLookup.GetFrequency(pack, request.Frequency);
            var savingsMonthly = ComputeSavingsMonthlyEquivalent(sumAssured, anb, request.PolicyTerm, premiumMode, pack);
            var savingsPremium = RoundUpPremium(ApplyFrequencyAndDiscount(savingsMonthly, freq));

            var covers = BuildCoverPremiumMenu(
                request,
                pack,
                anb,
                premiumMode,
                freq,
                solveCache);

            response.Success = true;
            response.Message = "Cover premiums calculated successfully.";
            response.Data = new FlexiFutureCoverPremiumsResultDto
            {
                Anb = anb,
                SumAssured = RoundUpAwayFromZero(sumAssured),
                SavingsPremium = savingsPremium,
                Covers = covers,
                CoversCumulativeTotal = RoundUpAwayFromZero(covers.Sum(c => c.CumulativePremium))
            };
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = FlexiFutureErrorHandling.ToClientMessage(ex, _logger, nameof(ComputeCoverPremiumsAsync));
        }

        return response;
    }

    private static void ValidateCoverPremiumRules(
        FlexiFutureCoverPremiumsRequestDto request,
        FlexiFutureRatePack pack,
        int anb)
    {
        try
        {
            var s = pack.Settings;
            var today = DateOnly.FromDateTime(DateTime.Today);
            if (request.IssueDate < today)
                throw new InvalidOperationException("Issue date must be today or a future date.");

            if (request.PolicyTerm < s.MinTerm || request.PolicyTerm > s.MaxTerm)
                throw new InvalidOperationException($"Policy term must be between {s.MinTerm} and {s.MaxTerm}.");

            if (anb < s.MinEntryAge || anb > s.MaxEntryAge)
                throw new InvalidOperationException($"Client ANB must be between {s.MinEntryAge} and {s.MaxEntryAge}.");

            if (string.IsNullOrWhiteSpace(request.Client.Gender))
                throw new InvalidOperationException("Client gender is required.");

            RateLookup.GetFrequency(pack, request.Frequency);

            if (request.DeathBenefitPct > 0)
            {
                var life = RateLookup.GetTable(
                    pack,
                    FlexiFutureRateCodes.Life,
                    RateLookup.ResolvePremiumMode(request.Frequency));
                if (life != null)
                    ValidateRiderAge(life, anb);
            }

            ValidateQuoteSpouses(request, pack);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static void ValidateQuoteSpouses(
        FlexiFutureComputeRequestDto request,
        FlexiFutureRatePack pack)
    {
        try
        {
            var spouses = (request.Spouses ?? new List<FlexiFutureSpouseDto>())
                .Select(s => (
                    s.SpouseIndex,
                    s.DateOfBirth,
                    s.Name,
                    (IReadOnlyList<string>?)s.SelectedRiders))
                .ToList();

            ValidateQuoteSpouseEntries(
                request.IssueDate,
                request.Frequency,
                pack,
                spouses);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static void ValidateQuoteSpouses(
        FlexiFutureCoverPremiumsRequestDto request,
        FlexiFutureRatePack pack)
    {
        try
        {
            var spouses = (request.Spouses ?? new List<FlexiFutureCoverPremiumsSpouseDto>())
                .Select(s => (
                    s.SpouseIndex,
                    s.DateOfBirth,
                    (string?)null,
                    (IReadOnlyList<string>?)s.SelectedRiders))
                .ToList();

            ValidateQuoteSpouseEntries(
                request.IssueDate,
                request.Frequency,
                pack,
                spouses);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static void ValidateQuoteSpouseEntries(
        DateOnly issueDate,
        string frequency,
        FlexiFutureRatePack pack,
        IReadOnlyList<(int SpouseIndex, DateOnly? DateOfBirth, string? Name, IReadOnlyList<string>? SelectedRiders)> spouses)
    {
        try
        {
            if (spouses.Count == 0)
                return;

            var settings = pack.Settings;
            if (spouses.Count > settings.MaxSpouses)
            {
                throw new InvalidOperationException($"Maximum {settings.MaxSpouses} spouses allowed.");
            }

            var premiumMode = RateLookup.ResolvePremiumMode(frequency);
            var seenIndices = new HashSet<int>();

            foreach (var spouse in spouses)
            {
                if (spouse.SpouseIndex < 1 || spouse.SpouseIndex > settings.MaxSpouses)
                {
                    throw new InvalidOperationException(
                        $"spouses[].spouseIndex must be between 1 and {settings.MaxSpouses}.");
                }

                if (!seenIndices.Add(spouse.SpouseIndex))
                {
                    throw new InvalidOperationException(
                        $"Duplicate spouses[].spouseIndex '{spouse.SpouseIndex}'.");
                }

                var label = string.IsNullOrWhiteSpace(spouse.Name)
                    ? $"Spouse {spouse.SpouseIndex}"
                    : spouse.Name.Trim();

                if (!spouse.DateOfBirth.HasValue)
                {
                    throw new InvalidOperationException(
                        $"spouses[].dateOfBirth is required for '{label}' (spouseIndex {spouse.SpouseIndex}).");
                }

                var dateOfBirth = spouse.DateOfBirth.Value;
                if (dateOfBirth > issueDate)
                {
                    throw new InvalidOperationException(
                        $"spouses[].dateOfBirth for '{label}' cannot be after the issue date.");
                }

                var anb = ComputeAnb(dateOfBirth, issueDate);
                if (anb < settings.MinEntryAge || anb > settings.MaxEntryAge)
                {
                    throw new InvalidOperationException(
                        $"Spouse '{label}' ANB must be between {settings.MinEntryAge} and {settings.MaxEntryAge} (got {anb}).");
                }

                foreach (var code in spouse.SelectedRiders ?? Array.Empty<string>())
                {
                    if (string.Equals(code, FlexiFutureRateCodes.ChildLe, StringComparison.OrdinalIgnoreCase))
                        continue;

                    var table = RateLookup.GetTable(pack, code, premiumMode)
                                ?? RateLookup.GetTable(pack, code, FlexiFuturePremiumModes.Regular);
                    if (table != null)
                        ValidateRiderAge(table, anb);
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    private decimal ResolveSumAssuredForCoverPremiums(
        FlexiFutureCoverPremiumsRequestDto request,
        FlexiFutureRatePack pack,
        int anb,
        Dictionary<string, decimal> solveCache)
    {
        try
        {
            return ResolveCoverScenarioSumAssured(request, pack, anb, extraCode: null, solveCache);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Sum assured for one cover scenario: covers already selected, plus <paramref name="extraCode"/> when a
    /// menu row is priced as an add-on. For PremiumToSa protection is funded from the premium budget, so each
    /// scenario solves its own sum assured — the same one the quote will report once that cover is switched on.
    /// </summary>
    private decimal ResolveCoverScenarioSumAssured(
        FlexiFutureCoverPremiumsRequestDto request,
        FlexiFutureRatePack pack,
        int anb,
        string? extraCode,
        Dictionary<string, decimal> solveCache)
    {
        try
        {
            if (string.Equals(request.CalculationMode, FlexiFutureCalculationModes.SaToPremium, StringComparison.OrdinalIgnoreCase))
            {
                if (!request.TargetSumAssured.HasValue || request.TargetSumAssured <= 0)
                    throw new InvalidOperationException("TargetSumAssured is required for SaToPremium.");
                return request.TargetSumAssured.Value;
            }

            if (!request.TargetPremium.HasValue || request.TargetPremium <= 0)
                throw new InvalidOperationException("TargetPremium is required for PremiumToSa.");

            var solveRequest = BuildCoverScenarioRequest(request, extraCode);
            var key = BuildCoverScenarioKey(solveRequest);
            if (solveCache.TryGetValue(key, out var cached))
                return cached;

            var solved = SolveSumAssuredFromPremium(solveRequest, pack, anb);
            solveCache[key] = solved;
            return solved;
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static FlexiFutureComputeRequestDto BuildCoverScenarioRequest(
        FlexiFutureCoverPremiumsRequestDto request,
        string? extraCode)
    {
        try
        {
            var mainSelected = new HashSet<string>(
                request.SelectedRiders ?? new List<string>(),
                StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrWhiteSpace(extraCode))
                mainSelected.Add(extraCode);
            mainSelected.Remove(FlexiFutureRateCodes.ChildLe);

            // LIFE only carries a premium when a death benefit percentage applies.
            var deathBenefitPct = mainSelected.Contains(FlexiFutureRateCodes.Life)
                ? request.DeathBenefitPct
                : 0m;

            return new FlexiFutureComputeRequestDto
            {
                CalculationMode = request.CalculationMode,
                IssueDate = request.IssueDate,
                PolicyTerm = request.PolicyTerm,
                Frequency = request.Frequency,
                DeathBenefitPct = deathBenefitPct,
                MaturityBenefitPayments = request.MaturityBenefitPayments <= 0 ? 1 : request.MaturityBenefitPayments,
                TargetPremium = request.TargetPremium,
                TargetSumAssured = request.TargetSumAssured,
                Client = new FlexiFutureClientDto
                {
                    ClientName = "CoverPremiumPreview",
                    DateOfBirth = request.Client.DateOfBirth,
                    Gender = request.Client.Gender,
                    PhoneNumber = "0000000000"
                },
                SelectedRiders = mainSelected.ToList(),
                Spouses = (request.Spouses ?? new List<FlexiFutureCoverPremiumsSpouseDto>())
                    .Where(s => s.DateOfBirth.HasValue)
                    .Select(s =>
                    {
                        var spouseSelected = new HashSet<string>(
                            s.SelectedRiders ?? new List<string>(),
                            StringComparer.OrdinalIgnoreCase);
                        if (!string.IsNullOrWhiteSpace(extraCode))
                            spouseSelected.Add(extraCode);

                        return new FlexiFutureSpouseDto
                        {
                            SpouseIndex = s.SpouseIndex,
                            DateOfBirth = s.DateOfBirth,
                            Gender = DeriveSpouseGender(request.Client.Gender),
                            SelectedRiders = spouseSelected.ToList()
                        };
                    })
                    .ToList(),
                Children = new List<FlexiFutureChildDto>()
            };
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static string BuildCoverScenarioKey(FlexiFutureComputeRequestDto solveRequest)
    {
        try
        {
            var main = string.Join(
                ",",
                solveRequest.SelectedRiders.OrderBy(c => c, StringComparer.OrdinalIgnoreCase));
            var spouses = string.Join(
                ";",
                solveRequest.Spouses.Select(s =>
                    $"{s.SpouseIndex}:{string.Join(",", s.SelectedRiders.OrderBy(c => c, StringComparer.OrdinalIgnoreCase))}"));
            return $"{solveRequest.DeathBenefitPct}|{main}|{spouses}";
        }
        catch (Exception)
        {
            throw;
        }
    }

    private List<FlexiFutureCoverPremiumDto> BuildCoverPremiumMenu(
        FlexiFutureCoverPremiumsRequestDto request,
        FlexiFutureRatePack pack,
        int mainAnb,
        string premiumMode,
        FlexiFutureFrequencyDiscount freq,
        Dictionary<string, decimal> solveCache)
    {
        try
        {
            var isSingle = string.Equals(premiumMode, FlexiFuturePremiumModes.Single, StringComparison.OrdinalIgnoreCase);
            var spouses = (request.Spouses ?? new List<FlexiFutureCoverPremiumsSpouseDto>())
                .Where(s => s.DateOfBirth.HasValue)
                .ToList();
            var mainSelected = new HashSet<string>(
                request.SelectedRiders ?? new List<string>(),
                StringComparer.OrdinalIgnoreCase);
            var ciCoverPct = pack.Tables
                .FirstOrDefault(t => t.Code == FlexiFutureRateCodes.Ci && t.PremiumMode == premiumMode)
                ?.DefaultCoverFactor ?? pack.Settings.CiCoverPct;

            var covers = new List<FlexiFutureCoverPremiumDto>();

            foreach (var table in pack.Tables
                         .Where(t => t.IsRider && t.PremiumMode == premiumMode && t.AppliesToMain)
                         .OrderBy(t => t.SortOrder))
            {
                if (string.Equals(table.Code, FlexiFutureRateCodes.ChildLe, StringComparison.OrdinalIgnoreCase))
                    continue;

                var cover = new FlexiFutureCoverPremiumDto
                {
                    Code = table.Code,
                    Name = table.Name,
                    Selected = mainSelected.Contains(table.Code)
                };

                if (isSingle && !table.AvailableForSinglePremium)
                {
                    cover.Available = false;
                    cover.UnavailableReason = $"{table.Name} is not available for single premium.";
                    covers.Add(cover);
                    continue;
                }

                try
                {
                    // Price this row on the sum assured that applies once the cover is switched on.
                    var sumAssured = ResolveCoverScenarioSumAssured(request, pack, mainAnb, table.Code, solveCache);
                    var spouseSa = CapSpouseSumAssured(sumAssured, pack.Settings.SpouseSaCap);
                    cover.SumAssuredIfSelected = RoundUpAwayFromZero(sumAssured);

                    if (!TryValidateRiderAge(table, mainAnb, out var mainAgeReason))
                    {
                        cover.Available = false;
                        cover.UnavailableReason = mainAgeReason;
                    }
                    else
                    {
                        cover.MainPremium = RoundUpPremium(ApplyFrequencyAndDiscount(
                            ComputeRiderMonthly(
                                table,
                                pack,
                                premiumMode,
                                mainAnb,
                                sumAssured,
                                request.DeathBenefitPct,
                                request.PolicyTerm),
                            freq));
                    }

                    if (table.AppliesToSpouse && spouses.Count > 0)
                    {
                        foreach (var spouse in spouses)
                        {
                            var spouseAnb = ComputeAnb(spouse.DateOfBirth!.Value, request.IssueDate);
                            var spouseLine = new FlexiFutureCoverSpousePremiumDto
                            {
                                SpouseIndex = spouse.SpouseIndex,
                                Anb = spouseAnb
                            };

                            if (!TryValidateRiderAge(table, spouseAnb, out var spouseAgeReason))
                            {
                                spouseLine.Available = false;
                                spouseLine.UnavailableReason = spouseAgeReason;
                            }
                            else
                            {
                                var coverAmount = ResolveSpouseRiderCover(
                                    table.Code,
                                    sumAssured,
                                    spouseSa,
                                    pack.Settings.SpouseSaCap,
                                    ciCoverPct,
                                    request.DeathBenefitPct);
                                var rate = RateLookup.LookupAgeTerm(
                                    pack,
                                    table.Code,
                                    premiumMode,
                                    spouseAnb,
                                    request.PolicyTerm);
                                var monthly = rate * (coverAmount / 1000m);
                                spouseLine.Premium = RoundUpPremium(ApplyFrequencyAndDiscount(monthly, freq));
                            }

                            cover.Spouses.Add(spouseLine);
                        }

                        cover.SpousesPremiumTotal = RoundUpAwayFromZero(
                            cover.Spouses.Where(s => s.Available).Sum(s => s.Premium));
                    }
                }
                catch (InvalidOperationException ex)
                {
                    // One un-priceable cover (e.g. no rate for the chosen term) must not fail the whole menu.
                    _logger.LogWarning(
                        ex,
                        "FlexiFuture cover {Code} could not be priced for term {Term}.",
                        table.Code,
                        request.PolicyTerm);
                    cover.Available = false;
                    cover.UnavailableReason = $"{table.Name} is not available for the selected term.";
                    cover.MainPremium = 0;
                    cover.SumAssuredIfSelected = 0;
                    cover.Spouses.Clear();
                    cover.SpousesPremiumTotal = 0;
                }

                cover.CumulativePremium = RoundUpAwayFromZero(cover.MainPremium + cover.SpousesPremiumTotal);
                covers.Add(cover);
            }

            return covers;
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static bool TryValidateRiderAge(FlexiFutureRateTable table, int anb, out string? reason)
    {
        try
        {
            reason = null;
            if (table.MaxCoverageAge.HasValue && anb > table.MaxCoverageAge.Value)
            {
                reason = $"{table.Name} is not available above ANB {table.MaxCoverageAge}.";
                return false;
            }

            if (table.MinEntryAge.HasValue && anb < table.MinEntryAge.Value)
            {
                reason = $"{table.Name} requires minimum ANB {table.MinEntryAge}.";
                return false;
            }

            return true;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public FlexiFutureComputeResultDto BuildResult(
        FlexiFutureComputeRequestDto request,
        FlexiFutureRatePack pack,
        int anb,
        decimal sumAssured)
    {
        try
        {
            var premiumMode = RateLookup.ResolvePremiumMode(request.Frequency);
            var freq = RateLookup.GetFrequency(pack, request.Frequency);
            var settings = pack.Settings;

            var savingsMonthly = ComputeSavingsMonthlyEquivalent(sumAssured, anb, request.PolicyTerm, premiumMode, pack);
            var savingsPremium = RoundUpPremium(ApplyFrequencyAndDiscount(savingsMonthly, freq));

            var mainRiders = ComputeMainRiders(request, pack, anb, sumAssured, premiumMode, freq);
            var spouseResults = ComputeSpouses(request, pack, sumAssured, premiumMode, freq);
            var childResults = ComputeChildren(request, pack, premiumMode, freq);

            var ridersTotal = mainRiders.Where(r => r.Selected).Sum(r => r.Premium)
                              + spouseResults.Sum(s => s.PremiumTotal)
                              + childResults.Sum(c => c.Premium);

            var exclPhcl = savingsPremium + ridersTotal;
            var phcl = ComputePhcl(exclPhcl, settings.PhclRate);
            var total = RoundUpPremium(exclPhcl + phcl);

            var maturityDate = request.IssueDate.AddYears(request.PolicyTerm);
            var maturity = ComputeMaturitySchedule(
                sumAssured,
                request.PolicyTerm,
                request.MaturityBenefitPayments,
                maturityDate,
                settings.MaturityEscalationRate);
            var taxRelief = ComputeTaxReliefIllustration(savingsPremium, request.Frequency, request.PolicyTerm, settings);
            var requiresApproval = sumAssured > settings.ApprovalRequiredAboveSa;
            var childLeName = RateLookup.GetTable(pack, FlexiFutureRateCodes.ChildLe, premiumMode)?.Name
                              ?? "Child Last Expense";
            var cumulativeRiders = BuildCumulativeRiderBreakdown(
                mainRiders,
                spouseResults,
                childResults,
                childLeName);

            return new FlexiFutureComputeResultDto
            {
                Anb = anb,
                SumAssured = RoundUpAwayFromZero(sumAssured),
                MonthlyEquivalent = RoundUpAwayFromZero(savingsMonthly),
                SavingsPremium = savingsPremium,
                RidersTotal = RoundUpAwayFromZero(ridersTotal),
                Phcl = phcl,
                TotalPremiumPayable = total,
                RequiresApproval = requiresApproval,
                MaturityDate = maturityDate,
                Warning = requiresApproval
                    ? $"Sum assured is above the limit of KES {settings.MaxCoverPerLife:N0} and requires approval."
                    : null,
                RiderBreakdown = mainRiders,
                CumulativeRiderBreakdown = cumulativeRiders,
                MaturitySchedule = maturity,
                SurrenderSchedule = new List<FlexiFutureSurrenderYearDto>(),
                TaxRelief = taxRelief,
                Spouses = spouseResults,
                Children = childResults
            };
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Aggregates main + spouse (+ child LE) premiums per rider code with <see cref="FlexiFutureLifeRoles.All"/>.
    /// </summary>
    public static List<FlexiFuturePremiumLineDto> BuildCumulativeRiderBreakdown(
        List<FlexiFuturePremiumLineDto> mainRiders,
        List<FlexiFutureSpouseResultDto> spouses,
        List<FlexiFutureChildResultDto> children,
        string childLeName = "Child Last Expense")
    {
        try
        {
            var lines = new List<FlexiFuturePremiumLineDto>();
            var spouseLines = spouses.SelectMany(s => s.Riders).ToList();

            foreach (var main in mainRiders)
            {
                var spousePremium = spouseLines
                    .Where(r => string.Equals(r.Code, main.Code, StringComparison.OrdinalIgnoreCase))
                    .Sum(r => r.Premium);
                var spouseSelected = spouseLines.Any(r =>
                    string.Equals(r.Code, main.Code, StringComparison.OrdinalIgnoreCase) && r.Selected);

                lines.Add(new FlexiFuturePremiumLineDto
                {
                    Code = main.Code,
                    Name = main.Name,
                    LifeRole = FlexiFutureLifeRoles.All,
                    Selected = main.Selected || spouseSelected,
                    Premium = RoundUpAwayFromZero(main.Premium + spousePremium)
                });
            }

            var childPremium = children.Sum(c => c.Premium);
            if (children.Count > 0)
            {
                lines.Add(new FlexiFuturePremiumLineDto
                {
                    Code = FlexiFutureRateCodes.ChildLe,
                    Name = childLeName,
                    LifeRole = FlexiFutureLifeRoles.All,
                    Selected = childPremium > 0,
                    Premium = RoundUpAwayFromZero(childPremium)
                });
            }

            return lines;
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// PremiumToSa product premium only (savings + riders). Levy is ROUNDUP'd on the target and deducted first.
    /// </summary>
    private decimal ComputePremiumExcludingPhcl(
        FlexiFutureComputeRequestDto request,
        FlexiFutureRatePack pack,
        int anb,
        decimal sumAssured)
    {
        try
        {
            var premiumMode = RateLookup.ResolvePremiumMode(request.Frequency);
            var freq = RateLookup.GetFrequency(pack, request.Frequency);

            var savingsMonthly = ComputeSavingsMonthlyEquivalent(sumAssured, anb, request.PolicyTerm, premiumMode, pack);
            var savingsPremium = RoundUpPremium(ApplyFrequencyAndDiscount(savingsMonthly, freq));

            var mainRiders = ComputeMainRiders(request, pack, anb, sumAssured, premiumMode, freq);
            var spouseResults = ComputeSpouses(request, pack, sumAssured, premiumMode, freq);
            var childResults = ComputeChildren(request, pack, premiumMode, freq);

            var ridersTotal = mainRiders.Where(r => r.Selected).Sum(r => r.Premium)
                              + spouseResults.Sum(s => s.PremiumTotal)
                              + childResults.Sum(c => c.Premium);

            return savingsPremium + ridersTotal;
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// ROUNDUP levy implied by the payable target (excl ≈ target/(1+rate)), then deduct for product budget.
    /// Matches forward PHCL = ROUNDUP(rate × (savings+riders)).
    /// </summary>
    public static decimal ResolveProductTargetAfterLevy(decimal targetPremium, decimal phclRate)
    {
        try
        {
            return targetPremium - ResolveLevyOnTarget(targetPremium, phclRate);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public static decimal ResolveLevyOnTarget(decimal targetPremium, decimal phclRate)
    {
        try
        {
            if (phclRate <= 0m)
                return 0m;
            var impliedExcl = targetPremium / (1m + phclRate);
            return RoundUpPremium(phclRate * impliedExcl);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static void ApplyPremiumToSaLevyDeduction(
        FlexiFutureComputeResultDto result,
        decimal targetPremium,
        decimal phclRate)
    {
        try
        {
            var levy = ResolveLevyOnTarget(targetPremium, phclRate);
            result.Phcl = levy;
            result.TotalPremiumPayable = result.SavingsPremium + result.RidersTotal + levy;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public static int ComputeAnb(DateOnly dateOfBirth, DateOnly issueDate)
    {
        try
        {
            var years = issueDate.Year - dateOfBirth.Year;
            if (issueDate < dateOfBirth.AddYears(years))
                years--;
            return years + 1;
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Child Age Next Birthday. Infants (attained age 0) always rate at ANB 1.
    /// </summary>
    public static int ComputeChildAnb(DateOnly dateOfBirth, DateOnly issueDate)
    {
        try
        {
            // Age 0 (not yet 1 last birthday) → Age Next Birthday is 1.
            // Also clamps a same-day/timezone edge that can yield ANB 0 when DOB is slightly after issue.
            return Math.Max(1, ComputeAnb(dateOfBirth, issueDate));
        }
        catch (Exception)
        {
            throw;
        }
    }

    public static decimal RoundUpPremium(decimal value)
    {
        try
        {
            return Math.Ceiling(value);
        }
        catch (Exception)
        {
            throw;
        }
    }
    
    public static decimal RoundUpAwayFromZero(decimal value)
    {
        try
        {
            return value >= 0 ? Math.Ceiling(value) : Math.Floor(value);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public static decimal ApplyFrequencyFactor(decimal monthlyEquivalent, FlexiFutureFrequencyDiscount freq)
    {
        try
        {
            if (string.Equals(freq.Frequency, FlexiFutureFrequencies.Monthly, StringComparison.OrdinalIgnoreCase)
                || string.Equals(freq.Frequency, FlexiFutureFrequencies.Single, StringComparison.OrdinalIgnoreCase))
            {
                return monthlyEquivalent;
            }

            var factor = freq.MonthFactor;
            if (freq.UsesElevenTwelfths)
                return monthlyEquivalent * factor * 11m / 12m;

            return monthlyEquivalent * factor;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public static decimal ApplyFrequencyDiscount(decimal premium, FlexiFutureFrequencyDiscount freq)
    {
        try
        {
            return premium * (1m - freq.DiscountRate);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public static decimal ApplyFrequencyAndDiscount(decimal monthlyEquivalent, FlexiFutureFrequencyDiscount freq)
    {
        try
        {
            return ApplyFrequencyDiscount(ApplyFrequencyFactor(monthlyEquivalent, freq), freq);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public static decimal ComputePhcl(decimal totalExclPhcl, decimal phclRate)
    {
        try
        {
            return RoundUpPremium(phclRate * totalExclPhcl);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public static decimal CapSpouseSumAssured(decimal mainSa, decimal spouseCap)
    {
        try
        {
            return Math.Min(mainSa, spouseCap);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Excel Client Schedule E20: MIN(DTH_Ben * sum_assured, 1000000) — spouse death sum assured.
    /// </summary>
    public static decimal ResolveSpouseLifeCover(decimal mainSa, decimal spouseCap, decimal deathBenefitPct)
    {
        try
        {
            return Math.Min(deathBenefitPct * mainSa, spouseCap);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Excel Riders_Spouse CI: MIN(0.5 * sum_assured, 1000000).
    /// Cap applies to the CI benefit (50% of main SA), not to SA before the 50%.
    /// </summary>
    public static decimal ResolveSpouseCiCover(decimal mainSa, decimal spouseCap, decimal ciCoverPct)
    {
        try
        {
            return Math.Min(ciCoverPct * mainSa, spouseCap);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Cover amount used in spouse rider premium: rate × cover / 1000.
    /// LIFE matches Excel Riders_Spouse!G7: DTH_Ben * sum_assured_spouse
    /// where sum_assured_spouse = MIN(DTH_Ben * main SA, spouseCap).
    /// CI: MIN(ci% × main SA, spouseCap). PTD: capped spouse SA.
    /// </summary>
    public static decimal ResolveSpouseRiderCover(
        string riderCode,
        decimal mainSa,
        decimal spouseSa,
        decimal spouseCap,
        decimal ciCoverPct,
        decimal deathBenefitPct)
    {
        try
        {
            if (string.Equals(riderCode, FlexiFutureRateCodes.Ci, StringComparison.OrdinalIgnoreCase))
                return ResolveSpouseCiCover(mainSa, spouseCap, ciCoverPct);

            if (string.Equals(riderCode, FlexiFutureRateCodes.Life, StringComparison.OrdinalIgnoreCase))
            {
                // Excel G7: rate * F7 * sum_assured_spouse / 1000, F7 = DTH_Ben.
                return deathBenefitPct * ResolveSpouseLifeCover(mainSa, spouseCap, deathBenefitPct);
            }

            return spouseSa;
        }
        catch (Exception)
        {
            throw;
        }
    }

    private decimal ComputeSavingsMonthlyEquivalent(
        decimal sumAssured,
        int anb,
        int term,
        string premiumMode,
        FlexiFutureRatePack pack)
    {
        try
        {
            var saRate = RateLookup.LookupSaBandTerm(pack, FlexiFutureRateCodes.Savings, premiumMode, sumAssured, term);
            var ageLoading = RateLookup.LookupAgeOnly(pack, FlexiFutureRateCodes.WopDeathLoading, premiumMode, anb);
            return (saRate + ageLoading) * (sumAssured / 1000m);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private List<FlexiFuturePremiumLineDto> ComputeMainRiders(
        FlexiFutureComputeRequestDto request,
        FlexiFutureRatePack pack,
        int anb,
        decimal sumAssured,
        string premiumMode,
        FlexiFutureFrequencyDiscount freq)
    {
        try
        {
            var selected = ResolveMainSelectedRiders(request);
            var isSingle = string.Equals(premiumMode, FlexiFuturePremiumModes.Single, StringComparison.OrdinalIgnoreCase);
            var lines = new List<FlexiFuturePremiumLineDto>();

            foreach (var table in pack.Tables.Where(t => t.IsRider && t.PremiumMode == premiumMode && t.AppliesToMain)
                         .OrderBy(t => t.SortOrder))
            {
                var code = table.Code;
                // CHILD_LE is priced per child, never as a main-life rider line.
                if (string.Equals(code, FlexiFutureRateCodes.ChildLe, StringComparison.OrdinalIgnoreCase))
                    continue;

                // LIFE is implied by deathBenefitPct > 0 even when not in selectedRiders.
                var isLife = string.Equals(code, FlexiFutureRateCodes.Life, StringComparison.OrdinalIgnoreCase);
                var isSelected = isLife
                    ? request.DeathBenefitPct > 0
                    : selected.Contains(code);

                if (isSingle && !table.AvailableForSinglePremium)
                {
                    lines.Add(new FlexiFuturePremiumLineDto
                    {
                        Code = code,
                        Name = table.Name,
                        LifeRole = FlexiFutureLifeRoles.Main,
                        Selected = false,
                        Premium = 0
                    });
                    continue;
                }

                decimal premium = 0;
                if (isSelected)
                {
                    ValidateRiderAge(table, anb);
                    premium = RoundUpPremium(ApplyFrequencyAndDiscount(
                        ComputeRiderMonthly(table, pack, premiumMode, anb, sumAssured, request.DeathBenefitPct, request.PolicyTerm),
                        freq));
                }

                lines.Add(new FlexiFuturePremiumLineDto
                {
                    Code = code,
                    Name = table.Name,
                    LifeRole = FlexiFutureLifeRoles.Main,
                    Selected = isSelected,
                    Premium = premium
                });
            }

            return lines;
        }
        catch (Exception)
        {
            throw;
        }
    }
    private static HashSet<string> ResolveMainSelectedRiders(FlexiFutureComputeRequestDto request)
    {
        try
        {
            var selected = new HashSet<string>(
                request.SelectedRiders ?? new List<string>(),
                StringComparer.OrdinalIgnoreCase);
            selected.Remove(FlexiFutureRateCodes.ChildLe);
            if (request.DeathBenefitPct > 0)
                selected.Add(FlexiFutureRateCodes.Life);
            return selected;
        }
        catch (Exception)
        {
            throw;
        }
    }

    private decimal ComputeRiderMonthly(
        FlexiFutureRateTable table,
        FlexiFutureRatePack pack,
        string premiumMode,
        int anb,
        decimal sumAssured,
        decimal deathBenefitPct,
        int term)
    {
        try
        {
            var rate = RateLookup.LookupAgeTerm(pack, table.Code, premiumMode, anb, term);
            var coverFactor = table.Code switch
            {
                FlexiFutureRateCodes.Life => deathBenefitPct,
                FlexiFutureRateCodes.Ci => table.DefaultCoverFactor ?? pack.Settings.CiCoverPct,
                _ => table.DefaultCoverFactor ?? 1m
            };
            return rate * coverFactor * (sumAssured / 1000m);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private List<FlexiFutureSpouseResultDto> ComputeSpouses(
        FlexiFutureComputeRequestDto request,
        FlexiFutureRatePack pack,
        decimal mainSa,
        string premiumMode,
        FlexiFutureFrequencyDiscount freq)
    {
        try
        {
            var results = new List<FlexiFutureSpouseResultDto>();
            if (request.Spouses == null || request.Spouses.Count == 0)
                return results;

            var isSingle = string.Equals(premiumMode, FlexiFuturePremiumModes.Single, StringComparison.OrdinalIgnoreCase);

            // Spouse riders are only priced when the same rider is on the main member.
            // LIFE counts as main-selected whenever deathBenefitPct > 0.
            var mainSelected = ResolveMainSelectedRiders(request);

            foreach (var spouse in request.Spouses.Where(s => s.DateOfBirth.HasValue))
            {
                var anb = ComputeAnb(spouse.DateOfBirth!.Value, request.IssueDate);
                var spouseSa = CapSpouseSumAssured(mainSa, pack.Settings.SpouseSaCap);
                var ciCoverPct = pack.Tables
                    .FirstOrDefault(t => t.Code == FlexiFutureRateCodes.Ci && t.PremiumMode == premiumMode)
                    ?.DefaultCoverFactor ?? pack.Settings.CiCoverPct;
                var spouseSelected = new HashSet<string>(
                    spouse.SelectedRiders ?? new List<string>(),
                    StringComparer.OrdinalIgnoreCase);
                var lines = new List<FlexiFuturePremiumLineDto>();
                decimal total = 0;

                foreach (var table in pack.Tables.Where(t => t.IsRider && t.PremiumMode == premiumMode && t.AppliesToSpouse)
                             .OrderBy(t => t.SortOrder))
                {
                    // Disregard spouse riders that were not also selected for main.
                    var isSelected = spouseSelected.Contains(table.Code)
                                     && mainSelected.Contains(table.Code);
                    decimal premium = 0;
                    if (isSelected && !(isSingle && !table.AvailableForSinglePremium))
                    {
                        ValidateRiderAge(table, anb);
                        // Excel: CI uses MIN(0.5*main_SA, cap); LIFE/PTD use sum_assured_spouse.
                        var cover = ResolveSpouseRiderCover(
                            table.Code,
                            mainSa,
                            spouseSa,
                            pack.Settings.SpouseSaCap,
                            ciCoverPct,
                            request.DeathBenefitPct);
                        var rate = RateLookup.LookupAgeTerm(pack, table.Code, premiumMode, anb, request.PolicyTerm);
                        var monthly = rate * (cover / 1000m);
                        premium = RoundUpPremium(ApplyFrequencyAndDiscount(monthly, freq));
                        total += premium;
                    }

                    lines.Add(new FlexiFuturePremiumLineDto
                    {
                        Code = table.Code,
                        Name = table.Name,
                        LifeRole = spouse.SpouseIndex == 2 ? FlexiFutureLifeRoles.Spouse2 : FlexiFutureLifeRoles.Spouse1,
                        Selected = isSelected,
                        Premium = premium
                    });
                }

                results.Add(new FlexiFutureSpouseResultDto
                {
                    SpouseIndex = spouse.SpouseIndex,
                    Name = spouse.Name,
                    DateOfBirth = spouse.DateOfBirth,
                    Anb = anb,
                    SumAssured = RoundUpAwayFromZero(spouseSa),
                    PremiumTotal = total,
                    Riders = lines
                });
            }

            return results;
        }
        catch (Exception)
        {
            throw;
        }
    }

    private List<FlexiFutureChildResultDto> ComputeChildren(
        FlexiFutureComputeRequestDto request,
        FlexiFutureRatePack pack,
        string premiumMode,
        FlexiFutureFrequencyDiscount freq)
    {
        try
        {
            var results = new List<FlexiFutureChildResultDto>();
            if (request.Children == null || request.Children.Count == 0)
                return results;

            // CHILD_LE is allowed only with death cover (deathBenefitPct > 0).
            // If deathBenefitPct <= 0, disregard even when CHILD_LE is in selectedRiders.
            var isSingle = string.Equals(premiumMode, FlexiFuturePremiumModes.Single, StringComparison.OrdinalIgnoreCase);
            if (isSingle || request.DeathBenefitPct <= 0)
                return results;

            var table = RateLookup.GetTable(pack, FlexiFutureRateCodes.ChildLe, premiumMode);
            if (table == null)
                return results;

            foreach (var child in request.Children.Where(c => c.DateOfBirth.HasValue))
            {
                var anb = ComputeChildAnb(child.DateOfBirth!.Value, request.IssueDate);
                ValidateChildAnb(anb, child.Name);
                var sa = pack.Settings.ChildLastExpenseSa;
                var rate = RateLookup.LookupAgeTerm(pack, FlexiFutureRateCodes.ChildLe, premiumMode, anb, request.PolicyTerm);
                var monthly = rate * (sa / 1000m);
                var premium = RoundUpPremium(ApplyFrequencyAndDiscount(monthly, freq));
                results.Add(new FlexiFutureChildResultDto
                {
                    Name = child.Name,
                    DateOfBirth = child.DateOfBirth,
                    Anb = anb,
                    SumAssured = RoundUpAwayFromZero(sa),
                    Premium = premium
                });
            }

            return results;
        }
        catch (Exception)
        {
            throw;
        }
    }

    private decimal SolveSumAssuredFromPremium(
        FlexiFutureComputeRequestDto request,
        FlexiFutureRatePack pack,
        int anb)
    {
        try
        {
            var target = request.TargetPremium!.Value;
            // ROUNDUP levy on the target first, then deduct — solve SA to the remaining product budget.
            var productTarget = ResolveProductTargetAfterLevy(target, pack.Settings.PhclRate);
            decimal low = pack.Settings.MinSumAssured;
            decimal high = pack.Settings.MaxCoverPerLife;
            decimal bestSa = low;
            decimal bestDiff = decimal.MaxValue;

            for (var i = 0; i < 80; i++)
            {
                // Whole-shilling SA is the final unit; stop once the search window is sub-shilling.
                if (high - low < 1m)
                    break;

                var mid = (low + high) / 2m;
                var product = ComputePremiumExcludingPhcl(request, pack, anb, mid);
                var diff = Math.Abs(product - productTarget);
                if (diff < bestDiff)
                {
                    bestDiff = diff;
                    bestSa = mid;
                }

                if (diff == 0m)
                {
                    bestSa = mid;
                    bestDiff = 0m;
                    break;
                }

                if (product > productTarget)
                    high = mid;
                else
                    low = mid;
            }

            // ROUNDUP makes premium flat across a range of whole-shilling SAs.
            var center = Math.Round(bestSa, 0, MidpointRounding.AwayFromZero);
            var seedProduct = ComputePremiumExcludingPhcl(request, pack, anb, center);
            var plateauProduct = seedProduct;
            if (bestDiff == 0m || Math.Abs(seedProduct - productTarget) <= Math.Abs(bestDiff))
            {
                var roundedBest = Math.Round(bestSa, 0, MidpointRounding.AwayFromZero);
                var atBest = roundedBest == center
                    ? seedProduct
                    : ComputePremiumExcludingPhcl(request, pack, anb, roundedBest);
                if (Math.Abs(atBest - productTarget) < Math.Abs(plateauProduct - productTarget))
                {
                    center = roundedBest;
                    plateauProduct = atBest;
                }
            }

            decimal plateauSa = center;
            var plateauDiff = Math.Abs(plateauProduct - productTarget);
            if (plateauDiff > 0m)
            {
                for (var delta = -200; delta <= 200; delta++)
                {
                    if (delta == 0)
                        continue;
                    var candidate = center + delta;
                    if (candidate < pack.Settings.MinSumAssured || candidate > pack.Settings.MaxCoverPerLife)
                        continue;
                    var product = ComputePremiumExcludingPhcl(request, pack, anb, candidate);
                    var diff = Math.Abs(product - productTarget);
                    if (diff < plateauDiff)
                    {
                        plateauDiff = diff;
                        plateauProduct = product;
                        plateauSa = candidate;
                        if (plateauDiff == 0m)
                            break;
                    }
                }
            }

            if (plateauDiff > 1m)
                throw new InvalidOperationException("Chosen terms of the policy are not within the product rules (unable to solve sum assured).");

            // Every sum assured on the plateau costs the same premium, so return the highest one —
            // the most cover the premium can buy.
            return FindHighestPlateauSumAssured(request, pack, anb, plateauSa, plateauProduct);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Highest sum assured that still prices to <paramref name="plateauProduct"/> (product premium excl. levy);
    /// same result as walking up in steps of 1 from <paramref name="knownSa"/>.
    /// </summary>
    private decimal FindHighestPlateauSumAssured(
        FlexiFutureComputeRequestDto request,
        FlexiFutureRatePack pack,
        int anb,
        decimal knownSa,
        decimal plateauProduct)
    {
        try
        {
            var maxSa = pack.Settings.MaxCoverPerLife;
            var good = knownSa;
            var step = 1m;

            while (true)
            {
                var probe = good + step;
                if (probe > maxSa)
                    break;
                if (ComputePremiumExcludingPhcl(request, pack, anb, probe) != plateauProduct)
                    break;
                good = probe;
                step *= 2m;
            }

            // good is on the plateau; good + step is past its end (or past the cap).
            var lo = good;
            var hi = Math.Min(maxSa, good + step);
            while (lo < hi)
            {
                var mid = Math.Ceiling((lo + hi) / 2m);
                if (ComputePremiumExcludingPhcl(request, pack, anb, mid) == plateauProduct)
                    lo = mid;
                else
                    hi = mid - 1m;
            }

            return lo;
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static List<FlexiFutureMaturityPaymentDto> ComputeMaturitySchedule(
        decimal sumAssured,
        int policyTerm,
        int benTerm,
        DateOnly maturityDate,
        decimal escalationRate)
    {
        try
        {
            benTerm = Math.Max(1, benTerm);
            var payments = new List<FlexiFutureMaturityPaymentDto>();
            for (var i = 0; i < benTerm; i++)
            {
                // Excel: Polterm + SEQUENCE index - 1 → first payout year = policy term
                var amount = sumAssured / benTerm * (decimal)Math.Pow((double)(1 + escalationRate), i);
                payments.Add(new FlexiFutureMaturityPaymentDto
                {
                    PaymentYear = policyTerm + i,
                    PaymentDate = maturityDate.AddYears(i),
                    Amount = RoundUpAwayFromZero(amount)
                });
            }

            return payments;
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static FlexiFutureTaxReliefDto ComputeTaxReliefIllustration(
        decimal savingsPremium,
        string frequency,
        int policyTerm,
        FlexiFutureProductSettings settings)
    {
        try
        {
            var periodsPerYear = frequency.ToLowerInvariant() switch
            {
                "monthly" => 11m,
                "quarterly" => 4m,
                "semi-annual" => 2m,
                _ => 1m
            };
            var gross = periodsPerYear * savingsPremium * 1.0025m * (string.Equals(frequency, FlexiFutureFrequencies.Single, StringComparison.OrdinalIgnoreCase) ? 1m : policyTerm);
            var eligible = policyTerm >= settings.TaxReliefMinTerm;
            var relief = eligible
                ? -Math.Min(settings.TaxReliefRate * gross, settings.TaxReliefAnnualCap * policyTerm)
                : 0m;
            // Excel ROUNDUP(..., 0): away from zero to whole shillings.
            var grossRounded = RoundUpAwayFromZero(gross);
            var reliefRounded = RoundUpAwayFromZero(relief);
            return new FlexiFutureTaxReliefDto
            {
                GrossAnnualisedContribution = grossRounded,
                TaxReliefAmount = reliefRounded,
                PremiumAfterTaxRelief = RoundUpAwayFromZero(gross + relief),
                Eligible = eligible
            };
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static void ValidateProductRules(
        FlexiFutureComputeRequestDto request,
        FlexiFutureRatePack pack,
        int anb,
        decimal? sumAssured)
    {
        try
        {
            var s = pack.Settings;
            var today = DateOnly.FromDateTime(DateTime.Today);
            if (request.IssueDate < today)
                throw new InvalidOperationException("Issue date must be today or a future date.");

            if (request.PolicyTerm < s.MinTerm || request.PolicyTerm > s.MaxTerm)
                throw new InvalidOperationException($"Policy term must be between {s.MinTerm} and {s.MaxTerm}.");

            if (anb < s.MinEntryAge || anb > s.MaxEntryAge)
                throw new InvalidOperationException($"Client ANB must be between {s.MinEntryAge} and {s.MaxEntryAge}.");

            if (string.IsNullOrWhiteSpace(request.Client.PhoneNumber))
                throw new InvalidOperationException("PhoneNumber is required.");

            RateLookup.GetFrequency(pack, request.Frequency);

            if (sumAssured.HasValue)
                ValidateSaTermRules(s, sumAssured.Value, request.PolicyTerm);

            ValidateQuoteSpouses(request, pack);

            foreach (var child in request.Children ?? new List<FlexiFutureChildDto>())
            {
                if (!child.DateOfBirth.HasValue)
                    continue;
                var childAnb = ComputeChildAnb(child.DateOfBirth.Value, request.IssueDate);
                ValidateChildAnb(childAnb, child.Name);
            }

            foreach (var code in request.SelectedRiders ?? new List<string>())
            {
                if (string.Equals(code, FlexiFutureRateCodes.ChildLe, StringComparison.OrdinalIgnoreCase))
                    continue; // validated per child when deathBenefitPct > 0

                var mode = RateLookup.ResolvePremiumMode(request.Frequency);
                var table = RateLookup.GetTable(pack, code, mode)
                            ?? RateLookup.GetTable(pack, code, FlexiFuturePremiumModes.Regular);
                if (table != null)
                    ValidateRiderAge(table, anb);
            }

            if (request.DeathBenefitPct > 0)
            {
                var life = RateLookup.GetTable(pack, FlexiFutureRateCodes.Life, RateLookup.ResolvePremiumMode(request.Frequency));
                if (life != null)
                    ValidateRiderAge(life, anb);
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static void ValidateSaTermRules(FlexiFutureProductSettings settings, decimal sumAssured, int term)
    {
        try
        {
            if (sumAssured < settings.MinSumAssured)
                throw new InvalidOperationException(
                    $"Sum assured must be at least KES {settings.MinSumAssured:N0}.");

            if (sumAssured > settings.MaxCoverPerLife)
                throw new InvalidOperationException(
                    $"Sum assured must not exceed KES {settings.MaxCoverPerLife:N0}.");

            var minTerm = settings.MinTerm;
            if (sumAssured < settings.SaBandLow)
                minTerm = settings.MinTermBelowSaBandLow;
            else if (sumAssured < settings.SaBandMid)
                minTerm = settings.MinTermBelowSaBandMid;

            if (term < minTerm)
                throw new InvalidOperationException($"For sum assured {sumAssured:N0}, minimum policy term is {minTerm} years.");
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static void ValidateChildAnb(int anb, string? childName)
    {
        try
        {
            const int minAnb = 1;
            const int maxAnb = 21;
            if (anb < minAnb || anb > maxAnb)
            {
                var label = string.IsNullOrWhiteSpace(childName) ? "Child" : childName.Trim();
                throw new InvalidOperationException(
                    $"{label} ANB must be between {minAnb} and {maxAnb} (got {anb}).");
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static void ValidateRiderAge(FlexiFutureRateTable table, int anb)
    {
        try
        {
            if (table.MaxCoverageAge.HasValue && anb > table.MaxCoverageAge.Value)
                throw new InvalidOperationException($"{table.Name} is not available above ANB {table.MaxCoverageAge}.");
            if (table.MinEntryAge.HasValue && anb < table.MinEntryAge.Value)
                throw new InvalidOperationException($"{table.Name} requires minimum ANB {table.MinEntryAge}.");
        }
        catch (Exception)
        {
            throw;
        }
    }
}


