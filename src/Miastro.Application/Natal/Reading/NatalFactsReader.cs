using Miastro.Domain.Natal;
using Miastro.Domain.Objects;
using Miastro.Domain.Rulerships;
using Miastro.Domain.Zodiac;
using Miastro.Domain.Angles;

namespace Miastro.Application.Natal.Reading;

public static class NatalFactsReader
{
    public static IReadOnlyList<NatalObjectFacts> Read(
        NatalChartSnapshotReadModel snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        return Read(
            snapshot.Placements,
            snapshot.HouseCusps);
    }

    public static IReadOnlyList<NatalObjectFacts> Read(
        IReadOnlyList<NatalPlacementSnapshot> placements,
        IReadOnlyList<NatalHouseCuspSnapshot> houseCusps)
    {
        ArgumentNullException.ThrowIfNull(placements);
        ArgumentNullException.ThrowIfNull(houseCusps);

        var cuspByHouse =
            BuildCuspIndex(
                houseCusps);

        return placements
            .OrderBy(
                placement =>
                    NatalObjectOrder.GetIndex(
                        placement.ObjectId))
            .Select(
                placement =>
                    BuildFacts(
                        placement,
                        cuspByHouse))
            .ToArray();
    }

    private static NatalObjectFacts BuildFacts(
        NatalPlacementSnapshot placement,
        IReadOnlyDictionary<int, NatalHouseCuspSnapshot>
            cuspByHouse)
    {
        var sign =
            ValidateSign(
                placement.ZodiacSign);

        var signRulers =
            RulershipCatalog
                .Get(sign)
                .Both
                .ToArray();

        ZodiacSign? houseCuspSign =
            null;

        IReadOnlyList<AstrologicalObjectId>
            houseRulers =
                Array.Empty<AstrologicalObjectId>();

        if (placement.HouseNumber is int houseNumber)
        {
            if (houseNumber is < 1 or > 12)
            {
                throw new InvalidOperationException(
                    $"Número de casa inválido: {houseNumber}.");
            }

            if (!cuspByHouse.TryGetValue(
                houseNumber,
                out var cusp))
            {
                throw new InvalidOperationException(
                    $"Falta la cúspide de Casa {houseNumber}.");
            }

            houseCuspSign =
                ZodiacSignInfo.FromLongitude(
                    EclipticLongitude.FromDegrees(
                        cusp.LongitudeDegrees));

            houseRulers =
                RulershipCatalog
                    .Get(houseCuspSign.Value)
                    .Both
                    .ToArray();
        }

        return new NatalObjectFacts(
            placement.ObjectId,
            placement.LongitudeDegrees,
            sign,
            placement.DegreeInSign,
            placement.HouseNumber,
            placement.Motion,
            signRulers,
            houseCuspSign,
            houseRulers);
    }

    private static IReadOnlyDictionary<
        int,
        NatalHouseCuspSnapshot>
        BuildCuspIndex(
            IReadOnlyList<NatalHouseCuspSnapshot>
                houseCusps)
    {
        var result =
            new Dictionary<
                int,
                NatalHouseCuspSnapshot>();

        foreach (var cusp in houseCusps)
        {
            if (cusp.HouseNumber is < 1 or > 12)
            {
                throw new InvalidOperationException(
                    $"Número de cúspide inválido: {cusp.HouseNumber}.");
            }

            if (!result.TryAdd(
                cusp.HouseNumber,
                cusp))
            {
                throw new InvalidOperationException(
                    $"Cúspide duplicada para Casa {cusp.HouseNumber}.");
            }
        }

        return result;
    }

    private static ZodiacSign ValidateSign(
        int zodiacSign)
    {
        if (zodiacSign is < 0 or > 11)
        {
            throw new InvalidOperationException(
                $"Índice zodiacal inválido: {zodiacSign}.");
        }

        return (ZodiacSign)zodiacSign;
    }
}
