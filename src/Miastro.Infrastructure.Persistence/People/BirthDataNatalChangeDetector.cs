using Miastro.Application.People;
using Miastro.Infrastructure.Persistence.Entities;

namespace Miastro.Infrastructure.Persistence.People;

internal static class BirthDataNatalChangeDetector
{
    public static bool HasNatalRelevantChange(
        BirthDataEntity? current,
        BirthDataWriteModel? incoming)
    {
        if (current is null || incoming is null)
        {
            return current is not null
                || incoming is not null;
        }

        return
            current.LocalDate != incoming.LocalDate
            || current.TimePrecision !=
                (int)incoming.TimePrecision
            || current.LocalTime != incoming.LocalTime
            || current.RangeStart != incoming.RangeStart
            || current.RangeEnd != incoming.RangeEnd
            || current.DayPeriod !=
                (incoming.DayPeriod is null
                    ? null
                    : (int)incoming.DayPeriod.Value)
            || current.GeoNameId != incoming.GeoNameId
            || !Same(
                current.Locality,
                incoming.Locality)
            || !Same(
                current.Country,
                incoming.Country)
            || !Same(
                current.Region,
                incoming.Region)
            || !SameOptional(
                current.Subregion,
                incoming.Subregion)
            || !SameDouble(
                current.Latitude,
                incoming.Latitude)
            || !SameDouble(
                current.Longitude,
                incoming.Longitude)
            || !Same(
                current.IanaTimeZoneId,
                incoming.IanaTimeZoneId)
            || !SameOptional(
                current.TzdbVersion,
                incoming.TzdbVersion)
            || current.TemporalResolutionState !=
                (int)incoming.ResolutionState
            || current.HistoricalOffsetSeconds !=
                incoming.HistoricalOffsetSeconds
            || current.ResolvedInstantUtc !=
                Normalize(
                    incoming.ResolvedInstantUtc)
            || current.AmbiguousSelectedCandidate !=
                incoming.AmbiguousSelectedCandidate
            || current.AmbiguousEarlierOffsetSeconds !=
                incoming.AmbiguousEarlierOffsetSeconds
            || current.AmbiguousEarlierInstantUtc !=
                Normalize(
                    incoming.AmbiguousEarlierInstantUtc)
            || current.AmbiguousLaterOffsetSeconds !=
                incoming.AmbiguousLaterOffsetSeconds
            || current.AmbiguousLaterInstantUtc !=
                Normalize(
                    incoming.AmbiguousLaterInstantUtc)
            || current.ManualCoordinateOverride !=
                incoming.ManualCoordinateOverride;
    }

    private static bool Same(
        string first,
        string second)
        => string.Equals(
            first.Trim(),
            second.Trim(),
            StringComparison.Ordinal);

    private static bool SameOptional(
        string? first,
        string? second)
        => string.Equals(
            Clean(first),
            Clean(second),
            StringComparison.Ordinal);

    private static string? Clean(
        string? value)
        => string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();

    private static bool SameDouble(
        double first,
        double second)
        => BitConverter.DoubleToInt64Bits(first)
            == BitConverter.DoubleToInt64Bits(second);

    private static DateTimeOffset? Normalize(
        DateTimeOffset? value)
        => value?.ToUniversalTime();
}
