using DAL.ModelView.FlexiFuture;
using DAL.Model.FlexiFuture;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace API.Infrastructure.Application.FlexiFuturePlus;

public partial class FlexiFuturePlusManager
{
    private const string RatesCacheKey = "flexifuture:rates";

    public async Task<FlexiFutureRatePack> GetActiveRatesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _cache.GetOrCreateAsync(RatesCacheKey, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromHours(1);
                return await LoadFromDbAsync(cancellationToken);
            }) ?? await LoadFromDbAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            var message = FlexiFutureErrorHandling.ToClientMessage(ex, _logger, nameof(GetActiveRatesAsync));
            throw new InvalidOperationException(message);
        }
    }

    public async Task<FlexiFutureUiConfigDto> GetUiConfigAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var pack = await GetActiveRatesAsync(cancellationToken);
            var settings = pack.Settings;
            var terms = Enumerable.Range(settings.MinTerm, settings.MaxTerm - settings.MinTerm + 1).ToList();

            var riderTables = pack.Tables
                .Where(t => t.IsRider
                            && t.PremiumMode == FlexiFuturePremiumModes.Regular
                            && t.Active
                            // CHILD_LE is auto-applied with death cover; not a selectable UI rider.
                            && !string.Equals(t.Code, FlexiFutureRateCodes.ChildLe, StringComparison.OrdinalIgnoreCase))
                .OrderBy(t => t.SortOrder)
                .ToList();

            return new FlexiFutureUiConfigDto
            {
                // MinTerm = settings.MinTerm,
                // MaxTerm = settings.MaxTerm,
                Terms = terms,
                // SaBandLow = settings.SaBandLow,
                // SaBandMid = settings.SaBandMid,
                // MinTermBelowSaBandLow = settings.MinTermBelowSaBandLow,
                // MinTermBelowSaBandMid = settings.MinTermBelowSaBandMid,
                MinEntryAge = settings.MinEntryAge,
                MaxEntryAge = settings.MaxEntryAge,
                Frequencies = pack.Frequencies
                    .OrderBy(f => f.SortOrder)
                    .Select(f => new FrequencyOptionDto
                    {
                        Frequency = f.Frequency,
                        DiscountRate = f.DiscountRate,
                        Label = f.Frequency,
                        IsSingle = string.Equals(f.Frequency, FlexiFutureFrequencies.Single, StringComparison.OrdinalIgnoreCase)
                    }).ToList(),
                Riders = riderTables.Select(t => new RiderOptionDto
                {
                    Code = t.Code,
                    Name = t.Name,
                    Description = t.Name,
                    MaxCoverageAge = t.MaxCoverageAge,
                    DefaultCoverFactor = t.DefaultCoverFactor,
                    AvailableForSinglePremium = t.AvailableForSinglePremium,
                    RequiresMainDeathCover = t.RequiresMainDeathCover,
                    AppliesToMain = t.AppliesToMain,
                    AppliesToSpouse = t.AppliesToSpouse,
                    SortOrder = t.SortOrder
                }).ToList(),
                MinSumAssured = settings.MinSumAssured,
                MaxSumAssured = settings.MaxCoverPerLife,
                MaxCoverPerLife = settings.MaxCoverPerLife,
                SpouseSaCap = settings.SpouseSaCap,
                CalculationModes = new[]
                {
                    FlexiFutureCalculationModes.SaToPremium,
                    FlexiFutureCalculationModes.PremiumToSa
                },
                DeathBenefitPercentOptions = new[] { 0m, 0.5m, 1m, 2m },
                MinMaturityPayments = settings.MinMaturityPayments,
                MaxMaturityPayments = settings.MaxMaturityPayments,
                MaxSpouses = settings.MaxSpouses,
                MaxChildren = settings.MaxChildren
            };
        }
        catch (Exception ex)
        {
            var message = FlexiFutureErrorHandling.ToClientMessage(ex, _logger, nameof(GetUiConfigAsync));
            throw new InvalidOperationException(message);
        }
    }

    private async Task<FlexiFutureRatePack> LoadFromDbAsync(CancellationToken cancellationToken)
    {
        try
        {
            var settings = await _akiba.FlexiFutureProductSettings
                .AsNoTracking()
                .Where(s => s.Active)
                .OrderByDescending(s => s.EffectiveFrom)
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new InvalidOperationException("FlexiFuture product settings are not configured.");

            var frequencies = await _akiba.FlexiFutureFrequencyDiscounts
                .AsNoTracking()
                .Where(f => f.Active)
                .OrderBy(f => f.SortOrder)
                .ToListAsync(cancellationToken);

            var tables = await _akiba.FlexiFutureRateTables
                .AsNoTracking()
                .Include(t => t.Rates.Where(r => r.Active))
                .Where(t => t.Active)
                .ToListAsync(cancellationToken);

            var pack = new FlexiFutureRatePack
            {
                Settings = settings,
                Frequencies = frequencies,
                Tables = tables
            };
            RateLookup.BuildIndexes(pack);
            return pack;
        }
        catch (Exception)
        {
            throw;
        }
    }
}
