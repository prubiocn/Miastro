using Miastro.Domain.People;

namespace Miastro.Application.People;

public sealed record BirthDataWriteModel(
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

public sealed record CurrentResidenceWriteModel(
    string Locality,
    long? GeoNameId,
    string Region,
    string Country,
    double Latitude,
    double Longitude,
    string IanaTimeZoneId,
    DateTimeOffset? UpdatedAtUtc);

public sealed record CreatePersonCommand(
    string FirstName,
    string LastName,
    string? Phone,
    string? Email,
    string? PrivateNote,
    bool IsFavorite,
    BirthDataWriteModel? BirthData,
    CurrentResidenceWriteModel? CurrentResidence);

public sealed record UpdatePersonCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string? Phone,
    string? Email,
    string? PrivateNote,
    bool IsFavorite,
    BirthDataWriteModel? BirthData,
    CurrentResidenceWriteModel? CurrentResidence);
