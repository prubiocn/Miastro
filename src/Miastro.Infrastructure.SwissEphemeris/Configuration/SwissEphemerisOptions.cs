namespace Miastro.Infrastructure.SwissEphemeris.Configuration;

public sealed record SwissEphemerisOptions
{
    public string NativeLibraryPath { get; }

    public string EphemerisPath { get; }

    public string? ExpectedSha256 { get; }

    public string ExpectedEngineVersion { get; }

    public SwissEphemerisOptions(
        string nativeLibraryPath,
        string ephemerisPath,
        string? expectedSha256,
        string expectedEngineVersion)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nativeLibraryPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(ephemerisPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(expectedEngineVersion);

        NativeLibraryPath =
            Path.GetFullPath(nativeLibraryPath);

        EphemerisPath =
            Path.GetFullPath(ephemerisPath);

        ExpectedSha256 =
            string.IsNullOrWhiteSpace(expectedSha256)
                ? null
                : expectedSha256.Trim().ToLowerInvariant();

        ExpectedEngineVersion =
            expectedEngineVersion.Trim();
    }
}
