using Miastro.Domain.Houses;
using Miastro.Domain.People;

namespace Miastro.Domain.Natal;

public sealed record NatalInputFingerprint(
    DateOnly LocalDate,
    TimeOnly LocalTime,
    DateTimeOffset InstantUtc,
    double Latitude,
    double Longitude,
    string IanaTimeZoneId,
    string TzdbVersion,
    HouseSystem HouseSystem,
    string CalculationProfileId,
    string Engine,
    string EngineVersion,
    string EphemerisVersion,
    BirthTimePrecision TimePrecision = BirthTimePrecision.Exact,
    long GeoNameId = 0,
    string Locality = "",
    int? HistoricalOffsetSeconds = null,
    string? AmbiguousSelection = null,
    TimeOnly? RangeStart = null,
    TimeOnly? RangeEnd = null,
    DayPeriod? DayPeriod = null,
    string Country = "",
    string Region = "",
    string? Subregion = null,
    BirthTemporalResolutionState ResolutionState =
        BirthTemporalResolutionState.Resolved,
    int? AmbiguousEarlierOffsetSeconds = null,
    DateTimeOffset? AmbiguousEarlierInstantUtc = null,
    int? AmbiguousLaterOffsetSeconds = null,
    DateTimeOffset? AmbiguousLaterInstantUtc = null,
    bool ManualCoordinateOverride = false);
