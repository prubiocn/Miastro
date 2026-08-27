namespace Miastro.Application.Natal.Reading;

public static class NatalDataPanelReader
{
    public static IReadOnlyList<
        NatalDataRowReadModel>
        Read(
            NatalChartSnapshotReadModel snapshot)
    {
        ArgumentNullException.ThrowIfNull(
            snapshot);

        return NatalFactsReader
            .Read(snapshot)
            .Select(
                fact =>
                    new NatalDataRowReadModel(
                        fact.ObjectId,
                        NatalFactsPresentationCatalog
                            .ObjectName(
                                fact.ObjectId),
                        NatalPositionFormatter
                            .DegreeOnly(
                                fact.LongitudeDegrees),
                        NatalFactsPresentationCatalog
                            .SignName(
                                fact.Sign),
                        NatalFactsPresentationCatalog
                            .RulersText(
                                fact.SignRulers),
                        NatalFactsPresentationCatalog
                            .IsAngle(
                                fact.ObjectId)))
            .ToArray();
    }
}
