using DAL.ModelView.FlexiFuture;
using DAL.Model.FlexiFuture;

namespace API.Infrastructure.Application.FlexiFuturePlus;

/// <summary>
/// Precomputed rate bands for O(log n) lookups. Built once when the rate pack loads.
/// </summary>
public sealed class RateTableLookupIndex
{
    /// <summary>TermYears → bands sorted ascending by BandMin (AgeTerm / SaBandTerm).</summary>
    public Dictionary<int, (decimal BandMin, decimal Rate)[]> ByTerm { get; init; } = new();

    /// <summary>Bands sorted ascending by BandMin (AgeOnly).</summary>
    public (decimal BandMin, decimal Rate)[] AgeOnly { get; init; } = Array.Empty<(decimal, decimal)>();
}

public class FlexiFutureRatePack
{
    public FlexiFutureProductSettings Settings { get; set; } = new();
    public IReadOnlyList<FlexiFutureFrequencyDiscount> Frequencies { get; set; } = Array.Empty<FlexiFutureFrequencyDiscount>();
    public IReadOnlyList<FlexiFutureRateTable> Tables { get; set; } = Array.Empty<FlexiFutureRateTable>();
    public Dictionary<(string Code, string PremiumMode), FlexiFutureRateTable> TableIndex { get; set; } = new();
    public Dictionary<(string Code, string PremiumMode), RateTableLookupIndex> RateIndexes { get; set; } = new();
    public Dictionary<string, FlexiFutureFrequencyDiscount> FrequencyIndex { get; set; } =
        new(StringComparer.OrdinalIgnoreCase);
}

