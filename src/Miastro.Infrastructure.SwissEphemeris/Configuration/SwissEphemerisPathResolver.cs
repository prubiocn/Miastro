namespace Miastro.Infrastructure.SwissEphemeris.Configuration;

public static class SwissEphemerisPathResolver
{
    public const string DistributionNativeLibrary =
        "/usr/lib/miastro/native/libswe.so";

    public const string DistributionEphemerisPath =
        "/usr/share/miastro/ephemeris";

    public static SwissEphemerisOptions
        FromApplicationBaseDirectory(
            string expectedSha256,
            string expectedEngineVersion = "2.10.03")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            expectedSha256);

        var baseDirectory =
            AppContext.BaseDirectory;

        return new(
            Path.Combine(
                baseDirectory,
                "native",
                "linux-x64",
                "libswe.so"),
            Path.Combine(
                baseDirectory,
                "ephemeris"),
            expectedSha256,
            expectedEngineVersion);
    }

    public static SwissEphemerisOptions
        FromDistributionLayout(
            string expectedSha256,
            string expectedEngineVersion = "2.10.03")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            expectedSha256);

        return new(
            DistributionNativeLibrary,
            DistributionEphemerisPath,
            expectedSha256,
            expectedEngineVersion);
    }
}
