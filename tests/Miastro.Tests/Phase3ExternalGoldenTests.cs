using System.Text.Json;
using Miastro.Astronomy.Abstractions.Models;
using Miastro.Domain.Calculation;
using Miastro.Domain.Houses;
using Miastro.Domain.Objects;
using Miastro.Infrastructure.SwissEphemeris.Calculation;
using Miastro.Infrastructure.SwissEphemeris.Configuration;
using Miastro.Infrastructure.SwissEphemeris.Houses;

namespace Miastro.Tests;

[TestClass]
public sealed class Phase3ExternalGoldenTests
{
    private const string LibraryHash =
        "47e6fed985ccb5f067b7a0f6f746ec3567a7b54ce5f86140b2138616a8e6a653";

    [TestMethod]
    public void Modern_positions_match_external_astrodienst_reference()
    {
        using var golden = LoadGolden();

        var root = FindRepositoryRoot();
        var calculator = CreatePositionCalculator(root);

        var section =
            golden.RootElement.GetProperty("modern");

        var instant =
            AstronomicalInstant.FromUtc(
                DateTimeOffset.Parse(
                    section
                        .GetProperty("instantUtc")
                        .GetString()!,
                    System.Globalization.CultureInfo.InvariantCulture));

        var tolerances =
            golden.RootElement.GetProperty("tolerances");

        var longitudeTolerance =
            tolerances
                .GetProperty("longitudeDegrees")
                .GetDouble();

        var latitudeTolerance =
            tolerances
                .GetProperty("latitudeDegrees")
                .GetDouble();

        var speedTolerance =
            tolerances
                .GetProperty(
                    "longitudeSpeedDegreesPerDay")
                .GetDouble();

        foreach (
            var expected
            in section
                .GetProperty("positions")
                .EnumerateArray())
        {
            var objectId =
                Enum.Parse<AstrologicalObjectId>(
                    expected
                        .GetProperty("object")
                        .GetString()!);

            var actual =
                calculator.Calculate(
                    objectId,
                    instant,
                    CalculationProfile.MiastroV1);

            AssertAngularClose(
                expected
                    .GetProperty("longitude")
                    .GetDouble(),
                actual.Longitude.Degrees,
                longitudeTolerance,
                objectId.ToString());

            Assert.AreEqual(
                expected
                    .GetProperty("latitude")
                    .GetDouble(),
                actual.LatitudeDegrees,
                latitudeTolerance,
                $"{objectId} latitude");

            Assert.AreEqual(
                expected
                    .GetProperty("longitudeSpeed")
                    .GetDouble(),
                actual.LongitudeSpeedDegreesPerDay,
                speedTolerance,
                $"{objectId} speed");
        }
    }

    [TestMethod]
    public void Historical_positions_match_external_reference()
    {
        using var golden = LoadGolden();

        var root = FindRepositoryRoot();
        var calculator = CreatePositionCalculator(root);

        var section =
            golden.RootElement.GetProperty("historical");

        var instant =
            AstronomicalInstant.FromUtc(
                DateTimeOffset.Parse(
                    section
                        .GetProperty("instantUtc")
                        .GetString()!,
                    System.Globalization.CultureInfo.InvariantCulture));

        var tolerance =
            golden.RootElement
                .GetProperty("tolerances")
                .GetProperty("longitudeDegrees")
                .GetDouble();

        foreach (
            var expected
            in section
                .GetProperty("positions")
                .EnumerateArray())
        {
            var objectId =
                Enum.Parse<AstrologicalObjectId>(
                    expected
                        .GetProperty("object")
                        .GetString()!);

            var actual =
                calculator.Calculate(
                    objectId,
                    instant,
                    CalculationProfile.MiastroV1);

            AssertAngularClose(
                expected
                    .GetProperty("longitude")
                    .GetDouble(),
                actual.Longitude.Degrees,
                tolerance,
                $"historical {objectId}");
        }
    }

