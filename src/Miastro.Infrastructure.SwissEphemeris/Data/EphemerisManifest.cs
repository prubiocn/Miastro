namespace Miastro.Infrastructure.SwissEphemeris.Data;

internal sealed record EphemerisManifest(
    int SchemaVersion,
    string Engine,
    string EngineVersion,
    EphemerisSupportedRange? SupportedRange,
    IReadOnlyList<EphemerisManifestFile> Files);

internal sealed record EphemerisSupportedRange(
    DateTimeOffset FromUtc,
    DateTimeOffset ToUtc);

internal sealed record EphemerisManifestFile(
    string Name,
    long Size,
    string Sha256,
    string Version,
    string Range,
    bool Required,
    string Purpose);
