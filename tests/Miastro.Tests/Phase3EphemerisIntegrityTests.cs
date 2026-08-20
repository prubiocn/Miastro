using Miastro.Astronomy.Abstractions.Errors;
using Miastro.Astronomy.Abstractions.Models;
using Miastro.Domain.Calculation;
using Miastro.Domain.Objects;
using Miastro.Infrastructure.SwissEphemeris.Calculation;
using Miastro.Infrastructure.SwissEphemeris.Configuration;

namespace Miastro.Tests;

[TestClass]
public sealed class Phase3EphemerisIntegrityTests
{
    private const string LibraryHash =
        "47e6fed985ccb5f067b7a0f6f746ec3567a7b54ce5f86140b2138616a8e6a653";

    [TestMethod]
    public void Missing_ephemeris_is_explicit()
    {
        var root =
            FindRepositoryRoot();

        var calculator =
            new SwissEphemerisPositionCalculator(
                new SwissEphemerisOptions(
                    Library(root),
                    Path.Combine(
                        root,
                        "does-not-exist",
                        "ephemeris"),
                    LibraryHash,
                    "2.10.03"));

        var ex =
            Assert.ThrowsExactly<AstronomyEngineException>(
                () => calculator.Calculate(
                    AstrologicalObjectId.Sun,
                    Instant(),
                    CalculationProfile.MiastroV1));

        Assert.AreEqual(
            AstronomyErrorCode.EphemerisFileMissing,
            ex.Error.Code);
    }

    [TestMethod]
    public void Corrupt_ephemeris_is_explicit()
    {
        var root =
            FindRepositoryRoot();

        var source =
            Path.Combine(
                root,
                "data",
                "ephemeris");

        var temp =
            Path.Combine(
                Path.GetTempPath(),
                "miastro-phase3-corrupt-" +
                Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(temp);

        try
        {
            foreach (var file in Directory.GetFiles(source))
            {
                File.Copy(
                    file,
                    Path.Combine(
                        temp,
                        Path.GetFileName(file)));
            }

            File.AppendAllText(
                Path.Combine(
                    temp,
                    "sepl_18.se1"),
                "corruption");

            var calculator =
                new SwissEphemerisPositionCalculator(
                    new SwissEphemerisOptions(
                        Library(root),
                        temp,
                        LibraryHash,
                        "2.10.03"));

            var ex =
                Assert.ThrowsExactly<AstronomyEngineException>(
                    () => calculator.Calculate(
                        AstrologicalObjectId.Sun,
                        Instant(),
                        CalculationProfile.MiastroV1));

            Assert.AreEqual(
                AstronomyErrorCode.EphemerisFileCorrupt,
                ex.Error.Code);
        }
        finally
        {
            Directory.Delete(
                temp,
                recursive: true);
        }
    }

    [TestMethod]
    public void Wrong_manifest_hash_is_explicit()
    {
        var root =
            FindRepositoryRoot();

        var source =
            Path.Combine(
                root,
                "data",
                "ephemeris");

        var temp =
            Path.Combine(
                Path.GetTempPath(),
                "miastro-phase3-hash-" +
                Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(temp);

        try
        {
            foreach (var file in Directory.GetFiles(source))
            {
                File.Copy(
                    file,
                    Path.Combine(
                        temp,
                        Path.GetFileName(file)));
            }

            var manifest =
                Path.Combine(
                    temp,
                    "manifest.json");

            var text =
                File.ReadAllText(manifest);

            var marker =
                "\"sha256\": \"";

            var index =
                text.IndexOf(
                    marker,
                    StringComparison.Ordinal);

            Assert.IsGreaterThanOrEqualTo(
                0,
                index);

            index += marker.Length;

            text =
                text[..index] +
                new string('0', 64) +
                text[(index + 64)..];

            File.WriteAllText(
                manifest,
                text);

            var calculator =
                new SwissEphemerisPositionCalculator(
                    new SwissEphemerisOptions(
                        Library(root),
                        temp,
                        LibraryHash,
                        "2.10.03"));

            var ex =
                Assert.ThrowsExactly<AstronomyEngineException>(
                    () => calculator.Calculate(
                        AstrologicalObjectId.Sun,
                        Instant(),
                        CalculationProfile.MiastroV1));

            Assert.AreEqual(
                AstronomyErrorCode.EphemerisFileCorrupt,
                ex.Error.Code);
        }
        finally
        {
            Directory.Delete(
                temp,
                recursive: true);
        }
    }

    private static AstronomicalInstant Instant() =>
        AstronomicalInstant.FromUtc(
            new DateTimeOffset(
                2024, 1, 1,
                12, 0, 0,
                TimeSpan.Zero));

    private static string Library(
        string root) =>
        Path.Combine(
            root,
            "src",
            "Miastro.Infrastructure.SwissEphemeris",
            "native",
            "linux-x64",
            "libswe.so");

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
