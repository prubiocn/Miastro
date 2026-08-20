using System.Runtime.InteropServices;
using Miastro.Astronomy.Abstractions.Contracts;
using Miastro.Astronomy.Abstractions.Errors;
using Miastro.Astronomy.Abstractions.Models;
using Miastro.Domain.Angles;
using Miastro.Domain.Charts;
using Miastro.Domain.Houses;
using Miastro.Infrastructure.SwissEphemeris.Configuration;
using Miastro.Infrastructure.SwissEphemeris.Loading;
using Miastro.Infrastructure.SwissEphemeris.Mapping;
using Miastro.Infrastructure.SwissEphemeris.Runtime;
using Miastro.Infrastructure.SwissEphemeris.Time;

namespace Miastro.Infrastructure.SwissEphemeris.Houses;

public sealed class SwissEphemerisHouseCalculator :
    IHouseCalculator
{
    private const int SwissSuccess = 0;

    private readonly SwissEphemerisOptions _options;

    public SwissEphemerisHouseCalculator(
        SwissEphemerisOptions options)
    {
        _options =
            options ??
            throw new ArgumentNullException(
                nameof(options));
    }

    public HouseCalculationResult Calculate(
        AstronomicalInstant instant,
        GeographicLocation location,
        HouseSystem houseSystem)
    {
        var houseCode =
            SwissHouseSystemMapper
                .ToSwissCode(houseSystem);

        lock (SwissEphemerisGate.SyncRoot)
        {
            using var library =
                SwissNativeLibrary.Load(
                    _options);

            var api =
                library.Api;

            var version =
                api.GetVersion();

            if (!string.Equals(
                    version,
                    _options.ExpectedEngineVersion,
                    StringComparison.Ordinal))
            {
                return Failed(
                    houseSystem,
                    location,
                    instant,
                    AstronomyErrorCode.UnexpectedEngineVersion,
                    "SWISS_VERSION_UNEXPECTED",
                    "La versión del motor astronómico no es la esperada.");
            }

            var julianDay =
                SwissJulianDayConverter
                    .ToJulianDayUt(
                        instant,
                        api);

            var native =
                api.CalculateHouses(
                    julianDay,
                    location.LatitudeDegrees,
                    location.LongitudeDegrees,
                    houseCode);

            // Swiss devuelve ERR para Placidus/Koch no
            // calculables en determinadas latitudes.
            // Miastro descarta cualquier fallback/resultado
            // parcial y devuelve fallo explícito.
            if (native.ReturnCode != SwissSuccess)
            {
                return Failed(
                    houseSystem,
                    location,
                    instant,
                    AstronomyErrorCode.HouseCalculationUnavailable,
                    "SWISS_HOUSES_UNAVAILABLE",
                    "El sistema de casas no está disponible para esta localización e instante.");
            }

            if (!ValidateNativeResult(native))
            {
                return Failed(
                    houseSystem,
                    location,
                    instant,
                    AstronomyErrorCode.InvalidResult,
                    "SWISS_HOUSES_INVALID_RESULT",
                    "El motor astronómico devolvió un resultado de casas inválido.");
            }

            var cusps =
                Enumerable
                    .Range(1, 12)
                    .Select(number =>
                        new HouseCusp(
                            AstrologicalHouse
                                .FromNumber(number),
                            EclipticLongitude
                                .FromDegrees(
                                    native.Cusps[number])))
                    .ToArray();

            var ascendant =
                EclipticLongitude.FromDegrees(
                    native.Auxiliary[0]);

            var midheaven =
                EclipticLongitude.FromDegrees(
                    native.Auxiliary[1]);

            return HouseCalculationResult
                .Succeeded(
                    houseSystem,
                    cusps,
                    ascendant,
                    midheaven,
                    location,
                    instant,
                    CreateMetadata(version));
        }
    }

    private static bool ValidateNativeResult(
        Interop.NativeHouseResult result)
    {
        if (result.Cusps.Length < 13 ||
            result.Auxiliary.Length < 2)
        {
            return false;
        }

        for (var i = 1; i <= 12; i++)
        {
            if (!double.IsFinite(
                    result.Cusps[i]))
            {
                return false;
            }
        }

        return
            double.IsFinite(
                result.Auxiliary[0])
            &&
            double.IsFinite(
                result.Auxiliary[1]);
    }

    private static AstronomyEngineMetadata
        CreateMetadata(
            string engineVersion)
    {
        var adapterVersion =
            typeof(
                SwissEphemerisHouseCalculator)
                .Assembly
                .GetName()
                .Version?
                .ToString()
            ?? "unknown";

        return new(
            "Swiss Ephemeris",
            engineVersion,
            adapterVersion,
            RuntimeInformation
                .ProcessArchitecture
                .ToString());
    }

    private static HouseCalculationResult Failed(
        HouseSystem houseSystem,
        GeographicLocation location,
        AstronomicalInstant instant,
        AstronomyErrorCode code,
        string technicalCode,
        string safeMessage) =>
        HouseCalculationResult.Failed(
            houseSystem,
            location,
            instant,
            new AstronomyError(
                code,
                technicalCode,
                safeMessage));
}
