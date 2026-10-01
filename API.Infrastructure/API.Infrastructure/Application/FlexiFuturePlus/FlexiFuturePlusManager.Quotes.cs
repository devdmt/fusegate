using System.Text.Json;
using System.Text.RegularExpressions;
using DAL.Model.FlexiFuture;
using DAL.Model.FlexiFund;
using DAL.Model.Shared;
using DAL.ModelView;
using DAL.ModelView.FlexiFuture;
using DAL.ModelView.FlexiFuturePlus;
using Microsoft.EntityFrameworkCore;

namespace API.Infrastructure.Application.FlexiFuturePlus;

public partial class FlexiFuturePlusManager
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    private async Task<FlexiFuturePlusResponse<FlexiFutureQuoteResultDto>> UpsertQuoteInternalAsync(
        FlexiFutureQuoteUpsertDto request,
        PartnerContext partner,
        CancellationToken cancellationToken)
    {
        var response = new FlexiFuturePlusResponse<FlexiFutureQuoteResultDto>();
        try
        {
            var compute = await ComputeAsync(request, cancellationToken);
            if (!compute.Success || compute.Data == null)
            {
                response.Success = false;
                response.Message = compute.Message;
                return response;
            }

            var result = compute.Data;
            var strategy = _akiba.Database.CreateExecutionStrategy();
            FlexiFutureQuoteResultDto? saved = null;

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _akiba.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    FlexiFutureQuote quote;

                    if (request.QuoteId.HasValue && request.QuoteId.Value != Guid.Empty)
                    {
                        var quoteId = request.QuoteId.Value;
                        var existing = await _akiba.FlexiFutureQuotes
                            .Include(q => q.Spouses)
                            .Include(q => q.Children)
                            .Include(q => q.Riders)
                            .FirstOrDefaultAsync(q => q.Id == quoteId, cancellationToken)
                            ?? throw new InvalidOperationException($"Quote '{quoteId}' was not found.");

                        EnsureQuoteAccessible(existing, partner);
                        EnsureQuoteNotCancelled(existing);

                        _akiba.FlexiFutureQuoteSpouses.RemoveRange(existing.Spouses.ToList());
                        _akiba.FlexiFutureQuoteChildren.RemoveRange(existing.Children.ToList());
                        _akiba.FlexiFutureQuoteRiders.RemoveRange(existing.Riders.ToList());
                        await _akiba.SaveChangesAsync(cancellationToken);

                        existing.Spouses.Clear();
                        existing.Children.Clear();
                        existing.Riders.Clear();

                        quote = existing;
                        quote.UpdatedAt = DateTime.UtcNow;
                        ApplySnapshot(quote, request, result, partner);

                        _akiba.FlexiFutureQuoteSpouses.AddRange(quote.Spouses);
                        _akiba.FlexiFutureQuoteChildren.AddRange(quote.Children);
                        _akiba.FlexiFutureQuoteRiders.AddRange(quote.Riders);
                    }
                    else
                    {
                        quote = new FlexiFutureQuote
                        {
                            Id = Guid.NewGuid(),
                            QuoteNumber = await GenerateQuoteNumberAsync(cancellationToken),
                            Status = FlexiFutureQuoteStatuses.Quoted,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        ApplySnapshot(quote, request, result, partner);
                        _akiba.FlexiFutureQuotes.Add(quote);
                    }

                    await _akiba.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                    saved = MapQuote(quote, result, request);
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            });

            response.Success = true;
            response.Message = request.QuoteId.HasValue ? "Quote updated successfully." : "Quote created successfully.";
            response.Data = saved;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = FlexiFutureErrorHandling.ToClientMessage(ex, _logger, nameof(UpsertQuoteInternalAsync));
        }

        return response;
    }

    private async Task<FlexiFuturePlusResponse<FlexiFutureQuoteResultDto>> UpdateQuoteInternalAsync(
        FlexiFutureQuoteRecalculateDto request,
        PartnerContext partner,
        string? ipAddress,
        string? browser,
        CancellationToken cancellationToken)
    {
        var response = new FlexiFuturePlusResponse<FlexiFutureQuoteResultDto>();
        try
        {
            if (request.QuoteId == Guid.Empty)
                throw new InvalidOperationException("quoteId is required.");

            if (!string.IsNullOrWhiteSpace(request.RefferalCode) && request.RefferalCode.Length > 100)
                throw new InvalidOperationException("refferalCode must not exceed 100 characters.");

            var existing = await _akiba.FlexiFutureQuotes
                .Include(q => q.Spouses)
                .Include(q => q.Children)
                .Include(q => q.Riders)
                .FirstOrDefaultAsync(q => q.Id == request.QuoteId, cancellationToken)
                ?? throw new InvalidOperationException($"Quote '{request.QuoteId}' was not found.");

            EnsureQuoteAccessible(existing, partner);
            EnsureQuoteNotCancelled(existing);

            var computeRequest = BuildRecalculateComputeRequest(existing, request, ipAddress, browser);
            var compute = await ComputeAsync(computeRequest, cancellationToken);
            if (!compute.Success || compute.Data == null)
            {
                response.Success = false;
                response.Message = compute.Message;
                return response;
            }

            var result = compute.Data;
            var strategy = _akiba.Database.CreateExecutionStrategy();
            FlexiFutureQuoteResultDto? saved = null;

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _akiba.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    var quote = await _akiba.FlexiFutureQuotes
                        .Include(q => q.Spouses)
                        .Include(q => q.Children)
                        .Include(q => q.Riders)
                        .FirstAsync(q => q.Id == request.QuoteId, cancellationToken);

                    _akiba.FlexiFutureQuoteSpouses.RemoveRange(quote.Spouses.ToList());
                    _akiba.FlexiFutureQuoteChildren.RemoveRange(quote.Children.ToList());
                    _akiba.FlexiFutureQuoteRiders.RemoveRange(quote.Riders.ToList());
                    await _akiba.SaveChangesAsync(cancellationToken);

                    quote.Spouses.Clear();
                    quote.Children.Clear();
                    quote.Riders.Clear();
                    quote.UpdatedAt = DateTime.UtcNow;

                    ApplySnapshot(quote, computeRequest, result, partner);
                    PreserveQuoteClientDetails(quote, existing);

                    _akiba.FlexiFutureQuoteSpouses.AddRange(quote.Spouses);
                    _akiba.FlexiFutureQuoteChildren.AddRange(quote.Children);
                    _akiba.FlexiFutureQuoteRiders.AddRange(quote.Riders);

                    await _akiba.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                    saved = MapQuote(quote, result, computeRequest);
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            });

            response.Success = true;
            response.Message = "Quote recalculated successfully.";
            response.Data = saved;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = FlexiFutureErrorHandling.ToClientMessage(ex, _logger, nameof(UpdateQuoteInternalAsync));
        }

        return response;
    }

    private async Task<FlexiFuturePlusResponse<object>> ShareQuoteInternalAsync(
        FlexiFutureShareQuoteDto request,
        PartnerContext partner,
        CancellationToken cancellationToken)
    {
        var response = new FlexiFuturePlusResponse<object>();
        try
        {
            if (request.QuoteId == Guid.Empty)
                throw new InvalidOperationException("QuoteId is required.");
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new InvalidOperationException("Email is required.");

            var email = request.Email.Trim();
            var quote = await _akiba.FlexiFutureQuotes
                .FirstOrDefaultAsync(q => q.Id == request.QuoteId, cancellationToken)
                ?? throw new InvalidOperationException($"Quote '{request.QuoteId}' was not found.");

            EnsureQuoteAccessible(quote, partner);
            EnsureQuoteNotCancelled(quote);

            var onboardingComplete = await _akiba.FlexiFuturePolicies
                .AsNoTracking()
                .AnyAsync(
                    p => p.QuoteId == request.QuoteId
                         && p.OnboardingStep == FlexiFutureOnboardingSteps.Complete,
                    cancellationToken);
            if (onboardingComplete)
                throw new InvalidOperationException("Quotes that have completed onboarding cannot be shared anonymously.");

            if (quote.RetryCount >= DefaultMaximumFlexiQuoteRetry)
                throw new InvalidOperationException(
                    $"Maximum quote share retries ({DefaultMaximumFlexiQuoteRetry}) have been reached.");

            var ipAddress = Truncate(request.IpAddress, 100);
            var browser = Truncate(request.Browser, 500);

            quote.Email = email;
            quote.RetryCount += 1;
            quote.LastRetryDate = DateTime.UtcNow;
            quote.UpdatedAt = DateTime.UtcNow;

            var share = await _akiba.QuotesShares
                .FirstOrDefaultAsync(
                    s => s.Context == QuoteShareContexts.FlexiFutureQuote
                         && s.QuoteId == request.QuoteId
                         && s.ToEmail == email,
                    cancellationToken);

            if (share != null)
            {
                share.IpAddress = ipAddress;
                share.Browser = browser;
                share.IsSent = false;
                share.IsGenerated = false;
                share.SentOn = null;
                share.FilePath = null;
                share.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                share = new QuotesShare
                {
                    Id = Guid.NewGuid(),
                    Context = QuoteShareContexts.FlexiFutureQuote,
                    QuoteId = request.QuoteId,
                    ToEmail = email,
                    IsGenerated = false,
                    IsSent = false,
                    FilePath = null,
                    SentOn = null,
                    IpAddress = ipAddress,
                    Browser = browser,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _akiba.QuotesShares.Add(share);
            }

            await _akiba.SaveChangesAsync(cancellationToken);

            response.Success = true;
            response.Message = $"Quote has been shared to {MaskEmail(email)}.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = FlexiFutureErrorHandling.ToClientMessage(ex, _logger, nameof(ShareQuoteInternalAsync));
        }

        return response;
    }

    private async Task<FlexiFuturePlusResponse<List<FlexiFutureQuoteSearchResultDto>>> SearchQuotesInternalAsync(
        FlexiFutureQuoteSearchDto request,
        PartnerContext partner,
        CancellationToken cancellationToken)
    {
        var response = new FlexiFuturePlusResponse<List<FlexiFutureQuoteSearchResultDto>>();
        try
        {
            var hasQuoteId = request.QuoteId.HasValue && request.QuoteId.Value != Guid.Empty;
            if (!hasQuoteId
                && string.IsNullOrWhiteSpace(request.QuoteNumber)
                && string.IsNullOrWhiteSpace(request.IdNumber)
                && string.IsNullOrWhiteSpace(request.PhoneNumber))
                throw new InvalidOperationException("Provide QuoteId, QuoteNumber, IdNumber, and/or PhoneNumber.");

            var partnerCode = partner.PartnerCode.Trim();

            var completedQuoteIds = _akiba.FlexiFuturePolicies
                .AsNoTracking()
                .Where(p => p.OnboardingStep == FlexiFutureOnboardingSteps.Complete)
                .Where(p => p.Source == PartnerApiSource)
                .Where(p => p.PartnerCode != null && p.PartnerCode.ToUpper() == partnerCode.ToUpper())
                .Select(p => p.QuoteId);

            var query = _akiba.FlexiFutureQuotes.AsNoTracking()
                .Where(q => q.Status != FlexiFutureQuoteStatuses.Cancelled)
                .Where(q => !completedQuoteIds.Contains(q.Id))
                .Where(q => q.Source == PartnerApiSource)
                .Where(q => q.PartnerCode != null && q.PartnerCode.ToUpper() == partnerCode.ToUpper());

            if (hasQuoteId)
                query = query.Where(q => q.Id == request.QuoteId!.Value);
            if (!string.IsNullOrWhiteSpace(request.QuoteNumber))
                query = query.Where(q => q.QuoteNumber == request.QuoteNumber.Trim());
            if (!string.IsNullOrWhiteSpace(request.IdNumber))
                query = query.Where(q => q.IdNumber == request.IdNumber.Trim());
            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                var lastNine = LastNineDigits(request.PhoneNumber);
                if (lastNine.Length < 9)
                    throw new InvalidOperationException("PhoneNumber must contain at least 9 digits.");

                query = query.Where(q =>
                    q.PhoneNumber.Replace("+", "")
                        .Replace(" ", "")
                        .Replace("-", "")
                        .EndsWith(lastNine));
            }

            var quotes = await query
                .Include(q => q.Spouses)
                .Include(q => q.Children)
                .Include(q => q.Riders)
                .OrderByDescending(q => q.UpdatedAt)
                .ToListAsync(cancellationToken);

            var quoteIds = quotes.Select(q => q.Id).ToList();
            var policies = quoteIds.Count == 0
                ? new Dictionary<Guid, FlexiFuturePolicy>()
                : await _akiba.FlexiFuturePolicies
                    .AsNoTracking()
                    .Where(p => quoteIds.Contains(p.QuoteId))
                    .Where(p => p.Source == PartnerApiSource)
                    .Where(p => p.PartnerCode != null && p.PartnerCode.ToUpper() == partnerCode.ToUpper())
                    .ToDictionaryAsync(p => p.QuoteId, cancellationToken);

            response.Success = true;
            response.Message = quotes.Count == 0 ? "No quote(s) found." : "Quotes retrieved successfully.";
            response.Data = quotes
                .Select(q => MapQuoteSearchResult(q, policies.GetValueOrDefault(q.Id)))
                .ToList();
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = FlexiFutureErrorHandling.ToClientMessage(ex, _logger, nameof(SearchQuotesInternalAsync));
        }

        return response;
    }

    private static void EnsureQuoteAccessible(FlexiFutureQuote quote, PartnerContext partner)
    {
        if (!MatchesPartnerOrigin(quote.Source, quote.PartnerCode, partner))
            throw new InvalidOperationException("You do not have access to this quote.");
    }

    private static bool MatchesPartnerOrigin(int source, string? partnerCode, PartnerContext partner)
    {
        return source == PartnerApiSource
               && !string.IsNullOrWhiteSpace(partnerCode)
               && string.Equals(partnerCode, partner.PartnerCode, StringComparison.OrdinalIgnoreCase);
    }

    private static void EnsureQuoteNotCancelled(FlexiFutureQuote quote)
    {
        if (string.Equals(quote.Status, FlexiFutureQuoteStatuses.Cancelled, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Cancelled quotes cannot be modified.");
    }

    internal static string MaskEmail(string email)
    {
        var at = email.IndexOf('@');
        if (at <= 0 || at == email.Length - 1)
            return "***";

        var local = email[..at];
        var domain = email[(at + 1)..];
        var visible = local.Length == 1 ? "*" : $"{local[0]}***";
        return $"{visible}@{domain}";
    }

    internal static string? Truncate(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var trimmed = value.Trim();
        return trimmed.Length <= maxLength ? trimmed : trimmed[..maxLength];
    }

    internal static string LastNineDigits(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var digits = Regex.Replace(value, @"\D", string.Empty);
        return digits.Length <= 9 ? digits : digits[^9..];
    }

    private static FlexiFutureQuoteUpsertDto BuildRecalculateComputeRequest(
        FlexiFutureQuote existing,
        FlexiFutureQuoteRecalculateDto request,
        string? ipAddress,
        string? browser)
    {
        var selectedRiders = request.SelectedRiders.Count > 0
            ? request.SelectedRiders
            : GetStoredMainRiders(existing);

        return new FlexiFutureQuoteUpsertDto
        {
            QuoteId = existing.Id,
            CalculationMode = request.CalculationMode,
            IssueDate = existing.IssueDate,
            PolicyTerm = request.PolicyTerm,
            Frequency = request.Frequency,
            DeathBenefitPct = request.DeathBenefitPct,
            MaturityBenefitPayments = request.MaturityBenefitPayments,
            TargetPremium = request.TargetPremium,
            TargetSumAssured = request.TargetSumAssured,
            RefferalCode = request.RefferalCode ?? existing.RefferalCode,
            IpAddress = ipAddress,
            Browser = browser,
            Client = new FlexiFutureClientDto
            {
                ClientName = existing.ClientName,
                DateOfBirth = existing.DateOfBirth,
                Gender = existing.Gender,
                PhoneNumber = existing.PhoneNumber,
                Email = existing.Email,
                IdNumber = existing.IdNumber
            },
            SelectedRiders = selectedRiders,
            Spouses = BuildRecalculateSpouses(existing, request.Spouses),
            Children = existing.Children
                .OrderBy(c => c.DateOfBirth)
                .Select(c => new FlexiFutureChildDto
                {
                    Name = c.Name,
                    DateOfBirth = c.DateOfBirth
                })
                .ToList()
        };
    }

    private async Task RefreshQuoteFromOnboardingAsync(
        FlexiFutureQuote quote,
        FlexiFuturePolicy policy,
        PartnerContext partner,
        CancellationToken cancellationToken)
    {
        try
        {
            await _akiba.Entry(quote).Collection(q => q.Spouses).LoadAsync(cancellationToken);
            await _akiba.Entry(quote).Collection(q => q.Children).LoadAsync(cancellationToken);
            await _akiba.Entry(quote).Collection(q => q.Riders).LoadAsync(cancellationToken);

            var familyMembers = await _akiba.FlexiFutureFamilyMembers
                .AsNoTracking()
                .Where(m => m.PolicyId == policy.Id)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync(cancellationToken);

            if (familyMembers.Count == 0)
                return;

            var computeRequest = BuildOnboardingRefreshComputeRequest(quote, familyMembers);
            var compute = await ComputeAsync(computeRequest, cancellationToken);
            if (!compute.Success || compute.Data == null)
                throw new InvalidOperationException(compute.Message ?? "Unable to refresh quote from onboarding.");

            var clientSnapshot = CaptureQuoteClientSnapshot(quote);

            _akiba.FlexiFutureQuoteSpouses.RemoveRange(quote.Spouses.ToList());
            _akiba.FlexiFutureQuoteChildren.RemoveRange(quote.Children.ToList());
            _akiba.FlexiFutureQuoteRiders.RemoveRange(quote.Riders.ToList());

            quote.Spouses.Clear();
            quote.Children.Clear();
            quote.Riders.Clear();
            quote.UpdatedAt = DateTime.UtcNow;

            ApplySnapshot(quote, computeRequest, compute.Data, partner);
            RestoreQuoteClientSnapshot(quote, clientSnapshot);

            _akiba.FlexiFutureQuoteSpouses.AddRange(quote.Spouses);
            _akiba.FlexiFutureQuoteChildren.AddRange(quote.Children);
            _akiba.FlexiFutureQuoteRiders.AddRange(quote.Riders);
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            throw new InvalidOperationException("Unable to refresh quote from onboarding.", ex);
        }
    }

    private async Task SyncFlexiFundFromQuoteAsync(
        FlexiFuturePolicy policy,
        FlexiFutureQuote quote,
        CancellationToken cancellationToken)
    {
        try
        {
            var fund = await _akiba.FlexiFunds
                .FirstOrDefaultAsync(f => f.PolicyId == policy.Id.ToString(), cancellationToken);
            if (fund == null)
                return;

            ApplyQuoteSummaryToFlexiFund(fund, quote);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Unable to sync FlexiFund from quote.", ex);
        }
    }

    private static FlexiFutureQuoteUpsertDto BuildOnboardingRefreshComputeRequest(
        FlexiFutureQuote quote,
        IReadOnlyList<FlexiFutureFamilyMember> familyMembers)
    {
        var (spouses, children) = MapPolicyFamilyMembersToDependents(quote, familyMembers);

        return new FlexiFutureQuoteUpsertDto
        {
            QuoteId = quote.Id,
            CalculationMode = quote.CalculationMode,
            IssueDate = quote.IssueDate,
            PolicyTerm = quote.PolicyTerm,
            Frequency = quote.Frequency,
            DeathBenefitPct = quote.DeathBenefitPct,
            MaturityBenefitPayments = quote.MaturityBenefitPayments,
            TargetPremium = quote.TargetPremium,
            TargetSumAssured = quote.TargetSumAssured,
            RefferalCode = quote.RefferalCode,
            Client = MapQuoteClientDto(quote),
            SelectedRiders = GetStoredMainRiders(quote),
            Spouses = spouses,
            Children = children
        };
    }

    private static FlexiFutureClientDto MapQuoteClientDto(FlexiFutureQuote quote)
    {
        return new FlexiFutureClientDto
        {
            ClientName = quote.ClientName,
            DateOfBirth = quote.DateOfBirth,
            Gender = quote.Gender,
            PhoneNumber = quote.PhoneNumber,
            Email = quote.Email,
            IdNumber = quote.IdNumber
        };
    }

    private static (List<FlexiFutureSpouseDto> Spouses, List<FlexiFutureChildDto> Children) MapPolicyFamilyMembersToDependents(
        FlexiFutureQuote quote,
        IReadOnlyList<FlexiFutureFamilyMember> familyMembers)
    {
        var spouses = familyMembers
            .Where(m => IsOnboardingSpouseRelationship(m.Relationship))
            .Select(m =>
            {
                var spouseIndex = ResolveSpouseIndexFromContext(m.AddedBy)
                    ?? throw new InvalidOperationException(
                        $"Unable to resolve spouse index for family member context '{m.AddedBy}'.");

                if (!m.DateOfBirth.HasValue)
                {
                    throw new InvalidOperationException(
                        $"DateOfBirth is required for spouse '{m.OtherNames} {m.Surname}'.");
                }

                return new FlexiFutureSpouseDto
                {
                    SpouseIndex = spouseIndex,
                    Name = $"{m.OtherNames} {m.Surname}".Trim(),
                    DateOfBirth = m.DateOfBirth,
                    Gender = m.Gender,
                    SelectedRiders = GetStoredSpouseRiders(quote, spouseIndex)
                };
            })
            .OrderBy(s => s.SpouseIndex)
            .ToList();

        var children = familyMembers
            .Where(m => IsOnboardingChildRelationship(m.Relationship))
            .OrderBy(m => ResolveChildOrderFromContext(m.AddedBy))
            .Select(m =>
            {
                if (!m.DateOfBirth.HasValue)
                {
                    throw new InvalidOperationException(
                        $"DateOfBirth is required for child '{m.OtherNames} {m.Surname}'.");
                }

                return new FlexiFutureChildDto
                {
                    Name = $"{m.OtherNames} {m.Surname}".Trim(),
                    DateOfBirth = m.DateOfBirth
                };
            })
            .ToList();

        return (spouses, children);
    }

    private static bool IsOnboardingSpouseRelationship(string? relationship)
    {
        return string.Equals(relationship?.Trim(), "Spouse", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsOnboardingChildRelationship(string? relationship)
    {
        return string.Equals(relationship?.Trim(), "Child", StringComparison.OrdinalIgnoreCase);
    }

    private static int? ResolveSpouseIndexFromContext(string? context)
    {
        if (string.IsNullOrWhiteSpace(context))
            return null;

        if (string.Equals(context, FlexiFuturePartnerHealthQuestionContexts.Spouse1, StringComparison.OrdinalIgnoreCase))
            return 1;
        if (string.Equals(context, FlexiFuturePartnerHealthQuestionContexts.Spouse2, StringComparison.OrdinalIgnoreCase))
            return 2;

        if (context.StartsWith("Spouse", StringComparison.OrdinalIgnoreCase)
            && int.TryParse(context[6..], out var index)
            && index is >= 1 and <= 2)
        {
            return index;
        }

        return null;
    }

    private static int ResolveChildOrderFromContext(string? context)
    {
        if (string.IsNullOrWhiteSpace(context))
            return int.MaxValue;

        if (context.StartsWith("Child", StringComparison.OrdinalIgnoreCase)
            && int.TryParse(context[5..], out var index))
        {
            return index;
        }

        return int.MaxValue;
    }

    private sealed class QuoteClientSnapshot
    {
        public string ClientName { get; init; } = string.Empty;
        public DateOnly DateOfBirth { get; init; }
        public string Gender { get; init; } = string.Empty;
        public string PhoneNumber { get; init; } = string.Empty;
        public string? Email { get; init; }
        public string? IdNumber { get; init; }
    }

    private static QuoteClientSnapshot CaptureQuoteClientSnapshot(FlexiFutureQuote quote)
    {
        return new QuoteClientSnapshot
        {
            ClientName = quote.ClientName,
            DateOfBirth = quote.DateOfBirth,
            Gender = quote.Gender,
            PhoneNumber = quote.PhoneNumber,
            Email = quote.Email,
            IdNumber = quote.IdNumber
        };
    }

    private static void RestoreQuoteClientSnapshot(FlexiFutureQuote quote, QuoteClientSnapshot snapshot)
    {
        quote.ClientName = snapshot.ClientName;
        quote.DateOfBirth = snapshot.DateOfBirth;
        quote.Gender = snapshot.Gender;
        quote.PhoneNumber = snapshot.PhoneNumber;
        quote.Email = snapshot.Email;
        quote.IdNumber = snapshot.IdNumber;
    }

    private static void ApplyQuoteSummaryToFlexiFund(FlexiFund fund, FlexiFutureQuote quote)
    {
        fund.Status = quote.Status;
        fund.CalculationMode = quote.CalculationMode;
        fund.IssueDate = quote.IssueDate;
        fund.PolicyTerm = quote.PolicyTerm;
        fund.Frequency = quote.Frequency;
        fund.DeathBenefitPct = quote.DeathBenefitPct;
        fund.MaturityBenefitPayments = quote.MaturityBenefitPayments;
        fund.TargetPremium = quote.TargetPremium;
        fund.TargetSumAssured = quote.TargetSumAssured;
        fund.SumAssured = quote.SumAssured;
        fund.MonthlyEquivalent = quote.MonthlyEquivalent;
        fund.SavingsPremium = quote.SavingsPremium;
        fund.RidersTotal = quote.RidersTotal;
        fund.Phcl = quote.Phcl;
        fund.TotalPremiumPayable = quote.TotalPremiumPayable;
        fund.RequiresApproval = quote.RequiresApproval;
        fund.MaturityDate = quote.MaturityDate;
        fund.LastModified = DateTimeOffset.UtcNow;
    }

    private static List<FlexiFutureSpouseDto> BuildRecalculateSpouses(
        FlexiFutureQuote existing,
        IReadOnlyList<FlexiFutureQuoteRecalculateSpouseDto> requestSpouses)
    {
        return existing.Spouses
            .OrderBy(s => s.SpouseIndex)
            .Select(entity =>
            {
                var riderInput = requestSpouses.FirstOrDefault(s => s.SpouseIndex == entity.SpouseIndex);
                var selectedRiders = riderInput?.SelectedRiders.Count > 0
                    ? riderInput.SelectedRiders
                    : GetStoredSpouseRiders(existing, entity.SpouseIndex);

                return new FlexiFutureSpouseDto
                {
                    SpouseIndex = entity.SpouseIndex,
                    Name = entity.Name,
                    DateOfBirth = entity.DateOfBirth,
                    Gender = entity.Gender,
                    SelectedRiders = selectedRiders
                };
            })
            .ToList();
    }

    private static List<string> GetStoredMainRiders(FlexiFutureQuote quote)
    {
        return quote.Riders
            .Where(r => r.Selected
                        && string.Equals(r.LifeRole, FlexiFutureLifeRoles.Main, StringComparison.OrdinalIgnoreCase))
            .Select(r => r.RiderCode)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static List<string> GetStoredSpouseRiders(FlexiFutureQuote quote, int spouseIndex)
    {
        var lifeRole = spouseIndex == 2 ? FlexiFutureLifeRoles.Spouse2 : FlexiFutureLifeRoles.Spouse1;
        return quote.Riders
            .Where(r => r.Selected
                        && string.Equals(r.LifeRole, lifeRole, StringComparison.OrdinalIgnoreCase))
            .Select(r => r.RiderCode)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static void PreserveQuoteClientDetails(FlexiFutureQuote quote, FlexiFutureQuote existing)
    {
        quote.ClientName = existing.ClientName;
        quote.DateOfBirth = existing.DateOfBirth;
        quote.Gender = existing.Gender;
        quote.PhoneNumber = existing.PhoneNumber;
        quote.Email = existing.Email;
        quote.IdNumber = existing.IdNumber;

        foreach (var spouse in quote.Spouses)
        {
            var prior = existing.Spouses.FirstOrDefault(s => s.SpouseIndex == spouse.SpouseIndex);
            if (prior == null)
                continue;

            spouse.Name = prior.Name;
            spouse.DateOfBirth = prior.DateOfBirth;
            spouse.Gender = prior.Gender;
        }

        var priorChildren = existing.Children.ToList();
        for (var i = 0; i < quote.Children.Count; i++)
        {
            var child = quote.Children.ElementAt(i);
            var prior = priorChildren.FirstOrDefault(c =>
                            !string.IsNullOrWhiteSpace(child.Name)
                            && string.Equals(c.Name, child.Name, StringComparison.OrdinalIgnoreCase))
                        ?? (i < priorChildren.Count ? priorChildren[i] : null);
            if (prior == null)
                continue;

            child.Name = prior.Name;
            child.DateOfBirth = prior.DateOfBirth;
        }
    }

    private void ApplySnapshot(
        FlexiFutureQuote quote,
        FlexiFutureQuoteUpsertDto request,
        FlexiFutureComputeResultDto result,
        PartnerContext partner)
    {
        quote.CalculationMode = request.CalculationMode;
        quote.IssueDate = request.IssueDate;
        quote.PolicyTerm = request.PolicyTerm;
        quote.Frequency = request.Frequency;
        quote.DeathBenefitPct = request.DeathBenefitPct;
        quote.MaturityBenefitPayments = request.MaturityBenefitPayments;
        quote.TargetPremium = request.TargetPremium;
        quote.TargetSumAssured = request.TargetSumAssured;
        quote.ClientName = request.Client.ClientName;
        quote.DateOfBirth = request.Client.DateOfBirth;
        quote.Gender = request.Client.Gender;
        quote.Anb = result.Anb;
        quote.PhoneNumber = request.Client.PhoneNumber;
        quote.Email = request.Client.Email;
        quote.IdNumber = request.Client.IdNumber;
        quote.RefferalCode = request.ReferralId;
        quote.IpAddress = Truncate(request.IpAddress, 100);
        quote.Browser = Truncate(request.Browser, 500);
        quote.Source = PartnerApiSource;
        quote.PartnerCode = partner.PartnerCode;
        quote.SumAssured = result.SumAssured;
        quote.MonthlyEquivalent = result.MonthlyEquivalent;
        quote.SavingsPremium = result.SavingsPremium;
        quote.RidersTotal = result.RidersTotal;
        quote.Phcl = result.Phcl;
        quote.TotalPremiumPayable = result.TotalPremiumPayable;
        quote.RequiresApproval = result.RequiresApproval;
        quote.MaturityDate = result.MaturityDate;
        quote.RiderBreakdownJson = JsonSerializer.Serialize(result.RiderBreakdown, JsonOptions);
        quote.MaturityScheduleJson = JsonSerializer.Serialize(result.MaturitySchedule, JsonOptions);
        quote.SurrenderScheduleJson = JsonSerializer.Serialize(result.SurrenderSchedule, JsonOptions);
        quote.TaxReliefJson = JsonSerializer.Serialize(result.TaxRelief, JsonOptions);
        quote.SpouseResultsJson = JsonSerializer.Serialize(result.Spouses, JsonOptions);
        quote.ChildrenResultsJson = JsonSerializer.Serialize(result.Children, JsonOptions);

        foreach (var spouse in result.Spouses)
        {
            quote.Spouses.Add(new FlexiFutureQuoteSpouse
            {
                Id = Guid.NewGuid(),
                QuoteId = quote.Id,
                SpouseIndex = spouse.SpouseIndex,
                Name = spouse.Name,
                Anb = spouse.Anb,
                SumAssured = spouse.SumAssured,
                PremiumTotal = spouse.PremiumTotal,
                DateOfBirth = request.Spouses.FirstOrDefault(s => s.SpouseIndex == spouse.SpouseIndex)?.DateOfBirth,
                Gender = request.Spouses.FirstOrDefault(s => s.SpouseIndex == spouse.SpouseIndex)?.Gender
            });
        }

        foreach (var child in result.Children)
        {
            quote.Children.Add(new FlexiFutureQuoteChild
            {
                Id = Guid.NewGuid(),
                QuoteId = quote.Id,
                Name = child.Name,
                Anb = child.Anb,
                SumAssured = child.SumAssured,
                Premium = child.Premium,
                DateOfBirth = request.Children.FirstOrDefault(c => c.Name == child.Name)?.DateOfBirth
            });
        }

        foreach (var line in result.RiderBreakdown)
        {
            quote.Riders.Add(new FlexiFutureQuoteRider
            {
                Id = Guid.NewGuid(),
                QuoteId = quote.Id,
                LifeRole = line.LifeRole,
                RiderCode = line.Code,
                Selected = line.Selected,
                Premium = line.Premium
            });
        }

        foreach (var spouse in result.Spouses)
        {
            foreach (var line in spouse.Riders)
            {
                quote.Riders.Add(new FlexiFutureQuoteRider
                {
                    Id = Guid.NewGuid(),
                    QuoteId = quote.Id,
                    LifeRole = line.LifeRole,
                    RiderCode = line.Code,
                    Selected = line.Selected,
                    Premium = line.Premium
                });
            }
        }
    }

    private async Task<string> GenerateQuoteNumberAsync(CancellationToken cancellationToken)
    {
        string quoteNumber;
        do
        {
            quoteNumber = GenerateNumber();
        }
        while (await _akiba.FlexiFutureQuotes.AnyAsync(q => q.QuoteNumber == quoteNumber, cancellationToken));

        return quoteNumber;
    }

    private static string GenerateNumber(int length = 8)
    {
        var suffix = Guid.NewGuid().ToString("N")[..length].ToUpperInvariant();
        return $"FFP-{suffix}";
    }

    private static FlexiFutureQuoteResultDto MapQuote(
        FlexiFutureQuote quote,
        FlexiFutureComputeResultDto result,
        FlexiFutureQuoteUpsertDto request)
    {
        return new FlexiFutureQuoteResultDto
        {
            QuoteId = quote.Id,
            QuoteNumber = quote.QuoteNumber,
            Status = quote.Status,
            CalculationMode = request.CalculationMode,
            IssueDate = request.IssueDate,
            PolicyTerm = request.PolicyTerm,
            Frequency = request.Frequency,
            DeathBenefitPct = request.DeathBenefitPct,
            MaturityBenefitPayments = request.MaturityBenefitPayments,
            TargetPremium = request.TargetPremium,
            TargetSumAssured = request.TargetSumAssured,
            ReferralId = request.ReferralId,
            Client = request.Client,
            Anb = result.Anb,
            SumAssured = result.SumAssured,
            MonthlyEquivalent = result.MonthlyEquivalent,
            SavingsPremium = result.SavingsPremium,
            RidersTotal = result.RidersTotal,
            Phcl = result.Phcl,
            TotalPremiumPayable = result.TotalPremiumPayable,
            RequiresApproval = result.RequiresApproval,
            MaturityDate = result.MaturityDate,
            Warning = result.Warning,
            RiderBreakdown = result.RiderBreakdown,
            CumulativeRiderBreakdown = result.CumulativeRiderBreakdown,
            MaturitySchedule = result.MaturitySchedule,
            SurrenderSchedule = result.SurrenderSchedule,
            TaxRelief = result.TaxRelief,
            Spouses = result.Spouses,
            Children = result.Children
        };
    }

    private static FlexiFutureQuoteSearchResultDto MapQuoteSearchResult(
        FlexiFutureQuote quote,
        FlexiFuturePolicy? policy)
    {
        var dto = MapPersistedQuote(quote);
        dto.PartnerCode = quote.PartnerCode;
        dto.RefferalCode = quote.RefferalCode;
        dto.ReferralId = quote.RefferalCode;
        dto.CreatedAt = quote.CreatedAt;
        dto.UpdatedAt = quote.UpdatedAt;

        if (policy != null)
        {
            dto.PolicyId = policy.Id;
            dto.PolicyStatus = policy.PolicyStatus;
            dto.OnboardingStep = policy.OnboardingStep;
            dto.CustomerDetailsComplete = !string.Equals(
                policy.OnboardingStep,
                FlexiFutureOnboardingSteps.KYC,
                StringComparison.OrdinalIgnoreCase);
            dto.InitialPaymentComplete = policy.InitialPaymentComplete;
            dto.CallbackUrl = policy.CallbackUrl;
        }

        return dto;
    }

    private static FlexiFutureQuoteSearchResultDto MapPersistedQuote(FlexiFutureQuote quote)
    {
        var dto = new FlexiFutureQuoteSearchResultDto
        {
            QuoteId = quote.Id,
            QuoteNumber = quote.QuoteNumber,
            Status = quote.Status,
            CalculationMode = quote.CalculationMode,
            IssueDate = quote.IssueDate,
            PolicyTerm = quote.PolicyTerm,
            Frequency = quote.Frequency,
            DeathBenefitPct = quote.DeathBenefitPct,
            MaturityBenefitPayments = quote.MaturityBenefitPayments,
            TargetPremium = quote.TargetPremium,
            TargetSumAssured = quote.TargetSumAssured,
            ReferralId = quote.RefferalCode,
            Client = new FlexiFutureClientDto
            {
                ClientName = quote.ClientName,
                DateOfBirth = quote.DateOfBirth,
                Gender = quote.Gender,
                PhoneNumber = quote.PhoneNumber,
                Email = quote.Email,
                IdNumber = quote.IdNumber
            },
            Anb = quote.Anb,
            SumAssured = quote.SumAssured,
            MonthlyEquivalent = quote.MonthlyEquivalent,
            SavingsPremium = quote.SavingsPremium,
            RidersTotal = quote.RidersTotal,
            Phcl = quote.Phcl,
            TotalPremiumPayable = quote.TotalPremiumPayable,
            RequiresApproval = quote.RequiresApproval,
            MaturityDate = quote.MaturityDate,
            RiderBreakdown = DeserializeQuoteList<FlexiFuturePremiumLineDto>(quote.RiderBreakdownJson),
            MaturitySchedule = DeserializeQuoteList<FlexiFutureMaturityPaymentDto>(quote.MaturityScheduleJson),
            SurrenderSchedule = DeserializeQuoteList<FlexiFutureSurrenderYearDto>(quote.SurrenderScheduleJson),
            TaxRelief = string.IsNullOrWhiteSpace(quote.TaxReliefJson)
                ? null
                : JsonSerializer.Deserialize<FlexiFutureTaxReliefDto>(quote.TaxReliefJson, JsonOptions),
            Spouses = MapPersistedSpouses(quote),
            Children = MapPersistedChildren(quote)
        };

        dto.CumulativeRiderBreakdown = BuildCumulativeRiderBreakdown(
            dto.RiderBreakdown,
            dto.Spouses,
            dto.Children);

        return dto;
    }

    private static List<FlexiFutureSpouseResultDto> MapPersistedSpouses(FlexiFutureQuote quote)
    {
        var spouses = DeserializeQuoteList<FlexiFutureSpouseResultDto>(quote.SpouseResultsJson);
        foreach (var spouse in spouses)
        {
            var entity = quote.Spouses.FirstOrDefault(s => s.SpouseIndex == spouse.SpouseIndex);
            if (entity == null)
                continue;

            spouse.DateOfBirth = entity.DateOfBirth;
            if (string.IsNullOrWhiteSpace(spouse.Name))
                spouse.Name = entity.Name;
        }

        if (spouses.Count == 0 && quote.Spouses.Count > 0)
        {
            spouses = quote.Spouses
                .OrderBy(s => s.SpouseIndex)
                .Select(s => new FlexiFutureSpouseResultDto
                {
                    SpouseIndex = s.SpouseIndex,
                    Name = s.Name,
                    DateOfBirth = s.DateOfBirth,
                    Anb = s.Anb,
                    SumAssured = s.SumAssured,
                    PremiumTotal = s.PremiumTotal
                })
                .ToList();
        }

        return spouses;
    }

    private static List<FlexiFutureChildResultDto> MapPersistedChildren(FlexiFutureQuote quote)
    {
        var children = DeserializeQuoteList<FlexiFutureChildResultDto>(quote.ChildrenResultsJson);
        var entities = quote.Children.ToList();
        for (var i = 0; i < children.Count; i++)
        {
            var child = children[i];
            var entity = entities.FirstOrDefault(c =>
                             !string.IsNullOrWhiteSpace(child.Name)
                             && string.Equals(c.Name, child.Name, StringComparison.OrdinalIgnoreCase))
                         ?? (i < entities.Count ? entities[i] : null);
            if (entity == null)
                continue;

            child.DateOfBirth = entity.DateOfBirth;
            if (string.IsNullOrWhiteSpace(child.Name))
                child.Name = entity.Name;
        }

        if (children.Count == 0 && entities.Count > 0)
        {
            children = entities
                .Select(c => new FlexiFutureChildResultDto
                {
                    Name = c.Name,
                    DateOfBirth = c.DateOfBirth,
                    Anb = c.Anb,
                    SumAssured = c.SumAssured,
                    Premium = c.Premium
                })
                .ToList();
        }

        return children;
    }

    private static List<T> DeserializeQuoteList<T>(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new List<T>();

        return JsonSerializer.Deserialize<List<T>>(json, JsonOptions) ?? new List<T>();
    }
}