public static class RateLookup
{
    public static void BuildIndexes(FlexiFutureRatePack pack)
    {
        try
        {
            pack.TableIndex = pack.Tables.ToDictionary(t => (t.Code, t.PremiumMode));
            pack.FrequencyIndex = pack.Frequencies.ToDictionary(
                f => f.Frequency,
                f => f,
                StringComparer.OrdinalIgnoreCase);
            pack.RateIndexes = new Dictionary<(string Code, string PremiumMode), RateTableLookupIndex>();

            foreach (var table in pack.Tables)
            {
                pack.RateIndexes[(table.Code, table.PremiumMode)] = BuildTableIndex(table);
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static RateTableLookupIndex BuildTableIndex(FlexiFutureRateTable table)
    {
        try
        {
            var active = table.Rates?.Where(r => r.Active).ToList()
                         ?? new List<FlexiFutureRate>();

            var byTerm = active
                .Where(r => r.TermYears.HasValue)
                .GroupBy(r => r.TermYears!.Value)
                .ToDictionary(
                    g => g.Key,
                    g => CollapseBands(g.Select(r => (r.BandMin, r.Rate))));

            var ageOnly = CollapseBands(
                active.Where(r => !r.TermYears.HasValue).Select(r => (r.BandMin, r.Rate)));

            // AgeOnly tables store age in BandMin with TermYears null; some seeds may still set TermYears.
            // Prefer dedicated AgeOnly rows; if empty, also index all rates as age bands (matches prior scan).
            if (ageOnly.Length == 0
                && string.Equals(table.LookupKind, FlexiFutureLookupKinds.AgeOnly, StringComparison.OrdinalIgnoreCase))
            {
                ageOnly = CollapseBands(active.Select(r => (r.BandMin, r.Rate)));
            }

            return new RateTableLookupIndex
            {
                ByTerm = byTerm,
                AgeOnly = ageOnly
            };
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Keep one rate per BandMin (first wins — matches OrderByDescending + First on ties).
    /// Result sorted ascending by BandMin for binary search.
    /// </summary>
    private static (decimal BandMin, decimal Rate)[] CollapseBands(
        IEnumerable<(decimal BandMin, decimal Rate)> bands)
    {
        try
        {
            var map = new SortedDictionary<decimal, decimal>();
            foreach (var (bandMin, rate) in bands)
            {
                if (!map.ContainsKey(bandMin))
                    map[bandMin] = rate;
            }

            return map.Select(kv => (kv.Key, kv.Value)).ToArray();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public static string ResolvePremiumMode(string frequency)
    {
        try
        {
            return string.Equals(frequency, FlexiFutureFrequencies.Single, StringComparison.OrdinalIgnoreCase)
                ? FlexiFuturePremiumModes.Single
                : FlexiFuturePremiumModes.Regular;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public static FlexiFutureRateTable? GetTable(FlexiFutureRatePack pack, string code, string premiumMode)
    {
        try
        {
            if (pack.TableIndex.TryGetValue((code, premiumMode), out var table))
                return table;
            return null;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public static decimal LookupSaBandTerm(FlexiFutureRatePack pack, string code, string premiumMode, decimal sumAssured, int termYears)
    {
        try
        {
            EnsureIndexed(pack);
            if (!pack.RateIndexes.TryGetValue((code, premiumMode), out var index)
                || !index.ByTerm.TryGetValue(termYears, out var bands)
                || bands.Length == 0)
            {
                throw new InvalidOperationException($"No {code} rate for SA={sumAssured}, term={termYears}, mode={premiumMode}.");
            }

            if (!TryLookupBand(bands, sumAssured, out var rate))
                throw new InvalidOperationException($"No {code} rate for SA={sumAssured}, term={termYears}, mode={premiumMode}.");
            return rate;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public static decimal LookupAgeTerm(FlexiFutureRatePack pack, string code, string premiumMode, int anb, int termYears)
    {
        try
        {
            EnsureIndexed(pack);
            if (!pack.RateIndexes.TryGetValue((code, premiumMode), out var index)
                || !index.ByTerm.TryGetValue(termYears, out var bands)
                || bands.Length == 0)
            {
                throw new InvalidOperationException($"No {code} rate for ANB={anb}, term={termYears}, mode={premiumMode}.");
            }

            if (!TryLookupBand(bands, anb, out var rate))
                throw new InvalidOperationException($"No {code} rate for ANB={anb}, term={termYears}, mode={premiumMode}.");
            return rate;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public static decimal LookupAgeOnly(FlexiFutureRatePack pack, string code, string premiumMode, int anb)
    {
        try
        {
            EnsureIndexed(pack);
            if (!pack.RateIndexes.TryGetValue((code, premiumMode), out var index) || index.AgeOnly.Length == 0)
                return 0m;
            return TryLookupBand(index.AgeOnly, anb, out var rate) ? rate : 0m;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public static FlexiFutureFrequencyDiscount GetFrequency(FlexiFutureRatePack pack, string frequency)
    {
        try
        {
            EnsureIndexed(pack);
            if (pack.FrequencyIndex.TryGetValue(frequency, out var freq))
                return freq;

            throw new InvalidOperationException($"Unknown premium frequency '{frequency}'.");
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Largest BandMin &lt;= value (same as prior Where + OrderByDescending + First).
    /// </summary>
    private static bool TryLookupBand((decimal BandMin, decimal Rate)[] bands, decimal value, out decimal rate)
    {
        rate = 0m;
        try
        {
            var lo = 0;
            var hi = bands.Length - 1;
            var found = -1;
            while (lo <= hi)
            {
                var mid = lo + ((hi - lo) / 2);
                if (bands[mid].BandMin <= value)
                {
                    found = mid;
                    lo = mid + 1;
                }
                else
                {
                    hi = mid - 1;
                }
            }

            if (found < 0)
                return false;

            rate = bands[found].Rate;
            return true;
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static void EnsureIndexed(FlexiFutureRatePack pack)
    {
        try
        {
            if (pack.RateIndexes.Count == 0 && pack.Tables.Count > 0)
                BuildIndexes(pack);
        }
        catch (Exception)
        {
            throw;
        }
    }
}
