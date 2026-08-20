using System.Security.Cryptography;
using System.Text.Json;
using Miastro.Astronomy.Abstractions.Diagnostics;
using Miastro.Astronomy.Abstractions.Errors;
using Miastro.Astronomy.Abstractions.Models;

namespace Miastro.Infrastructure.SwissEphemeris.Data;

internal static class EphemerisIntegrityValidator
{
    public static EphemerisDataStatus Validate(
        string ephemerisPath)
    {
        if (!TryLoadValidManifest(
                ephemerisPath,
                out _,
                out var status))
        {
            return status;
        }

        return EphemerisDataStatus.Available;
    }

    public static void EnsureAvailable(
        string ephemerisPath)
    {
        var status =
            Validate(ephemerisPath);

        switch (status)
        {
            case EphemerisDataStatus.Available:
                return;

            case EphemerisDataStatus.Missing:
                throw Create(
                    AstronomyErrorCode.EphemerisFileMissing,
                    "SWISS_EPHEMERIS_DATA_MISSING",
                    "Faltan datos necesarios del motor astronómico.",
                    ephemerisPath);

            case EphemerisDataStatus.Corrupt:
                throw Create(
                    AstronomyErrorCode.EphemerisFileCorrupt,
                    "SWISS_EPHEMERIS_DATA_CORRUPT",
                    "Los datos del motor astronómico no superan la validación de integridad.",
                    ephemerisPath);

            default:
                throw Create(
                    AstronomyErrorCode.InvalidConfiguration,
                    "SWISS_EPHEMERIS_DATA_UNKNOWN",
                    "El estado de los datos astronómicos no es válido.",
                    status.ToString());
        }
    }

    public static void EnsureSupportsInstant(
        string ephemerisPath,
        AstronomicalInstant instant)
    {
        if (!TryLoadValidManifest(
                ephemerisPath,
                out var manifest,
                out var status))
        {
            EnsureAvailable(ephemerisPath);
            return;
        }

        if (manifest!.SupportedRange is null)
        {
            throw Create(
                AstronomyErrorCode.InvalidConfiguration,
                "SWISS_EPHEMERIS_RANGE_UNDECLARED",
                "El rango temporal de los datos astronómicos no está declarado.",
                ephemerisPath);
        }

        var utc =
            instant.Utc;

        if (utc < manifest.SupportedRange.FromUtc ||
            utc > manifest.SupportedRange.ToUtc)
        {
            throw Create(
                AstronomyErrorCode.UnsupportedTimeRange,
                "SWISS_EPHEMERIS_TIME_RANGE_UNSUPPORTED",
                "El instante solicitado está fuera del rango temporal disponible.",
                $"Instant={utc:O}; " +
                $"From={manifest.SupportedRange.FromUtc:O}; " +
                $"To={manifest.SupportedRange.ToUtc:O}");
        }
    }

    private static bool TryLoadValidManifest(
        string ephemerisPath,
        out EphemerisManifest? manifest,
        out EphemerisDataStatus status)
    {
        manifest = null;

        if (string.IsNullOrWhiteSpace(ephemerisPath) ||
            !Directory.Exists(ephemerisPath))
        {
            status =
                EphemerisDataStatus.Missing;

            return false;
        }

        var manifestPath =
            Path.Combine(
                ephemerisPath,
                "manifest.json");

        if (!File.Exists(manifestPath))
        {
            status =
                EphemerisDataStatus.Missing;

            return false;
        }

        try
        {
            manifest =
                JsonSerializer.Deserialize<EphemerisManifest>(
                    File.ReadAllText(manifestPath),
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
        }
        catch (
            Exception ex
            ) when (
                ex is JsonException
                or IOException
                or UnauthorizedAccessException)
        {
            status =
                EphemerisDataStatus.Corrupt;

            return false;
        }

        if (manifest is null ||
            manifest.SchemaVersion != 1 ||
            manifest.Files is null ||
            manifest.SupportedRange is null ||
            manifest.SupportedRange.FromUtc >=
                manifest.SupportedRange.ToUtc)
        {
            status =
                EphemerisDataStatus.Corrupt;

            return false;
        }

        foreach (var file in manifest.Files)
        {
            if (!file.Required)
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(file.Name) ||
                Path.GetFileName(file.Name) != file.Name)
            {
                status =
                    EphemerisDataStatus.Corrupt;

                return false;
            }

            var fullPath =
                Path.Combine(
                    ephemerisPath,
                    file.Name);

            if (!File.Exists(fullPath))
            {
                status =
                    EphemerisDataStatus.Missing;

                return false;
            }

            var info =
                new FileInfo(fullPath);

            if (info.Length != file.Size)
            {
                status =
                    EphemerisDataStatus.Corrupt;

                return false;
            }

            using var stream =
                File.OpenRead(fullPath);

            var hash =
                Convert.ToHexString(
                    SHA256.HashData(stream))
                .ToLowerInvariant();

            if (!string.Equals(
                    hash,
                    file.Sha256,
                    StringComparison.OrdinalIgnoreCase))
            {
                status =
                    EphemerisDataStatus.Corrupt;

                return false;
            }
        }

        status =
            EphemerisDataStatus.Available;

        return true;
    }

    private static AstronomyEngineException Create(
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
