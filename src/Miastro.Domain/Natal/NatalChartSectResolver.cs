using Miastro.Domain.Angles;
using Miastro.Domain.Charts;
using Miastro.Domain.DerivedPoints;

namespace Miastro.Domain.Natal;

public static class NatalChartSectResolver
{
    public static ChartSect Resolve(
        EclipticLongitude sunLongitude,
        IReadOnlyList<HouseCusp> cusps)
    {
        var house =
            NatalHousePlacementResolver.Resolve(
                sunLongitude,
                cusps);

        // Casas 7–12: hemisferio superior del círculo local.
        // Casas 1–6: hemisferio inferior.
        //
        // Exactamente sobre DSC (cúspide 7) => Day.
        // Exactamente sobre ASC (cúspide 1) => Night.
        return house.Number is >= 7 and <= 12
            ? ChartSect.Day
            : ChartSect.Night;
    }
}
