using Miastro.Domain.Aspects;
using Miastro.Domain.Natal;
using Miastro.Domain.Objects;
using Miastro.Domain.Placements;

namespace Miastro.Application.Natal.Reading;

public static class NatalSummaryBuilder
{
    private const int MaximumMainAspects =
        5;

    public static NatalSummaryReadModel Build(
        NatalChartSnapshotReadModel snapshot,
        NatalDistributionProfile? profile = null)
    {
        ArgumentNullException.ThrowIfNull(
            snapshot);

        profile ??=
            NatalDistributionProfile.MiastroV1;

        var facts =
            NatalFactsReader.Read(
                snapshot);

        var distribution =
            NatalDistributionService.BuildFromFacts(
                facts,
                profile);

        return Build(
            facts,
            snapshot.Aspects,
            distribution,
            profile);
    }

    public static NatalSummaryReadModel Build(
        IReadOnlyList<NatalObjectFacts> facts,
        IReadOnlyList<NatalAspectSnapshot> aspects,
        NatalDistributionReadModel distribution,
        NatalDistributionProfile? profile = null)
    {
        ArgumentNullException.ThrowIfNull(
            facts);

        ArgumentNullException.ThrowIfNull(
            aspects);

        ArgumentNullException.ThrowIfNull(
            distribution);

        profile ??=
            NatalDistributionProfile.MiastroV1;

        if (!string.Equals(
            distribution.ProfileId,
            profile.Id,
            StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "El Resumen y la Distribución deben usar el mismo perfil.");
        }

        var orderedFacts =
            facts
                .OrderBy(
                    fact =>
                        NatalObjectOrder.GetIndex(
                            fact.ObjectId))
                .ToArray();

        return new NatalSummaryReadModel(
            BuildObjectLine(
                "Sol",
                RequireFact(
                    orderedFacts,
                    AstrologicalObjectId.Sun),
                includeHouse: true),
            BuildObjectLine(
                "Luna",
                RequireFact(
                    orderedFacts,
                    AstrologicalObjectId.Moon),
                includeHouse: true),
            BuildObjectLine(
                "ASC",
                RequireFact(
                    orderedFacts,
                    AstrologicalObjectId.Ascendant),
                includeHouse: false),
            BuildObjectLine(
                "MC",
                RequireFact(
                    orderedFacts,
                    AstrologicalObjectId.Midheaven),
                includeHouse: false),
            BuildPredominanceLine(
                "Elemento",
                distribution.Elements),
            BuildPredominanceLine(
                "Modalidad",
                distribution.Modalities),
            BuildHouseConcentrationLine(
                orderedFacts,
                profile),
            BuildRetrogradesLine(
                orderedFacts,
                profile),
            BuildMainAspects(
                aspects));
    }

    private static NatalObjectFacts RequireFact(
        IReadOnlyList<NatalObjectFacts> facts,
        AstrologicalObjectId objectId)
        => facts.SingleOrDefault(
               x => x.ObjectId == objectId)
           ?? throw new InvalidOperationException(
               "Falta el objeto requerido para Resumen: "
               + objectId
               + ".");

    private static string BuildObjectLine(
        string label,
        NatalObjectFacts fact,
        bool includeHouse)
    {
        var sign =
            NatalFactsPresentationCatalog
                .SignName(
                    fact.Sign);

        if (!includeHouse)
        {
            return
                $"{label}: {sign}.";
        }

        var house =
            fact.HouseNumber is int number
                ? $"Casa {number}"
                : "sin casa";

        return
            $"{label}: {sign}, {house}.";
    }

    private static string BuildPredominanceLine<TCategory>(
        string label,
        NatalDistributionSection<TCategory> section)
        where TCategory : struct, Enum
    {
        if (section.Predominant is { } predominant)
        {
            var bucket =
                section.Buckets.Single(
                    x =>
                        EqualityComparer<TCategory>
                            .Default
                            .Equals(
                                x.Category,
                                predominant));

            return
                $"{label}: {bucket.Label} "
                + $"({bucket.Count}).";
        }

        return
            section.IsBalanced
                ? $"{label}: equilibrado."
                : $"{label}: sin predominio único.";
    }

