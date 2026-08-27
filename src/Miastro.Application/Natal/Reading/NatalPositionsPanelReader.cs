namespace Miastro.Application.Natal.Reading;

public static class NatalPositionsPanelReader
{
    public static IReadOnlyList<
        NatalPositionRowReadModel>
        Read(
            NatalChartSnapshotReadModel snapshot)
    {
        ArgumentNullException.ThrowIfNull(
            snapshot);

        return NatalFactsReader
            .Read(snapshot)
            .Select(BuildRow)
            .ToArray();
    }

    private static NatalPositionRowReadModel BuildRow(
        NatalObjectFacts fact)
        => new(
            fact.ObjectId,
            NatalFactsPresentationCatalog
                .ObjectName(
                    fact.ObjectId),
            NatalPositionFormatter
                .ExactPosition(
                    fact.LongitudeDegrees),
            NatalFactsPresentationCatalog
                .SignName(
                    fact.Sign),
            NatalPositionFormatter
                .House(
                    fact.HouseNumber),
            NatalPositionFormatter
                .Motion(
                    fact.Motion),
            NatalFactsPresentationCatalog
                .RulersText(
                    fact.SignRulers),
            fact.HouseCuspSign is { } cuspSign
                ? NatalFactsPresentationCatalog
                    .SignName(cuspSign)
                : "—",
            NatalFactsPresentationCatalog
                .RulersText(
                    fact.HouseRulers),
            fact.Motion,
            NatalFactsPresentationCatalog
                .IsAngle(
                    fact.ObjectId));
}
