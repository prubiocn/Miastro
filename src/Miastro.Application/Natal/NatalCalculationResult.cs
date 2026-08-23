using Miastro.Domain.Aspects;
using Miastro.Domain.Charts;
using Miastro.Domain.DerivedPoints;

namespace Miastro.Application.Natal;

public enum NatalCalculationResultCode
{
    Calculated = 1,
    ExistingCurrentSnapshot = 2,

    PersonNotFound = 10,
    BirthDataMissing = 11,
    BirthTimeInsufficient = 12,
    HistoricalTimeUnresolved = 13,
    InvalidCoordinates = 14,

    HouseCalculationUnavailable = 20,
    AstronomyCalculationFailed = 21,
    PersistenceFailed = 22
}

public sealed record NatalCalculationResult(
    NatalCalculationResultCode Code,
    string Message,
    NatalChartSnapshotReadModel? Snapshot = null,
    AstrologicalChart? Chart = null,
    IReadOnlyList<AspectResult>? Aspects = null,
    ChartSect? Sect = null)
{
    public bool Success =>
        Code is
            NatalCalculationResultCode.Calculated
            or NatalCalculationResultCode.ExistingCurrentSnapshot;
}
