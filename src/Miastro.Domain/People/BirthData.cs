using Miastro.Domain.Geography;

namespace Miastro.Domain.People;

public sealed class BirthData
{
    public DateOnly LocalDate { get; private set; }
    public BirthTimePrecision TimePrecision { get; private set; }
    public TimeOnly? LocalTime { get; private set; }
    public TimeOnly? RangeStart { get; private set; }
    public TimeOnly? RangeEnd { get; private set; }
    public DayPeriod? DayPeriod { get; private set; }

    public long GeoNameId { get; private set; }
    public string Locality { get; private set; }
    public string Country { get; private set; }
    public string Region { get; private set; }
    public string? Subregion { get; private set; }

    public Latitude Latitude { get; private set; }
    public Longitude Longitude { get; private set; }
    public IanaTimeZoneId TimeZoneId { get; private set; }

    public string? TzdbVersion { get; private set; }
    public BirthTemporalResolutionState ResolutionState { get; private set; }
    public int? HistoricalOffsetSeconds { get; private set; }
    public DateTimeOffset? ResolvedInstantUtc { get; private set; }

    public int? AmbiguousEarlierOffsetSeconds { get; private set; }
    public DateTimeOffset? AmbiguousEarlierInstantUtc { get; private set; }
    public int? AmbiguousLaterOffsetSeconds { get; private set; }
    public DateTimeOffset? AmbiguousLaterInstantUtc { get; private set; }
    public int? AmbiguousSelectedCandidate { get; private set; }
    public DateTimeOffset? AmbiguousSelectionRecordedAt { get; private set; }

    public bool ManualCoordinateOverride { get; private set; }
    public double? OriginalGeoNamesLatitude { get; private set; }
    public double? OriginalGeoNamesLongitude { get; private set; }

    private BirthData(
        DateOnly localDate,
        BirthTimePrecision timePrecision,
        TimeOnly? localTime,
        TimeOnly? rangeStart,
        TimeOnly? rangeEnd,
        DayPeriod? dayPeriod,
        long geoNameId,
        string locality,
        string country,
        string region,
        string? subregion,
        Latitude latitude,
        Longitude longitude,
        IanaTimeZoneId timeZoneId)
    {
        LocalDate = localDate;
        TimePrecision = timePrecision;
        LocalTime = localTime;
        RangeStart = rangeStart;
        RangeEnd = rangeEnd;
        DayPeriod = dayPeriod;
        GeoNameId = geoNameId;
        Locality = Required(locality, nameof(locality), 200);
        Country = Required(country, nameof(country), 120);
        Region = Required(region, nameof(region), 160);
        Subregion = Optional(subregion, 160);
        Latitude = latitude;
        Longitude = longitude;
        TimeZoneId = timeZoneId;

        ValidatePrecision();
        ResolutionState = RequiresConcreteTime
            ? BirthTemporalResolutionState.Pending
            : BirthTemporalResolutionState.NotApplicable;
    }

    public bool RequiresConcreteTime =>
        TimePrecision is BirthTimePrecision.Exact
            or BirthTimePrecision.Approximate;

    public static BirthData CreateConcrete(
        DateOnly localDate,
        BirthTimePrecision precision,
        TimeOnly localTime,
        long geoNameId,
        string locality,
        string country,
        string region,
        string? subregion,
        Latitude latitude,
        Longitude longitude,
        IanaTimeZoneId timeZoneId)
    {
        if (precision is not BirthTimePrecision.Exact
            and not BirthTimePrecision.Approximate)
        {
            throw new ArgumentException(
                "Concrete birth time requires Exact or Approximate precision.",
                nameof(precision));
        }

        return new BirthData(
            localDate,
            precision,
            localTime,
            null,
            null,
            null,
            geoNameId,
            locality,
            country,
            region,
            subregion,
            latitude,
            longitude,
            timeZoneId);
    }

