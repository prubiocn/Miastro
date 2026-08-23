using Miastro.Domain.Angles;
using Miastro.Domain.Charts;
using Miastro.Domain.Houses;

namespace Miastro.Domain.Natal;

public static class NatalHousePlacementResolver
{
    // Tolerancia exclusivamente numérica.
    // No representa un orbe astrológico.
    public const double CuspToleranceDegrees = 1e-9;

    public static AstrologicalHouse Resolve(
        EclipticLongitude longitude,
        IReadOnlyList<HouseCusp> cusps)
    {
        ArgumentNullException.ThrowIfNull(cusps);

        ValidateCusps(cusps);

        var ordered =
            cusps
                .OrderBy(x => x.House.Number)
                .ToArray();

        var position =
            longitude.Degrees;

        for (var i = 0; i < 12; i++)
        {
            var current =
                ordered[i];

            var next =
                ordered[(i + 1) % 12];

            var start =
                current.Longitude.Degrees;

            var end =
                next.Longitude.Degrees;

            // Regla explícita:
            // exactamente sobre cúspide => casa que comienza allí.
            if (CircularDistance(
                    position,
                    start)
                <= CuspToleranceDegrees)
            {
                return current.House;
            }

            var arc =
                ForwardDistance(
                    start,
                    end);

            var offset =
                ForwardDistance(
                    start,
                    position);

            // Intervalo abierto en el extremo final.
            // Así la cúspide siguiente pertenece a la casa siguiente.
            if (offset > CuspToleranceDegrees
                && offset < arc - CuspToleranceDegrees)
            {
                return current.House;
            }
        }

        // Si el valor está dentro de tolerancia de una cúspide,
        // la segunda pasada garantiza la regla "casa que comienza".
        foreach (var cusp in ordered)
        {
            if (CircularDistance(
                    position,
                    cusp.Longitude.Degrees)
                <= CuspToleranceDegrees)
            {
                return cusp.House;
            }
        }

        throw new InvalidOperationException(
            "No se pudo asignar la longitud a una casa.");
    }

    private static void ValidateCusps(
        IReadOnlyList<HouseCusp> cusps)
    {
        if (cusps.Count != 12)
        {
            throw new ArgumentException(
                "Se requieren exactamente 12 cúspides.",
                nameof(cusps));
        }

        var houses =
            cusps
                .Select(x => x.House.Number)
                .Order()
                .ToArray();

        if (!houses.SequenceEqual(
            Enumerable.Range(1, 12)))
        {
            throw new ArgumentException(
                "Las cúspides deben contener las casas 1 a 12.",
                nameof(cusps));
        }

        for (var i = 0; i < cusps.Count; i++)
        {
            for (var j = i + 1; j < cusps.Count; j++)
            {
                if (CircularDistance(
                        cusps[i].Longitude.Degrees,
                        cusps[j].Longitude.Degrees)
                    <= CuspToleranceDegrees)
                {
                    throw new ArgumentException(
                        "Dos casas no pueden comenzar en la misma longitud.",
                        nameof(cusps));
                }
            }
        }
    }

    private static double ForwardDistance(
        double start,
        double end)
    {
        var value =
            (end - start) % 360.0;

        if (value < 0.0)
        {
            value += 360.0;
        }

        return value;
    }

    private static double CircularDistance(
        double first,
        double second)
    {
        var difference =
            Math.Abs(first - second) % 360.0;

        return Math.Min(
            difference,
            360.0 - difference);
    }
}
