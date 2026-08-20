using Miastro.Astronomy.Abstractions.Errors;
using Miastro.Astronomy.Abstractions.Models;
using Miastro.Domain.Calculation;
using Miastro.Domain.Objects;
using Miastro.Infrastructure.SwissEphemeris.Calculation;
using Miastro.Infrastructure.SwissEphemeris.Configuration;

namespace Miastro.Tests;

[TestClass]
public sealed class Phase3RangeAndNativeArchitectureTests
{
    private const string LibraryHash =
        "47e6fed985ccb5f067b7a0f6f746ec3567a7b54ce5f86140b2138616a8e6a653";

    [TestMethod]
    public void Unsupported_time_range_is_rejected_before_native_calculation()
    {
        var root =
            FindRepositoryRoot();

        var calculator =
            new SwissEphemerisPositionCalculator(
                new SwissEphemerisOptions(
                    Path.Combine(
                        root,
                        "src",
                        "Miastro.Infrastructure.SwissEphemeris",
                        "native",
                        "linux-x64",
                        "libswe.so"),
                    Path.Combine(
                        root,
                        "data",
                        "ephemeris"),
                    LibraryHash,
                    "2.10.03"));

        var ex =
            Assert.ThrowsExactly<AstronomyEngineException>(
                () => calculator.Calculate(
                    AstrologicalObjectId.Sun,
                    AstronomicalInstant.FromUtc(
                        new DateTimeOffset(
                            1700, 1, 1,
                            0, 0, 0,
                            TimeSpan.Zero)),
                    CalculationProfile.MiastroV1));

        Assert.AreEqual(
            AstronomyErrorCode.UnsupportedTimeRange,
            ex.Error.Code);
    }

    [TestMethod]
    public void Distribution_paths_are_not_personal_paths()
    {
        Assert.IsFalse(
            SwissEphemerisPathResolver
                .DistributionNativeLibrary
                .Contains(
                    "/home/",
                    StringComparison.Ordinal));

        Assert.IsFalse(
            SwissEphemerisPathResolver
                .DistributionEphemerisPath
                .Contains(
                    "/home/",
                    StringComparison.Ordinal));
    }

    [TestMethod]
    public void Swiss_native_boundary_exists_only_in_swiss_adapter()
    {
        var root =
            FindRepositoryRoot();

        var src =
            Path.Combine(
                root,
                "src");

        var forbiddenMarkers =
            new[]
            {
                "UnmanagedFunctionPointer",
                "NativeLibrary.Load(",
                "NativeLibrary.GetExport(",
                "swe_calc_ut",
                "swe_houses_ex",
                "swe_set_ephe_path"
            };

        var violations =
            Directory
                .EnumerateFiles(
                    src,
                    "*.cs",
                    SearchOption.AllDirectories)
                .Where(path =>
                    !path.Contains(
                        $"{Path.DirectorySeparatorChar}Miastro.Infrastructure.SwissEphemeris{Path.DirectorySeparatorChar}",
                        StringComparison.Ordinal))
                .Where(path =>
                {
                    var text =
                        File.ReadAllText(path);

                    return forbiddenMarkers.Any(
                        marker =>
                            text.Contains(
                                marker,
                                StringComparison.Ordinal));
                })
                .ToArray();

        Assert.HasCount(
            0,
            violations,
            string.Join(
                Environment.NewLine,
                violations));
    }

    private static string FindRepositoryRoot()
    {
        var directory =
            new DirectoryInfo(
                AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(
                    Path.Combine(
                        directory.FullName,
                        "Miastro.sln")))
            {
                return directory.FullName;
            }

            directory =
                directory.Parent;
        }

        throw new DirectoryNotFoundException();
    }
}