    public static BirthData CreateRange(
        DateOnly localDate,
        TimeOnly rangeStart,
        TimeOnly rangeEnd,
        long geoNameId,
        string locality,
        string country,
        string region,
        string? subregion,
        Latitude latitude,
        Longitude longitude,
        IanaTimeZoneId timeZoneId)
    {
        return new BirthData(
            localDate,
            BirthTimePrecision.Range,
            null,
            rangeStart,
            rangeEnd,
            null,
            geoNameId,
            locality,
            country,
            region,
            subregion,
            latitude,
            longitude,
            timeZoneId);
    }

    public static BirthData CreateDayPeriod(
        DateOnly localDate,
        DayPeriod dayPeriod,
        long geoNameId,
        string locality,
        string country,
        string region,
        string? subregion,
        Latitude latitude,
        Longitude longitude,
        IanaTimeZoneId timeZoneId)
    {
        return new BirthData(
            localDate,
            BirthTimePrecision.DayPeriod,
            null,
            null,
            null,
            dayPeriod,
            geoNameId,
            locality,
            country,
            region,
            subregion,
            latitude,
            longitude,
            timeZoneId);
    }

    public static BirthData CreateUnknown(
        DateOnly localDate,
        long geoNameId,
        string locality,
        string country,
        string region,
        string? subregion,
        Latitude latitude,
        Longitude longitude,
        IanaTimeZoneId timeZoneId)
    {
        return new BirthData(
            localDate,
            BirthTimePrecision.Unknown,
            null,
            null,
            null,
            null,
            geoNameId,
            locality,
            country,
            region,
            subregion,
            latitude,
            longitude,
            timeZoneId);
    }

    public void MarkResolved(
        string tzdbVersion,
        int historicalOffsetSeconds,
        DateTimeOffset instantUtc)
    {
        EnsureConcreteTime();

        TzdbVersion = Required(
            tzdbVersion,
            nameof(tzdbVersion),
            80);

        HistoricalOffsetSeconds = historicalOffsetSeconds;
        ResolvedInstantUtc = instantUtc.ToUniversalTime();
        ResolutionState = BirthTemporalResolutionState.Resolved;
        ClearAmbiguity();
    }

    public void MarkAmbiguous(
        string tzdbVersion,
        int earlierOffsetSeconds,
        DateTimeOffset earlierInstantUtc,
        int laterOffsetSeconds,
        DateTimeOffset laterInstantUtc)
    {
        EnsureConcreteTime();

        TzdbVersion = Required(
            tzdbVersion,
            nameof(tzdbVersion),
            80);

        ResolutionState = BirthTemporalResolutionState.Ambiguous;
        HistoricalOffsetSeconds = null;
        ResolvedInstantUtc = null;
        AmbiguousEarlierOffsetSeconds = earlierOffsetSeconds;
        AmbiguousEarlierInstantUtc = earlierInstantUtc.ToUniversalTime();
        AmbiguousLaterOffsetSeconds = laterOffsetSeconds;
        AmbiguousLaterInstantUtc = laterInstantUtc.ToUniversalTime();
        AmbiguousSelectedCandidate = null;
        AmbiguousSelectionRecordedAt = null;
    }

    public void SelectAmbiguousCandidate(
        int candidate,
        DateTimeOffset recordedAtUtc)
    {
        if (ResolutionState != BirthTemporalResolutionState.Ambiguous)
        {
            throw new InvalidOperationException(
                "Birth time is not ambiguous.");
        }

        if (candidate is not 1 and not 2)
        {
            throw new ArgumentOutOfRangeException(
                nameof(candidate),
                "Candidate must be 1 or 2.");
        }

        AmbiguousSelectedCandidate = candidate;
        AmbiguousSelectionRecordedAt = recordedAtUtc.ToUniversalTime();

        if (candidate == 1)
        {
            HistoricalOffsetSeconds = AmbiguousEarlierOffsetSeconds;
            ResolvedInstantUtc = AmbiguousEarlierInstantUtc;
        }
        else
        {
            HistoricalOffsetSeconds = AmbiguousLaterOffsetSeconds;
            ResolvedInstantUtc = AmbiguousLaterInstantUtc;
        }
    }

