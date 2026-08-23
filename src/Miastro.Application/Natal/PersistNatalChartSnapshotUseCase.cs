namespace Miastro.Application.Natal;

public sealed class PersistNatalChartSnapshotUseCase(
    INatalChartStore store)
{
    public Task<PersistNatalChartResult> ExecuteAsync(
        NatalChartSnapshotWriteModel snapshot,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        if (snapshot.PersonId == Guid.Empty)
        {
            throw new ArgumentException(
                "PersonId is required.",
                nameof(snapshot));
        }

        if (snapshot.Placements.Count == 0)
        {
            throw new ArgumentException(
                "Natal snapshot requires placements.",
                nameof(snapshot));
        }

        if (snapshot.HouseCusps.Count != 12)
        {
            throw new ArgumentException(
                "Natal snapshot requires exactly 12 house cusps.",
                nameof(snapshot));
        }

        if (snapshot.HouseCusps
            .Select(x => x.HouseNumber)
            .Order()
            .SequenceEqual(Enumerable.Range(1, 12)) is false)
        {
            throw new ArgumentException(
                "House cusps must contain houses 1 through 12.",
                nameof(snapshot));
        }

        NatalSnapshotValidator.Validate(snapshot);


        var inputHash =
            Miastro.Domain.Natal.NatalInputHash.Compute(
                snapshot.Input);

        return store.SaveOrGetExistingAsync(
            snapshot,
            inputHash,
            cancellationToken);
    }
}
