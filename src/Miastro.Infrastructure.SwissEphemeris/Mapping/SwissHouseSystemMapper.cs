using Miastro.Astronomy.Abstractions.Errors;
using Miastro.Domain.Houses;

namespace Miastro.Infrastructure.SwissEphemeris.Mapping;

internal static class SwissHouseSystemMapper
{
    public static int ToSwissCode(
        HouseSystem houseSystem) =>
        houseSystem switch
        {
            HouseSystem.Placidus => 'P',
            HouseSystem.Koch => 'K',

            _ => throw new AstronomyEngineException(
                new AstronomyError(
                    AstronomyErrorCode.InvalidConfiguration,
                    "SWISS_HOUSE_SYSTEM_UNSUPPORTED",
                    "El sistema de casas solicitado no está soportado."),
                $"HouseSystem={houseSystem}")
        };
}
