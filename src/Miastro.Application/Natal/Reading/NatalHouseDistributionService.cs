using Miastro.Domain.Natal;

namespace Miastro.Application.Natal.Reading;

public static class NatalHouseDistributionService
{
    public static NatalHouseDistributionReadModel Build(
        NatalChartSnapshotReadModel snapshot,
        NatalDistributionProfile? profile = null)
    {
        ArgumentNullException.ThrowIfNull(
            snapshot);

        profile ??=
            NatalDistributionProfile.MiastroV1;

        return BuildFromFacts(
            NatalFactsReader.Read(
                snapshot),
            profile);
    }

    public static NatalHouseDistributionReadModel BuildFromFacts(
        IReadOnlyList<NatalObjectFacts> facts,
        NatalDistributionProfile? profile = null)
    {
        ArgumentNullException.ThrowIfNull(
            facts);

        profile ??=
            NatalDistributionProfile.MiastroV1;

        var included =
            facts
                .Where(
                    fact =>
                        profile.Includes(
                            fact.ObjectId))
                .OrderBy(
                    fact =>
                        NatalObjectOrder.GetIndex(
                            fact.ObjectId))
                .ToArray();

        foreach (var fact in included)
        {
            if (fact.HouseNumber is null)
            {
                throw new InvalidOperationException(
                    "El objeto "
                    + NatalFactsPresentationCatalog
                        .ObjectName(
                            fact.ObjectId)
                    + " no tiene casa persistida.");
            }

            if (fact.HouseNumber is < 1 or > 12)
            {
                throw new InvalidOperationException(
                    $"Casa inválida {fact.HouseNumber} "
                    + "para "
                    + NatalFactsPresentationCatalog
                        .ObjectName(
                            fact.ObjectId)
                    + ".");
            }
        }

        return new NatalHouseDistributionReadModel(
            profile.Id,
            BuildSection(
                included,
                house =>
                    NatalHouseDistributionCatalog
                        .EastWest(house),
                Enum.GetValues<
                    NatalEastWestHemisphere>(),
                NatalHouseDistributionCatalog
                    .EastWestLabel),
            BuildSection(
                included,
                house =>
                    NatalHouseDistributionCatalog
                        .UpperLower(house),
                Enum.GetValues<
                    NatalUpperLowerHemisphere>(),
                NatalHouseDistributionCatalog
                    .UpperLowerLabel),
            BuildSection(
                included,
                house =>
                    NatalHouseDistributionCatalog
                        .Quadrant(house),
                Enum.GetValues<
                    NatalHouseQuadrant>(),
                NatalHouseDistributionCatalog
                    .QuadrantLabel),
            BuildSection(
                included,
                house =>
                    NatalHouseDistributionCatalog
                        .HouseMode(house),
                Enum.GetValues<
                    NatalHouseMode>(),
                NatalHouseDistributionCatalog
                    .HouseModeLabel));
    }

    private static NatalDistributionSection<TCategory>
        BuildSection<TCategory>(
            IReadOnlyList<NatalObjectFacts> facts,
            Func<int, TCategory> categorySelector,
            IReadOnlyList<TCategory> categories,
            Func<TCategory, string> labelSelector)
        where TCategory : struct, Enum
    {
        var buckets =
            categories
                .Select(
                    category =>
                    {
                        var members =
                            facts
                                .Where(
                                    fact =>
                                        EqualityComparer<TCategory>
                                            .Default
                                            .Equals(
                                                categorySelector(
                                                    fact.HouseNumber!.Value),
                                                category))
                                .OrderBy(
                                    fact =>
                                        NatalObjectOrder
                                            .GetIndex(
                                                fact.ObjectId))
                                .ToArray();

                        return new NatalDistributionBucket<TCategory>(
                            category,
                            labelSelector(
                                category),
                            members.Length,
                            members
                                .Select(
                                    x => x.ObjectId)
                                .ToArray(),
                            members
                                .Select(
                                    x =>
                                        NatalFactsPresentationCatalog
                                            .ObjectName(
                                                x.ObjectId))
                                .ToArray());
                    })
                .ToArray();

        var maximum =
            buckets.Max(
                x => x.Count);

        var leaders =
            buckets
                .Where(
                    x =>
                        x.Count == maximum
                        && maximum > 0)
                .ToArray();

        var predominant =
            leaders.Length == 1
                ? leaders[0].Category
                : (TCategory?)null;

        var counts =
            buckets
                .Select(x => x.Count)
                .ToArray();

        var balanced =
            counts.Max()
            - counts.Min()
            <= 1;

        return new NatalDistributionSection<TCategory>(
            buckets,
            predominant,
            balanced);
    }
}