    private static string BuildHouseConcentrationLine(
        IReadOnlyList<NatalObjectFacts> facts,
        NatalDistributionProfile profile)
    {
        var included =
            facts
                .Where(
                    fact =>
                        profile.Includes(
                            fact.ObjectId))
                .Where(
                    fact =>
                        fact.HouseNumber is not null)
                .ToArray();

        if (included.Length == 0)
        {
            return
                "Concentración de casas: sin datos.";
        }

        var groups =
            included
                .GroupBy(
                    fact =>
                        fact.HouseNumber!.Value)
                .Select(
                    group =>
                        new
                        {
                            House =
                                group.Key,

                            Count =
                                group.Count()
                        })
                .OrderByDescending(
                    x => x.Count)
                .ThenBy(
                    x => x.House)
                .ToArray();

        var maximum =
            groups[0].Count;

        var leaders =
            groups
                .Where(
                    x =>
                        x.Count == maximum)
                .ToArray();

        if (leaders.Length != 1)
        {
            return
                "Concentración de casas: sin casa única predominante.";
        }

        return
            $"Concentración de casas: Casa "
            + $"{leaders[0].House} "
            + $"({leaders[0].Count}/{included.Length}).";
    }

    private static string BuildRetrogradesLine(
        IReadOnlyList<NatalObjectFacts> facts,
        NatalDistributionProfile profile)
    {
        var retrogrades =
            facts
                .Where(
                    fact =>
                        profile.Includes(
                            fact.ObjectId))
                .Where(
                    fact =>
                        fact.Motion
                        == MotionState.Retrograde)
                .OrderBy(
                    fact =>
                        NatalObjectOrder.GetIndex(
                            fact.ObjectId))
                .Select(
                    fact =>
                        NatalFactsPresentationCatalog
                            .ObjectName(
                                fact.ObjectId))
                .ToArray();

        return
            retrogrades.Length == 0
                ? "Retrógrados: ninguno."
                : "Retrógrados: "
                  + string.Join(
                      ", ",
                      retrogrades)
                  + ".";
    }

    private static IReadOnlyList<
        NatalSummaryAspectReadModel>
        BuildMainAspects(
            IReadOnlyList<NatalAspectSnapshot> aspects)
        => aspects
            .Where(
                aspect =>
                    MiastroV1AspectProfile
                        .Instance
                        .IsParticipant(
                            aspect.FirstObject)
                    && MiastroV1AspectProfile
                        .Instance
                        .IsParticipant(
                            aspect.SecondObject))
            .OrderBy(
                aspect =>
                    aspect.DeviationDegrees)
            .ThenBy(
                aspect =>
                    MajorAspectPriority(
                        aspect.Kind))
            .ThenBy(
                aspect =>
                    NatalObjectOrder.GetIndex(
                        aspect.FirstObject))
            .ThenBy(
                aspect =>
                    NatalObjectOrder.GetIndex(
                        aspect.SecondObject))
            .Take(
                MaximumMainAspects)
            .Select(
                aspect =>
                    new NatalSummaryAspectReadModel(
                        aspect.FirstObject,
                        aspect.SecondObject,
                        aspect.Kind,
                        BuildAspectLine(
                            aspect),
                        aspect.DeviationDegrees))
            .ToArray();

    private static string BuildAspectLine(
        NatalAspectSnapshot aspect)
        =>
            "Aspecto: "
            + NatalFactsPresentationCatalog
                .ObjectName(
                    aspect.FirstObject)
            + " — "
            + NatalAspectPresentationCatalog
                .Name(
                    aspect.Kind)
                .ToLowerInvariant()
            + " — "
            + NatalFactsPresentationCatalog
                .ObjectName(
                    aspect.SecondObject)
            + " — orbe "
            + NatalAspectAngleFormatter
                .DegreesMinutes(
                    aspect.UsedOrbDegrees)
            + ".";

    private static int MajorAspectPriority(
        AspectKind kind)
        => kind switch
        {
            AspectKind.Conjunction => 0,
            AspectKind.Opposition => 1,
            AspectKind.Square => 2,
            AspectKind.Trine => 3,
            AspectKind.Sextile => 4,
            AspectKind.Quincunx => 5,
            AspectKind.Semisextile => 6,
            AspectKind.Quintile => 7,
            AspectKind.Biquintile => 8,
            _ => int.MaxValue
        };
}
