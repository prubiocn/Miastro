using Miastro.Domain.People;

namespace Miastro.Application.People;

public sealed record BirthDataReadModel(
    DateOnly LocalDate,
    BirthTimePrecision TimePrecision,
    TimeOnly? LocalTime,
    TimeOnly? RangeStart,
    TimeOnly? RangeEnd,
    DayPeriod? DayPeriod,
    long GeoNameId,
    string Locality,
    string Country,
    string Region,
    string? Subregion,
    double Latitude,
    double Longitude,
    string IanaTimeZoneId,
    string? TzdbVersion,
    BirthTemporalResolutionState ResolutionState,
    int? HistoricalOffsetSeconds,
    DateTimeOffset? ResolvedInstantUtc,
    int? AmbiguousEarlierOffsetSeconds,
    DateTimeOffset? AmbiguousEarlierInstantUtc,
    int? AmbiguousLaterOffsetSeconds,
    DateTimeOffset? AmbiguousLaterInstantUtc,
    int? AmbiguousSelectedCandidate,
    DateTimeOffset? AmbiguousSelectionRecordedAtUtc,
    bool ManualCoordinateOverride,
    double? OriginalGeoNamesLatitude,
    double? OriginalGeoNamesLongitude);
