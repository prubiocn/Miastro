namespace Miastro.Infrastructure.Persistence.Entities;

public sealed class BirthDataEntity
{
    public Guid PersonId { get; set; }
    public PersonEntity Person { get; set; } = null!;

    public DateOnly LocalDate { get; set; }
    public int TimePrecision { get; set; }
    public TimeOnly? LocalTime { get; set; }
    public TimeOnly? RangeStart { get; set; }
    public TimeOnly? RangeEnd { get; set; }
    public int? DayPeriod { get; set; }

    public long GeoNameId { get; set; }
    public string Locality { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string? Subregion { get; set; }

    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string IanaTimeZoneId { get; set; } = string.Empty;

    public string? TzdbVersion { get; set; }
    public int TemporalResolutionState { get; set; }
    public int? HistoricalOffsetSeconds { get; set; }
    public DateTimeOffset? ResolvedInstantUtc { get; set; }

    public int? AmbiguousEarlierOffsetSeconds { get; set; }
    public DateTimeOffset? AmbiguousEarlierInstantUtc { get; set; }
    public int? AmbiguousLaterOffsetSeconds { get; set; }
    public DateTimeOffset? AmbiguousLaterInstantUtc { get; set; }
    public int? AmbiguousSelectedCandidate { get; set; }
    public DateTimeOffset? AmbiguousSelectionRecordedAtUtc { get; set; }

    public bool ManualCoordinateOverride { get; set; }
    public double? OriginalGeoNamesLatitude { get; set; }
    public double? OriginalGeoNamesLongitude { get; set; }
}
