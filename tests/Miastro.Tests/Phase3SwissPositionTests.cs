using Miastro.Astronomy.Abstractions.Errors;
using Miastro.Astronomy.Abstractions.Models;
using Miastro.Domain.Calculation;
using Miastro.Domain.Objects;
using Miastro.Infrastructure.SwissEphemeris.Calculation;
using Miastro.Infrastructure.SwissEphemeris.Configuration;

namespace Miastro.Tests;

[TestClass]
public sealed class Phase3SwissPositionTests
{
    private const string ExpectedHash =
        "47e6fed985ccb5f067b7a0f6f746ec3567a7b54ce5f86140b2138616a8e6a653";

    [TestMethod]
    public void Julian_day_reference_date_is_reproducible_through_calculation()
    {
        var calculator =
            CreateCalculator();

        var instant =
            AstronomicalInstant.FromUtc(
                new DateTimeOffset(
                    2012, 1, 1,
                    0, 0, 0,
                    TimeSpan.Zero));

        var first =
            calculator.Calculate(
                AstrologicalObjectId.Sun,
                instant,
                CalculationProfile.MiastroV1);

        var second =
            calculator.Calculate(
                AstrologicalObjectId.Sun,
                instant,
                CalculationProfile.MiastroV1);

        Assert.AreEqual(first, second);
    }

    [TestMethod]
    [DataRow(AstrologicalObjectId.Sun)]
    [DataRow(AstrologicalObjectId.Moon)]
    [DataRow(AstrologicalObjectId.Mercury)]
    [DataRow(AstrologicalObjectId.Venus)]
    [DataRow(AstrologicalObjectId.Mars)]
    [DataRow(AstrologicalObjectId.Jupiter)]
    [DataRow(AstrologicalObjectId.Saturn)]
    [DataRow(AstrologicalObjectId.Uranus)]
    [DataRow(AstrologicalObjectId.Neptune)]
    [DataRow(AstrologicalObjectId.Pluto)]
    [DataRow(AstrologicalObjectId.NorthTrueNode)]
    [DataRow(AstrologicalObjectId.MeanLilith)]
    [DataRow(AstrologicalObjectId.Chiron)]
    [DataRow(AstrologicalObjectId.Ceres)]
    [DataRow(AstrologicalObjectId.Pallas)]
    [DataRow(AstrologicalObjectId.Juno)]
    [DataRow(AstrologicalObjectId.Vesta)]
    public void All_phase3_bodies_return_real_swiss_positions(
        AstrologicalObjectId objectId)
    {
        var calculator =
            CreateCalculator();

        var instant =
            AstronomicalInstant.FromUtc(
                new DateTimeOffset(
                    2024, 1, 1,
                    12, 0, 0,
                    TimeSpan.Zero));

        var result =
            calculator.Calculate(
                objectId,
                instant,
                CalculationProfile.MiastroV1);

        Assert.AreEqual(
            objectId,
            result.ObjectId);

        Assert.IsGreaterThanOrEqualTo(
            0.0,
            result.Longitude.Degrees);

        Assert.IsLessThan(
            360.0,
            result.Longitude.Degrees);

        Assert.IsTrue(
            double.IsFinite(
                result.LatitudeDegrees));

        Assert.IsTrue(
            double.IsFinite(
                result.DistanceAu));

        Assert.IsTrue(
            double.IsFinite(
                result.LongitudeSpeedDegreesPerDay));

        Assert.IsTrue(
            double.IsFinite(
                result.LatitudeSpeedDegreesPerDay));

        Assert.IsTrue(
            double.IsFinite(
                result.DistanceSpeedAuPerDay));

        Assert.AreEqual(
            "Swiss Ephemeris",
            result.EngineMetadata.Engine);

        Assert.AreEqual(
            "2.10.03",
            result.EngineMetadata.EngineVersion);

        CollectionAssert.Contains(
            result.AppliedFlags.ToArray(),
            "SEFLG_SWIEPH");

        CollectionAssert.Contains(
            result.AppliedFlags.ToArray(),
            "SEFLG_SPEED");
    }

    [TestMethod]
    public void South_node_is_not_a_native_supported_body()
    {
        var calculator =
            CreateCalculator();

        var ex =
            Assert.ThrowsExactly<AstronomyEngineException>(
                () => calculator.Calculate(
                    AstrologicalObjectId.SouthNode,
                    AstronomicalInstant.FromUtc(
                        DateTimeOffset.UtcNow),
                    CalculationProfile.MiastroV1));

        Assert.AreEqual(
            AstronomyErrorCode.UnsupportedObject,
            ex.Error.Code);
    }

    [TestMethod]
    public void Calculations_preserve_full_precision()
    {
        var result =
            CreateCalculator()
                .Calculate(
                    AstrologicalObjectId.Mercury,
                    AstronomicalInstant.FromUtc(
                        new DateTimeOffset(
                            2024, 1, 1,
                            12, 34, 56,
                            TimeSpan.Zero)),
                    CalculationProfile.MiastroV1);

        Assert.AreNotEqual(
            Math.Round(
                result.Longitude.Degrees,
                2),
            result.Longitude.Degrees);

        Assert.AreNotEqual(
            Math.Round(
                result.LongitudeSpeedDegreesPerDay,
                2),
            result.LongitudeSpeedDegreesPerDay);
    }

    private static SwissEphemerisPositionCalculator
        CreateCalculator()
    {
        var root =
            FindRepositoryRoot();

        return new(
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
                ExpectedHash,
                "2.10.03"));
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
