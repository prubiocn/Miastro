using Miastro.Domain.Aspects;
using Miastro.Domain.Natal;
using Miastro.Domain.Objects;

namespace Miastro.Application.Natal.Reading;

public static class NatalAspectMatrixReader
{
    public static NatalAspectMatrixReadModel Read(
        NatalChartSnapshotReadModel snapshot)
    {
        ArgumentNullException.ThrowIfNull(
            snapshot);

        return Read(
            snapshot.Placements,
            snapshot.Aspects);
    }

    public static NatalAspectMatrixReadModel Read(
        IReadOnlyList<NatalPlacementSnapshot> placements,
        IReadOnlyList<NatalAspectSnapshot> aspects)
    {
        ArgumentNullException.ThrowIfNull(
            placements);

        ArgumentNullException.ThrowIfNull(
            aspects);

        var presentObjects =
            placements
                .Select(x => x.ObjectId)
                .ToHashSet();

        var participants =
            NatalObjectOrder.All
                .Where(
                    objectId =>
                        MiastroV1AspectProfile
                            .Instance
                            .IsParticipant(
                                objectId))
                .Where(
                    presentObjects.Contains)
                .Select(
                    objectId =>
                        new NatalAspectMatrixParticipant(
                            objectId,
                            NatalFactsPresentationCatalog
                                .ObjectName(
                                    objectId),
                            NatalObjectOrder.GetIndex(
                                objectId)))
                .ToArray();

        var participantSet =
            participants
                .Select(x => x.ObjectId)
                .ToHashSet();

        var aspectIndex =
            BuildAspectIndex(
                aspects,
                participantSet);

        var cells =
            new List<NatalAspectMatrixCell>();

        for (var row = 1;
             row < participants.Length;
             row++)
        {
            for (var column = 0;
                 column < row;
                 column++)
            {
                var rowParticipant =
                    participants[row];

                var columnParticipant =
                    participants[column];

                aspectIndex.TryGetValue(
                    NormalizePair(
                        rowParticipant.ObjectId,
                        columnParticipant.ObjectId),
                    out var aspect);

                cells.Add(
                    BuildCell(
                        row,
                        column,
                        rowParticipant,
                        columnParticipant,
                        aspect));
            }
        }

        return new NatalAspectMatrixReadModel(
            participants,
            cells);
    }

    private static IReadOnlyDictionary<
        (AstrologicalObjectId First,
         AstrologicalObjectId Second),
        NatalAspectSnapshot>
        BuildAspectIndex(
            IReadOnlyList<NatalAspectSnapshot> aspects,
            IReadOnlySet<AstrologicalObjectId>
                participants)
    {
        var result =
            new Dictionary<
                (AstrologicalObjectId,
                 AstrologicalObjectId),
                NatalAspectSnapshot>();

        foreach (var aspect in aspects)
        {
            if (!participants.Contains(
                    aspect.FirstObject)
                || !participants.Contains(
                    aspect.SecondObject))
            {
                continue;
            }

            var key =
                NormalizePair(
                    aspect.FirstObject,
                    aspect.SecondObject);

            if (!result.TryAdd(
                key,
                aspect))
            {
                throw new InvalidOperationException(
                    "Aspecto natal duplicado para "
                    + $"{key.Item1}/{key.Item2}.");
            }
        }

        return result;
    }

    private static NatalAspectMatrixCell BuildCell(
        int row,
        int column,
        NatalAspectMatrixParticipant rowParticipant,
        NatalAspectMatrixParticipant columnParticipant,
        NatalAspectSnapshot? aspect)
    {
        if (aspect is null)
        {
            return new NatalAspectMatrixCell(
                row,
                column,
                rowParticipant.ObjectId,
                columnParticipant.ObjectId,
                rowParticipant.ObjectName,
                columnParticipant.ObjectName,
                null,
                "Sin aspecto",
                string.Empty,
                null,
                null,
                null,
                null,
                null,
                string.Empty,
                string.Empty,
                string.Empty,
                $"{columnParticipant.ObjectName} — "
                + $"{rowParticipant.ObjectName} — sin aspecto");
        }

        var aspectName =
            NatalAspectPresentationCatalog
                .Name(
                    aspect.Kind);

        var separationText =
            NatalAspectAngleFormatter
                .DegreesMinutes(
                    aspect.SeparationDegrees);

        var orbText =
            NatalAspectAngleFormatter
                .DegreesMinutes(
                    aspect.UsedOrbDegrees);

        var deviationText =
            NatalAspectAngleFormatter
                .DegreesMinutes(
                    aspect.DeviationDegrees);

        return new NatalAspectMatrixCell(
            row,
            column,
            rowParticipant.ObjectId,
            columnParticipant.ObjectId,
            rowParticipant.ObjectName,
            columnParticipant.ObjectName,
            aspect.Kind,
            aspectName,
            NatalAspectPresentationCatalog
                .Symbol(
                    aspect.Kind),
            aspect.SeparationDegrees,
            aspect.ExactAngleDegrees,
            aspect.DeviationDegrees,
            aspect.AllowedOrbDegrees,
            aspect.UsedOrbDegrees,
            separationText,
            orbText,
            deviationText,
            $"{columnParticipant.ObjectName} — "
            + $"{aspectName.ToLowerInvariant()} — "
            + $"{rowParticipant.ObjectName} — "
            + $"orbe {orbText}");
    }

    private static (
        AstrologicalObjectId First,
        AstrologicalObjectId Second)
        NormalizePair(
            AstrologicalObjectId first,
            AstrologicalObjectId second)
    {
        var firstIndex =
            NatalObjectOrder.GetIndex(
                first);

        var secondIndex =
            NatalObjectOrder.GetIndex(
                second);

        return firstIndex <= secondIndex
            ? (first, second)
            : (second, first);
    }
}
