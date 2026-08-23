using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Miastro.Application.Natal;
using Miastro.Astronomy.Abstractions.Contracts;
using Miastro.Infrastructure.SwissEphemeris.Calculation;
using Miastro.Infrastructure.SwissEphemeris.Configuration;
using Miastro.Infrastructure.SwissEphemeris.Diagnostics;
using Miastro.Infrastructure.SwissEphemeris.Houses;

namespace Miastro.Bootstrap;

internal static class Phase6AstronomyComposition
{
    private const string ExpectedSwissVersion =
        "2.10.03";

    public static IServiceCollection
        AddPhase6Astronomy(
            this IServiceCollection services)
    {
        services.AddSingleton(
            _ => CreateSwissOptions());

        services.AddSingleton<
            IEclipticPositionCalculator,
            SwissEphemerisPositionCalculator>();

        services.AddSingleton<
            IHouseCalculator,
            SwissEphemerisHouseCalculator>();

        services.AddSingleton<
            IAstronomyEngineDiagnostics,
            SwissEphemerisDiagnostics>();

        services.AddSingleton<
            INatalCalculationMetadataProvider,
            Phase6NatalCalculationMetadataProvider>();

        return services;
    }

    private static SwissEphemerisOptions
        CreateSwissOptions()
    {
        var nativeLibrary =
            ResolveExistingFile(
                "MIASTRO_SWISS_NATIVE_LIBRARY",
                [
                    Path.Combine(
                        AppContext.BaseDirectory,
                        "native",
                        "linux-x64",
                        "libswe.so"),

                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "src",
                        "Miastro.Infrastructure.SwissEphemeris",
                        "native",
                        "linux-x64",
                        "libswe.so"),

                    "/usr/lib/miastro/native/libswe.so"
                ]);

        var ephemerisDirectory =
            ResolveExistingDirectory(
                "MIASTRO_EPHEMERIS_DIR",
                [
                    Path.Combine(
                        AppContext.BaseDirectory,
                        "ephemeris"),

                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "data",
                        "ephemeris"),

                    "/usr/share/miastro/ephemeris"
                ]);

        var nativeHash =
            TryReadNativeSha256(
                nativeLibrary);

        return new SwissEphemerisOptions(
            nativeLibrary,
            ephemerisDirectory,
            nativeHash,
            ExpectedSwissVersion);
    }

    private static string ResolveExistingFile(
        string environmentVariable,
        IReadOnlyList<string> candidates)
    {
        var configured =
            Environment.GetEnvironmentVariable(
                environmentVariable);

        if (!string.IsNullOrWhiteSpace(
                configured))
        {
            var path =
                Path.GetFullPath(
                    configured);

            if (!File.Exists(path))
            {
                throw new FileNotFoundException(
                    $"No existe {environmentVariable}.",
                    path);
            }

            return path;
        }

        foreach (var candidate in candidates)
        {
            var path =
                Path.GetFullPath(
                    candidate);

            if (File.Exists(path))
            {
                return path;
            }
        }

        throw new FileNotFoundException(
            "No se encontró la biblioteca nativa Swiss Ephemeris.");
    }

    private static string ResolveExistingDirectory(
        string environmentVariable,
        IReadOnlyList<string> candidates)
    {
        var configured =
            Environment.GetEnvironmentVariable(
                environmentVariable);

        if (!string.IsNullOrWhiteSpace(
                configured))
        {
            var path =
                Path.GetFullPath(
                    configured);

            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException(
                    $"No existe {environmentVariable}: {path}");
            }

            return path;
        }

        foreach (var candidate in candidates)
        {
            var path =
                Path.GetFullPath(
                    candidate);

            if (Directory.Exists(path)
                && File.Exists(
                    Path.Combine(
                        path,
                        "manifest.json")))
            {
                return path;
            }
        }

        throw new DirectoryNotFoundException(
            "No se encontró el directorio de efemérides.");
    }

    private static string? TryReadNativeSha256(
        string nativeLibrary)
    {
        var manifests =
            new[]
            {
                Path.Combine(
                    Path.GetDirectoryName(
                        nativeLibrary)
                    ?? string.Empty,
                    "native-manifest.json"),

                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "third_party",
                    "swisseph",
                    "native-manifest.json")
            };

        foreach (var manifestPath in manifests)
        {
            if (!File.Exists(
                manifestPath))
            {
                continue;
            }

            using var document =
                JsonDocument.Parse(
                    File.ReadAllText(
                        manifestPath));

            if (document.RootElement
                .TryGetProperty(
                    "sha256",
                    out var sha256))
            {
                var value =
                    sha256.GetString();

                if (!string.IsNullOrWhiteSpace(
                    value))
                {
                    return value.Trim()
                        .ToLowerInvariant();
                }
            }
        }

        return null;
    }

    private sealed class
        Phase6NatalCalculationMetadataProvider(
            SwissEphemerisOptions options,
            IAstronomyEngineDiagnostics diagnostics)
        : INatalCalculationMetadataProvider
    {
        public NatalCalculationEnvironment Get()
        {
            var diagnostic =
                diagnostics.Diagnose();

            if (!diagnostic.LibraryAvailable
                || !diagnostic.LibraryLoaded
                || !diagnostic.AbiCompatible
                || string.IsNullOrWhiteSpace(
                    diagnostic.EngineVersion))
            {
                throw new InvalidOperationException(
                    "Swiss Ephemeris no está disponible.");
            }

            var ephemerisVersion =
                ReadEphemerisIdentity(
                    options.EphemerisPath);

            var miastroVersion =
                Assembly
                    .GetEntryAssembly()?
                    .GetName()
                    .Version?
                    .ToString()
                ?? typeof(
                        Phase6AstronomyComposition)
                    .Assembly
                    .GetName()
                    .Version?
                    .ToString()
                ?? "unknown";

            return new(
                miastroVersion,
                "Swiss Ephemeris",
                diagnostic.EngineVersion,
                diagnostic.AdapterVersion,
                ephemerisVersion);
        }

        private static string
            ReadEphemerisIdentity(
                string ephemerisPath)
        {
            var manifestPath =
                Path.Combine(
                    ephemerisPath,
                    "manifest.json");

            if (!File.Exists(
                manifestPath))
            {
                throw new FileNotFoundException(
                    "Falta el manifiesto de efemérides.",
                    manifestPath);
            }

            var bytes =
                File.ReadAllBytes(
                    manifestPath);

            var hash =
                Convert.ToHexString(
                    System.Security.Cryptography
                        .SHA256.HashData(bytes))
                    .ToLowerInvariant();

            return $"manifest-sha256:{hash}";
        }
    }
}
