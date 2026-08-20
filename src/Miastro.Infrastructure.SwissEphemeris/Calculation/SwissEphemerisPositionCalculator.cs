using System.Reflection;
using System.Runtime.InteropServices;
using Miastro.Astronomy.Abstractions.Contracts;
using Miastro.Astronomy.Abstractions.Errors;
using Miastro.Astronomy.Abstractions.Models;
using Miastro.Domain.Angles;
using Miastro.Domain.Calculation;
using Miastro.Domain.Objects;
using Miastro.Infrastructure.SwissEphemeris.Configuration;
using Miastro.Infrastructure.SwissEphemeris.Data;
using Miastro.Infrastructure.SwissEphemeris.Loading;
using Miastro.Infrastructure.SwissEphemeris.Mapping;
using Miastro.Infrastructure.SwissEphemeris.Runtime;
using Miastro.Infrastructure.SwissEphemeris.Time;

namespace Miastro.Infrastructure.SwissEphemeris.Calculation;

public sealed class SwissEphemerisPositionCalculator :
    IEclipticPositionCalculator
{
    private readonly SwissEphemerisOptions _options;

    public SwissEphemerisPositionCalculator(
        SwissEphemerisOptions options)
    {
        _options =
            options ??
            throw new ArgumentNullException(
                nameof(options));
    }

    public EclipticPosition Calculate(
        AstrologicalObjectId objectId,
        AstronomicalInstant instant,
        CalculationProfile profile)
    {
        SwissCalculationProfileValidator
            .EnsureMiastroV1(profile);

        var swissBody =
            SwissObjectMapper.ToSwissId(
                objectId);

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
                throw Error(
                    AstronomyErrorCode.UnexpectedEngineVersion,
                    "SWISS_VERSION_UNEXPECTED",
                    "La versión del motor astronómico no es la esperada.",
                    $"Expected={_options.ExpectedEngineVersion}; Actual={version}");
            }

            EphemerisIntegrityValidator.EnsureAvailable(
                _options.EphemerisPath);

            EphemerisIntegrityValidator.EnsureSupportsInstant(
                _options.EphemerisPath,
                instant);

            api.SetEphemerisPath(
                _options.EphemerisPath);

            var julianDay =
                SwissJulianDayConverter
                    .ToJulianDayUt(
                        instant,
                        api);

            var native =
                api.CalculateUt(
                    julianDay,
                    swissBody,
                    SwissCalculationFlags.MiastroV1);

            if (native.ReturnedFlags < 0)
            {
                throw Error(
                    AstronomyErrorCode.CalculationFailed,
                    "SWISS_CALCULATION_FAILED",
                    "No se pudo realizar el cálculo astronómico.",
                    native.NativeError);
            }

            // Swiss puede degradar a Moshier si no encuentra
            // efemérides. Miastro V1 no acepta esa degradación.
            if ((native.ReturnedFlags &
                 SwissCalculationFlags.SwissEphemeris) == 0)
            {
                throw Error(
                    AstronomyErrorCode.EphemerisFileMissing,
                    "SWISS_EPHEMERIS_FALLBACK_REJECTED",
                    "Faltan datos necesarios del motor astronómico.",
                    $"ReturnedFlags={native.ReturnedFlags}; " +
                    $"Native={native.NativeError}");
            }

            if ((native.ReturnedFlags &
                 SwissCalculationFlags.Speed) == 0)
            {
                throw Error(
                    AstronomyErrorCode.InvalidResult,
                    "SWISS_SPEED_NOT_RETURNED",
                    "El motor astronómico devolvió un resultado incompleto.",
                    $"ReturnedFlags={native.ReturnedFlags}");
            }

            ValidateValues(
                native.Values);

            return new EclipticPosition(
                objectId,
                EclipticLongitude.FromDegrees(
                    native.Values[0]),
                native.Values[1],
                native.Values[2],
                native.Values[3],
                native.Values[4],
                native.Values[5],
                instant,
                ReferenceFrame.Geocentric,
                SwissCalculationFlags.MiastroV1Names,
                CreateMetadata(version));
        }
    }

    private static void ValidateValues(
        double[] values)
    {
        if (values.Length != 6 ||
            values.Any(value =>
                !double.IsFinite(value)))
        {
            throw Error(
                AstronomyErrorCode.InvalidResult,
                "SWISS_INVALID_RESULT",
                "El motor astronómico devolvió un resultado inválido.",
                "Expected 6 finite doubles.");
        }
    }

    private static AstronomyEngineMetadata CreateMetadata(
        string engineVersion)
    {
        var adapterVersion =
            typeof(
                SwissEphemerisPositionCalculator)
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

    private static AstronomyEngineException Error(
        AstronomyErrorCode code,
        string technicalCode,
        string safeMessage,
        string technicalDetail) =>
        new(
            new AstronomyError(
                code,
                technicalCode,
                safeMessage),
            technicalDetail);
}
