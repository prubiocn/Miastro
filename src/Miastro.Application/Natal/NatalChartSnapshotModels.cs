using Miastro.Domain.Aspects;
using Miastro.Domain.Houses;
using Miastro.Domain.Natal;
using Miastro.Domain.Objects;
using Miastro.Domain.Placements;

namespace Miastro.Application.Natal;

public sealed record NatalPlacementSnapshot(
    AstrologicalObjectId ObjectId,
    double LongitudeDegrees,
    double? LatitudeDegrees,
    double? DistanceAu,
    double? LongitudeSpeedDegreesPerDay,
    double? LatitudeSpeedDegreesPerDay,
    double? DistanceSpeedAuPerDay,
    MotionState? Motion,
    int ZodiacSign,
    double DegreeInSign,
    int? HouseNumber);

public sealed record NatalHouseCuspSnapshot(
    int HouseNumber,
    double LongitudeDegrees);

public sealed record NatalAspectSnapshot(
    AstrologicalObjectId FirstObject,
    AstrologicalObjectId SecondObject,
    AspectKind Kind,
    double SeparationDegrees,
    double ExactAngleDegrees,
    double DeviationDegrees,
    double AllowedOrbDegrees,
    double UsedOrbDegrees);

public sealed record NatalChartSnapshotWriteModel(
    Guid PersonId,
    NatalInputFingerprint Input,
    bool IsApproximateBirthTime,
    string Locality,
    string MiastroVersion,
    string AdapterVersion,
    DateTimeOffset CalculatedAtUtc,
    IReadOnlyList<NatalPlacementSnapshot> Placements,
    IReadOnlyList<NatalHouseCuspSnapshot> HouseCusps,
    IReadOnlyList<NatalAspectSnapshot> Aspects,
    int BirthDataVersion = 1,
    string BirthDataHash = "");

public sealed record NatalChartSnapshotReadModel(
    Guid Id,
    Guid PersonId,
    NatalChartStatus Status,
    string InputHash,
    bool IsApproximateBirthTime,
    DateOnly BirthLocalDate,
    TimeOnly BirthLocalTime,
    DateTimeOffset InstantUtc,
    string Locality,
    double Latitude,
    double Longitude,
    string IanaTimeZoneId,
    string TzdbVersion,
    HouseSystem HouseSystem,
    string CalculationProfileId,
    string MiastroVersion,
    string Engine,
    string EngineVersion,
    string AdapterVersion,
    string EphemerisVersion,
    DateTimeOffset CalculatedAtUtc,
    DateTimeOffset? InvalidatedAtUtc,
    Guid? SupersededByChartId,
    IReadOnlyList<NatalPlacementSnapshot> Placements,
    IReadOnlyList<NatalHouseCuspSnapshot> HouseCusps,
    IReadOnlyList<NatalAspectSnapshot> Aspects,
    int BirthDataVersion = 1,
    string BirthDataHash = "",
    int BirthTimePrecision = 0,
    long GeoNameId = 0,
    int? HistoricalOffsetSeconds = null,
    string? AmbiguousSelection = null);

public sealed record PersistNatalChartResult(
    NatalChartSnapshotReadModel Chart,
    bool Created);
