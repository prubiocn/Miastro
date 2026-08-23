using Miastro.Domain.Aspects;
using Miastro.Domain.Natal;
using Miastro.Domain.Objects;

namespace Miastro.Application.Natal;

public static class NatalSnapshotValidator
{
    private const double NodeToleranceDegrees =
        1e-9;

    public static void Validate(
        NatalChartSnapshotWriteModel snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        if (snapshot.PersonId == Guid.Empty)
        {
            throw new ArgumentException(
                "PersonId natal no válido.",
                nameof(snapshot));
        }

        ValidateBirthIdentity(snapshot);
        ValidatePlacements(snapshot.Placements);
        ValidateCusps(snapshot.HouseCusps);
        ValidateAspects(snapshot.Aspects);
        ValidateDerivedNode(snapshot.Placements);
    }

    private static void ValidateBirthIdentity(
        NatalChartSnapshotWriteModel snapshot)
    {
        if (snapshot.BirthDataVersion !=
            NatalBirthDataIdentity.CurrentVersion)
        {
            throw new ArgumentException(
                "Versión BirthData no soportada.",
                nameof(snapshot));
        }

        var expected =
            NatalBirthDataIdentity.Compute(
                snapshot.Input);

        if (!string.Equals(
                expected,
                snapshot.BirthDataHash,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "BirthDataHash inconsistente.",
                nameof(snapshot));
        }
    }

    private static void ValidatePlacements(
        IReadOnlyList<NatalPlacementSnapshot> placements)
    {
        ArgumentNullException.ThrowIfNull(placements);

        if (placements.Count !=
            NatalObjectOrder.All.Count)
        {
            throw new ArgumentException(
                $"Se requieren exactamente {NatalObjectOrder.All.Count} placements.");
        }

        var actual =
            placements
                .Select(x => x.ObjectId)
                .ToArray();

        if (actual.Distinct().Count() !=
            actual.Length)
        {
            throw new ArgumentException(
                "Existen objetos natales duplicados.");
        }

        if (!actual.SequenceEqual(
            NatalObjectOrder.All))
        {
            throw new ArgumentException(
                "Los placements no siguen el orden canónico natal.");
        }

        foreach (var placement in placements)
        {
            if (!double.IsFinite(
                    placement.LongitudeDegrees)
                || placement.LongitudeDegrees < 0.0
                || placement.LongitudeDegrees >= 360.0)
            {
                throw new ArgumentException(
                    $"Longitud no válida para {placement.ObjectId}.");
            }

            if (placement.LatitudeDegrees is double latitude
                && !double.IsFinite(latitude))
            {
                throw new ArgumentException(
                    $"Latitud no válida para {placement.ObjectId}.");
            }

            if (placement.DistanceAu is double distance
                && !double.IsFinite(distance))
            {
                throw new ArgumentException(
                    $"Distancia no válida para {placement.ObjectId}.");
            }

            if (placement.LongitudeSpeedDegreesPerDay is double longitudeSpeed
                && !double.IsFinite(longitudeSpeed))
            {
                throw new ArgumentException(
                    $"Velocidad longitudinal no válida para {placement.ObjectId}.");
            }

            if (placement.LatitudeSpeedDegreesPerDay is double latitudeSpeed
                && !double.IsFinite(latitudeSpeed))
            {
                throw new ArgumentException(
                    $"Velocidad latitudinal no válida para {placement.ObjectId}.");
            }

            if (placement.DistanceSpeedAuPerDay is double distanceSpeed
                && !double.IsFinite(distanceSpeed))
            {
                throw new ArgumentException(
                    $"Velocidad radial no válida para {placement.ObjectId}.");
            }

            if (placement.ZodiacSign is < 0 or > 11)
            {
                throw new ArgumentException(
                    $"Signo no válido para {placement.ObjectId}.");
            }

            if (!double.IsFinite(
                    placement.DegreeInSign)
                || placement.DegreeInSign < 0.0
                || placement.DegreeInSign >= 30.0)
            {
                throw new ArgumentException(
                    $"Grado en signo no válido para {placement.ObjectId}.");
            }

            if (placement.HouseNumber is int house
                && house is < 1 or > 12)
            {
                throw new ArgumentException(
                    $"Casa no válida para {placement.ObjectId}.");
            }
        }

        Require(
            actual,
            AstrologicalObjectId.Ascendant);

        Require(
            actual,
            AstrologicalObjectId.Midheaven);

        Require(
            actual,
            AstrologicalObjectId.NorthTrueNode);

        Require(
            actual,
            AstrologicalObjectId.SouthNode);

        Require(
            actual,
            AstrologicalObjectId.PartOfFortune);
    }

