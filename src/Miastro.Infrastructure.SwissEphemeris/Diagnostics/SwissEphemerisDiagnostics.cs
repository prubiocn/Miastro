using System.Reflection;
using System.Runtime.InteropServices;
using Miastro.Astronomy.Abstractions.Contracts;
using Miastro.Astronomy.Abstractions.Diagnostics;
using Miastro.Astronomy.Abstractions.Errors;
using Miastro.Infrastructure.SwissEphemeris.Configuration;
using Miastro.Infrastructure.SwissEphemeris.Data;
using Miastro.Infrastructure.SwissEphemeris.Loading;

namespace Miastro.Infrastructure.SwissEphemeris.Diagnostics;

public sealed class SwissEphemerisDiagnostics :
    IAstronomyEngineDiagnostics
{
    private readonly SwissEphemerisOptions _options;

    public SwissEphemerisDiagnostics(
        SwissEphemerisOptions options)
    {
        _options =
            options ??
            throw new ArgumentNullException(nameof(options));
    }

    public AstronomyEngineDiagnostic Diagnose()
    {
        var adapterVersion =
            typeof(SwissEphemerisDiagnostics)
                .Assembly
                .GetName()
                .Version?
                .ToString()
            ?? "unknown";

        var architecture =
            RuntimeInformation.ProcessArchitecture
                .ToString();

        if (!File.Exists(
                _options.NativeLibraryPath))
        {
            return new(
                false,
                false,
                false,
                null,
                adapterVersion,
                architecture,
                null,
                EphemerisDataStatus.Unknown,
                "SWISS_LIBRARY_NOT_FOUND");
        }

        try
        {
            using var library =
                SwissNativeLibrary.Load(_options);

            var version =
                library.Api.GetVersion();

            if (!string.Equals(
                    version,
                    _options.ExpectedEngineVersion,
                    StringComparison.Ordinal))
            {
                return new(
                    true,
                    true,
                    false,
                    version,
                    adapterVersion,
                    architecture,
                    library.LibraryPath,
                    EphemerisDataStatus.Unknown,
                    "SWISS_VERSION_UNEXPECTED");
            }

            library.Api.SetEphemerisPath(
                _options.EphemerisPath);

            var dataStatus =
                EphemerisIntegrityValidator.Validate(
                    _options.EphemerisPath);

            return new(
                true,
                true,
                true,
                version,
                adapterVersion,
                architecture,
                library.LibraryPath,
                dataStatus,
                "OK");
        }
        catch (AstronomyEngineException ex)
        {
            return new(
                true,
                false,
                false,
                null,
                adapterVersion,
                architecture,
                null,
                EphemerisDataStatus.Unknown,
                ex.Error.TechnicalCode);
        }
    }
}
