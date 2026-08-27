using Miastro.Domain.Objects;
using Miastro.Domain.Natal;

namespace Miastro.Application.Natal.Reading;

public static class NatalDistributionService
{
    public static NatalDistributionReadModel Build(
        NatalChartSnapshotReadModel snapshot,
        NatalDistributionProfile? profile = null)
    {
        ArgumentNullException.ThrowIfNull(
            snapshot);

        profile ??=
            NatalDistributionProfile.MiastroV1;

        var facts =
            NatalFactsReader
                .Read(snapshot)
                .Where(
                    fact =>
                        profile.Includes(
                            fact.ObjectId))
                .OrderBy(
                    fact =>
                        NatalObjectOrder.GetIndex(
                            fact.ObjectId))
                .ToArray();

        return BuildFromFacts(
            facts,
            profile);
    }

    public static NatalDistributionReadModel BuildFromFacts(
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

        var countedObjects =
            included
                .Select(x => x.ObjectId)
                .ToArray();

        return new NatalDistributionReadModel(
            profile.Id,
            countedObjects,
            BuildSection(
                included,
                NatalDistributionSignCatalog.Element,
                Enum.GetValues<
                    NatalDistributionElement>(),
                NatalDistributionSignCatalog
                    .ElementLabel),
            BuildSection(
                included,
                NatalDistributionSignCatalog.Modality,
                Enum.GetValues<
                    NatalDistributionModality>(),
                NatalDistributionSignCatalog
                    .ModalityLabel),
            BuildSection(
                included,
                NatalDistributionSignCatalog.Polarity,
                Enum.GetValues<
                    NatalDistributionPolarity>(),
                NatalDistributionSignCatalog
                    .PolarityLabel));
    }

    private static NatalDistributionSection<TCategory>
        BuildSection<TCategory>(
            IReadOnlyList<NatalObjectFacts> facts,
            Func<
                Domain.Zodiac.ZodiacSign,
                TCategory> categorySelector,
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
                                                    fact.Sign),
                                                category))
                                .OrderBy(
                                    fact =>
                                        NatalObjectOrder
                                            .GetIndex(
                                                fact.ObjectId))
                                .ToArray();

                        return
                            new NatalDistributionBucket<
                                TCategory>(
                                category,
                                labelSelector(
                                    category),
                                members.Length,
                                members
                                    .Select(
                                        x =>
                                            x.ObjectId)
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
            buckets.Length == 0
                ? 0
                : buckets.Max(
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
            counts.Length > 0
            && counts.Max()
               - counts.Min()
               <= 1;

        return new NatalDistributionSection<TCategory>(
            buckets,
            predominant,
            balanced);
    }
}
