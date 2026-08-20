using Miastro.Application.Time;
using Miastro.Domain.Geography;
using NodaTime;

namespace Miastro.Infrastructure.Time.Historical;

public sealed class NodaTimeHistoricalTimeResolver
    : IHistoricalTimeResolver
{
    private readonly IDateTimeZoneProvider _provider;

    public NodaTimeHistoricalTimeResolver()
        : this(DateTimeZoneProviders.Tzdb)
    {
    }

    internal NodaTimeHistoricalTimeResolver(
        IDateTimeZoneProvider provider)
    {
        _provider = provider;
    }

    public HistoricalTimeResolution Resolve(
        LocalDateTime localDateTime,
        IanaTimeZoneId timeZoneId)
    {
        DateTimeZone? zone;

        try
        {
            zone = _provider.GetZoneOrNull(timeZoneId.Value);
        }
        catch (Exception ex)
        {
            throw new HistoricalTimeException(
                HistoricalTimeErrorCode.TzdbError,
                "Unable to access the bundled TZDB provider.",
                ex);
        }

        if (zone is null)
        {
            throw new HistoricalTimeException(
                HistoricalTimeErrorCode.UnknownTimeZone,
                $"Unknown IANA time zone: {timeZoneId.Value}");
        }

        try
        {
            var mapping = zone.MapLocal(localDateTime);
            var version = _provider.VersionId;

            return mapping.Count switch
            {
                1 => Resolved(mapping, timeZoneId, version),
                2 => Ambiguous(mapping, timeZoneId, version),
                0 => Skipped(mapping, timeZoneId, version),
                _ => throw new HistoricalTimeException(
                    HistoricalTimeErrorCode.TzdbError,
                    "Unexpected TZDB local mapping cardinality.")
            };
        }
        catch (HistoricalTimeException)
        {
            throw;
        }
        catch (ArgumentOutOfRangeException ex)
        {
            throw new HistoricalTimeException(
                HistoricalTimeErrorCode.DateOutOfRange,
                "Local date/time is outside the supported range.",
                ex);
        }
        catch (Exception ex)
        {
            throw new HistoricalTimeException(
                HistoricalTimeErrorCode.TzdbError,
                "Historical time resolution failed.",
                ex);
        }
    }

    private static HistoricalTimeResolution Resolved(
        NodaTime.TimeZones.ZoneLocalMapping mapping,
        IanaTimeZoneId zoneId,
        string version)
    {
        var value = mapping.Single();

        return new HistoricalTimeResolution(
            HistoricalTimeResolutionStatus.Resolved,
            mapping.LocalDateTime,
            zoneId,
            version,
            new[]
            {
                new HistoricalTimeCandidate(
                    value.ToInstant(),
                    value.Offset,
                    value)
            },
            null);
    }

    private static HistoricalTimeResolution Ambiguous(
        NodaTime.TimeZones.ZoneLocalMapping mapping,
        IanaTimeZoneId zoneId,
        string version)
    {
        var first = mapping.First();
        var second = mapping.Last();

        return new HistoricalTimeResolution(
            HistoricalTimeResolutionStatus.Ambiguous,
            mapping.LocalDateTime,
            zoneId,
            version,
            new[]
            {
                new HistoricalTimeCandidate(
                    first.ToInstant(),
                    first.Offset,
                    first),
                new HistoricalTimeCandidate(
                    second.ToInstant(),
                    second.Offset,
                    second)
            },
            new HistoricalTimeTransition(
                mapping.EarlyInterval.WallOffset,
                mapping.LateInterval.WallOffset,
                mapping.EarlyInterval.End));
    }

    private static HistoricalTimeResolution Skipped(
        NodaTime.TimeZones.ZoneLocalMapping mapping,
        IanaTimeZoneId zoneId,
        string version)
    {
        return new HistoricalTimeResolution(
            HistoricalTimeResolutionStatus.Skipped,
            mapping.LocalDateTime,
            zoneId,
            version,
            Array.Empty<HistoricalTimeCandidate>(),
            new HistoricalTimeTransition(
                mapping.EarlyInterval.WallOffset,
                mapping.LateInterval.WallOffset,
                mapping.EarlyInterval.End));
    }
}