    private static void ValidateCusps(
        IReadOnlyList<NatalHouseCuspSnapshot> cusps)
    {
        ArgumentNullException.ThrowIfNull(cusps);

        if (cusps.Count != 12)
        {
            throw new ArgumentException(
                "Se requieren exactamente 12 cúspides.");
        }

        var houses =
            cusps
                .Select(x => x.HouseNumber)
                .ToArray();

        if (!houses.SequenceEqual(
            Enumerable.Range(1, 12)))
        {
            throw new ArgumentException(
                "Las cúspides deben estar ordenadas de casa 1 a 12.");
        }

        if (houses.Distinct().Count() != 12)
        {
            throw new ArgumentException(
                "Hay cúspides duplicadas.");
        }

        foreach (var cusp in cusps)
        {
            if (!double.IsFinite(
                    cusp.LongitudeDegrees)
                || cusp.LongitudeDegrees < 0.0
                || cusp.LongitudeDegrees >= 360.0)
            {
                throw new ArgumentException(
                    $"Longitud de cúspide no válida: casa {cusp.HouseNumber}.");
            }
        }
    }

    private static void ValidateAspects(
        IReadOnlyList<NatalAspectSnapshot> aspects)
    {
        ArgumentNullException.ThrowIfNull(aspects);

        var pairs =
            new HashSet<
                (AstrologicalObjectId, AstrologicalObjectId)>();

        foreach (var aspect in aspects)
        {
            if (aspect.FirstObject ==
                aspect.SecondObject)
            {
                throw new ArgumentException(
                    "Un aspecto no puede referenciar el mismo objeto dos veces.");
            }

            if (!MiastroV1AspectProfile.Instance
                    .IsParticipant(
                        aspect.FirstObject)
                || !MiastroV1AspectProfile.Instance
                    .IsParticipant(
                        aspect.SecondObject))
            {
                throw new ArgumentException(
                    $"Aspecto con participante no permitido: "
                    + $"{aspect.FirstObject}/{aspect.SecondObject}.");
            }

            var first =
                (int)aspect.FirstObject
                <= (int)aspect.SecondObject
                    ? aspect.FirstObject
                    : aspect.SecondObject;

            var second =
                (int)aspect.FirstObject
                <= (int)aspect.SecondObject
                    ? aspect.SecondObject
                    : aspect.FirstObject;

            var key =
                (first, second);

            if (!pairs.Add(key))
            {
                throw new ArgumentException(
                    $"Aspecto duplicado: "
                    + $"{aspect.FirstObject}/{aspect.SecondObject}.");
            }

            if (!double.IsFinite(
                    aspect.SeparationDegrees)
                || !double.IsFinite(
                    aspect.ExactAngleDegrees)
                || !double.IsFinite(
                    aspect.DeviationDegrees)
                || !double.IsFinite(
                    aspect.AllowedOrbDegrees)
                || !double.IsFinite(
                    aspect.UsedOrbDegrees))
            {
                throw new ArgumentException(
                    "Aspecto con valores numéricos no finitos.");
            }

            if (aspect.SeparationDegrees
                    is < 0.0 or > 180.0
                || aspect.ExactAngleDegrees
                    is < 0.0 or > 180.0
                || aspect.DeviationDegrees < 0.0
                || aspect.AllowedOrbDegrees < 0.0
                || aspect.UsedOrbDegrees < 0.0)
            {
                throw new ArgumentException(
                    "Aspecto con magnitudes fuera de rango.");
            }

            if (aspect.DeviationDegrees >
                aspect.AllowedOrbDegrees)
            {
                throw new ArgumentException(
                    "El aspecto excede el orbe permitido.");
            }
        }
    }

    private static void ValidateDerivedNode(
        IReadOnlyList<NatalPlacementSnapshot> placements)
    {
        var north =
            placements.Single(x =>
                x.ObjectId ==
                AstrologicalObjectId.NorthTrueNode);

        var south =
            placements.Single(x =>
                x.ObjectId ==
                AstrologicalObjectId.SouthNode);

        var expected =
            (north.LongitudeDegrees + 180.0)
            % 360.0;

        var distance =
            CircularDistance(
                expected,
                south.LongitudeDegrees);

        if (distance >
            NodeToleranceDegrees)
        {
            throw new ArgumentException(
                "Nodo Sur no corresponde a Nodo Norte + 180°.");
        }
    }

    private static void Require(
        IReadOnlyCollection<AstrologicalObjectId> actual,
        AstrologicalObjectId required)
    {
        if (!actual.Contains(required))
        {
            throw new ArgumentException(
                $"Falta objeto natal requerido: {required}.");
        }
    }

    private static double CircularDistance(
        double first,
        double second)
    {
        var difference =
            Math.Abs(first - second)
            % 360.0;

        return Math.Min(
            difference,
            360.0 - difference);
    }
}