    [TestMethod]
    public void Mercury_retrograde_matches_external_reference()
    {
        using var golden = LoadGolden();

        var root = FindRepositoryRoot();

        var section =
            golden.RootElement.GetProperty("retrograde");

        var instant =
            AstronomicalInstant.FromUtc(
                DateTimeOffset.Parse(
                    section
                        .GetProperty("instantUtc")
                        .GetString()!,
                    System.Globalization.CultureInfo.InvariantCulture));

        var expected =
            section.GetProperty("position");

        var actual =
            CreatePositionCalculator(root)
                .Calculate(
                    AstrologicalObjectId.Mercury,
                    instant,
                    CalculationProfile.MiastroV1);

        Assert.IsLessThan(
            0.0,
            expected
                .GetProperty("longitudeSpeed")
                .GetDouble());

        Assert.IsLessThan(
            0.0,
            actual.LongitudeSpeedDegreesPerDay);
    }

    [TestMethod]
    [DataRow("madridPlacidus", HouseSystem.Placidus, 40.4168, -3.7038)]
    [DataRow("madridKoch", HouseSystem.Koch, 40.4168, -3.7038)]
    [DataRow("sydneyPlacidus", HouseSystem.Placidus, -33.8688, 151.2093)]
    [DataRow("sydneyKoch", HouseSystem.Koch, -33.8688, 151.2093)]
    public void Houses_match_external_astrodienst_reference(
        string caseName,
        HouseSystem system,
        double latitude,
        double longitude)
    {
        using var golden = LoadGolden();

        var root = FindRepositoryRoot();

        var houses =
            golden.RootElement.GetProperty("houses");

        var instant =
            AstronomicalInstant.FromUtc(
                DateTimeOffset.Parse(
                    houses
                        .GetProperty("instantUtc")
                        .GetString()!,
                    System.Globalization.CultureInfo.InvariantCulture));

        var expected =
            houses.GetProperty(caseName);

        var tolerance =
            golden.RootElement
                .GetProperty("tolerances")
                .GetProperty("houseCuspDegrees")
                .GetDouble();

        var actual =
            CreateHouseCalculator(root)
                .Calculate(
                    instant,
                    new GeographicLocation(
                        latitude,
                        longitude),
                    system);

        Assert.IsTrue(actual.Success);
        Assert.HasCount(12, actual.Cusps);

        var expectedCusps =
            expected
                .GetProperty("cusps")
                .EnumerateArray()
                .ToArray();

        for (var i = 0; i < 12; i++)
        {
            AssertAngularClose(
                expectedCusps[i]
                    .GetProperty("longitude")
                    .GetDouble(),
                actual.Cusps[i]
                    .Longitude
                    .Degrees,
                tolerance,
                $"{caseName} house {i + 1}");
        }

        AssertAngularClose(
            expected
                .GetProperty("ascendant")
                .GetDouble(),
            actual.Ascendant!.Value.Degrees,
            tolerance,
            $"{caseName} ASC");

        AssertAngularClose(
            expected
                .GetProperty("midheaven")
                .GetDouble(),
            actual.Midheaven!.Value.Degrees,
            tolerance,
            $"{caseName} MC");
    }

    private static void AssertAngularClose(
        double expected,
        double actual,
        double tolerance,
        string message)
    {
        var difference =
            Math.Abs(expected - actual);

        difference =
            Math.Min(
                difference,
                360.0 - difference);

        Assert.IsLessThanOrEqualTo(
            tolerance,
            difference,
            $"{message}: expected={expected:R}, actual={actual:R}");
    }

    private static JsonDocument LoadGolden()
    {
        var path =
            Path.Combine(
                FindRepositoryRoot(),
                "tests",
                "golden",
                "phase3",
                "golden-values.json");

        return JsonDocument.Parse(
            File.ReadAllText(path));
    }

    private static SwissEphemerisPositionCalculator
        CreatePositionCalculator(
            string root) =>
        new(
            Options(root));

    private static SwissEphemerisHouseCalculator
        CreateHouseCalculator(
            string root) =>
        new(
            Options(root));

    private static SwissEphemerisOptions Options(
        string root) =>
        new(
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
            "2.10.03");

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

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException();
    }
}