    public void MarkSkipped(string tzdbVersion)
    {
        EnsureConcreteTime();

        TzdbVersion = Required(
            tzdbVersion,
            nameof(tzdbVersion),
            80);

        ResolutionState = BirthTemporalResolutionState.Skipped;
        HistoricalOffsetSeconds = null;
        ResolvedInstantUtc = null;
        ClearAmbiguity();
    }

    public void ApplyManualCoordinates(
        Latitude latitude,
        Longitude longitude)
    {
        if (!ManualCoordinateOverride)
        {
            OriginalGeoNamesLatitude = Latitude.Value;
            OriginalGeoNamesLongitude = Longitude.Value;
        }

        Latitude = latitude;
        Longitude = longitude;
        ManualCoordinateOverride = true;

        InvalidateTemporalResolution();
    }

    public void InvalidateTemporalResolution()
    {
        TzdbVersion = null;
        HistoricalOffsetSeconds = null;
        ResolvedInstantUtc = null;
        ClearAmbiguity();

        ResolutionState = RequiresConcreteTime
            ? BirthTemporalResolutionState.Pending
            : BirthTemporalResolutionState.NotApplicable;
    }

    private void ValidatePrecision()
    {
        switch (TimePrecision)
        {
            case BirthTimePrecision.Exact:
            case BirthTimePrecision.Approximate:
                if (LocalTime is null)
                {
                    throw new ArgumentException(
                        "A concrete time is required.");
                }

                if (RangeStart is not null
                    || RangeEnd is not null
                    || DayPeriod is not null)
                {
                    throw new ArgumentException(
                        "Concrete time cannot include range or day period.");
                }
                break;

            case BirthTimePrecision.Range:
                if (RangeStart is null || RangeEnd is null)
                {
                    throw new ArgumentException(
                        "Range requires start and end.");
                }

                if (RangeStart >= RangeEnd)
                {
                    throw new ArgumentException(
                        "Range start must be earlier than range end.");
                }

                if (LocalTime is not null || DayPeriod is not null)
                {
                    throw new ArgumentException(
                        "Range cannot include exact time or day period.");
                }
                break;

            case BirthTimePrecision.DayPeriod:
                if (DayPeriod is null)
                {
                    throw new ArgumentException(
                        "Day period is required.");
                }

                if (LocalTime is not null
                    || RangeStart is not null
                    || RangeEnd is not null)
                {
                    throw new ArgumentException(
                        "Day period cannot include concrete time or range.");
                }
                break;

            case BirthTimePrecision.Unknown:
                if (LocalTime is not null
                    || RangeStart is not null
                    || RangeEnd is not null
                    || DayPeriod is not null)
                {
                    throw new ArgumentException(
                        "Unknown precision cannot contain time data.");
                }
                break;

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(TimePrecision));
        }
    }

    private void EnsureConcreteTime()
    {
        if (!RequiresConcreteTime || LocalTime is null)
        {
            throw new InvalidOperationException(
                "Historical resolution requires a concrete local time.");
        }
    }

    private void ClearAmbiguity()
    {
        AmbiguousEarlierOffsetSeconds = null;
        AmbiguousEarlierInstantUtc = null;
        AmbiguousLaterOffsetSeconds = null;
        AmbiguousLaterInstantUtc = null;
        AmbiguousSelectedCandidate = null;
        AmbiguousSelectionRecordedAt = null;
    }

    private static string Required(
        string value,
        string parameter,
        int maxLength)
    {
        var normalized = value?.Trim()
            ?? throw new ArgumentNullException(parameter);

        if (normalized.Length == 0)
        {
            throw new ArgumentException(
                "Value is required.",
                parameter);
        }

        if (normalized.Length > maxLength)
        {
            throw new ArgumentException(
                $"Value exceeds {maxLength} characters.",
                parameter);
        }

        return normalized;
    }

    private static string? Optional(
        string? value,
        int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.Trim();

        if (normalized.Length > maxLength)
        {
            throw new ArgumentException(
                $"Value exceeds {maxLength} characters.");
        }

        return normalized;
    }
}
