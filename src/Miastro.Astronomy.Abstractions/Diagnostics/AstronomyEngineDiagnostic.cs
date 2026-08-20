namespace Miastro.Astronomy.Abstractions.Diagnostics;

public sealed record AstronomyEngineDiagnostic(
    bool LibraryAvailable,
    bool LibraryLoaded,
    bool AbiCompatible,
    string? EngineVersion,
    string AdapterVersion,
    string Architecture,
    string? LoadedLibraryPath,
    EphemerisDataStatus EphemerisDataStatus,
    string? TechnicalStatus);
