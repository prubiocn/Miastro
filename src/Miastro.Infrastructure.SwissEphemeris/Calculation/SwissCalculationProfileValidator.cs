using Miastro.Astronomy.Abstractions.Errors;
using Miastro.Domain.Calculation;
using Miastro.Domain.Objects;

namespace Miastro.Infrastructure.SwissEphemeris.Calculation;

internal static class SwissCalculationProfileValidator
{
    public static void EnsureMiastroV1(
        CalculationProfile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);

        var valid =
            profile.Zodiac ==
                ZodiacMode.Tropical
            &&
            profile.ReferenceFrame ==
                ReferenceFrame.Geocentric
            &&
            profile.Coordinate ==
                CoordinateType.EclipticLongitude
            &&
            profile.PositionMode ==
                ApparentPositionMode.Apparent
            &&
            profile.IncludeSpeed
            &&
            !profile.Topocentric
            &&
            profile.NodeConvention ==
                NodeConvention.TrueNode
            &&
            profile.LilithVariant ==
                LilithVariant.Mean;

        if (!valid)
        {
            throw new AstronomyEngineException(
                new AstronomyError(
                    AstronomyErrorCode.InvalidConfiguration,
                    "SWISS_PROFILE_UNSUPPORTED",
                    "El perfil de cálculo solicitado no está soportado."),
                $"Profile={profile.Id}");
        }
    }
}
