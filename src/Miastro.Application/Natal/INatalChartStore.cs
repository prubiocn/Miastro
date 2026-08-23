namespace Miastro.Application.Natal;

public interface INatalChartStore
{
    Task<PersistNatalChartResult> SaveOrGetExistingAsync(
        NatalChartSnapshotWriteModel snapshot,
        string inputHash,
        CancellationToken cancellationToken = default);

    Task<NatalChartSnapshotReadModel?> GetCurrentAsync(
        Guid personId,
        CancellationToken cancellationToken = default);

    Task<NatalChartSnapshotReadModel?> GetByInputHashAsync(
        Guid personId,
        string inputHash,
        CancellationToken cancellationToken = default);

    Task InvalidateCurrentAsync(
        Guid personId,
        DateTimeOffset invalidatedAtUtc,
        CancellationToken cancellationToken = default);
}
