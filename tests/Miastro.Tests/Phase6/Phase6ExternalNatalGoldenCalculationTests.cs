using System.Globalization;
using System.Text.Json;
using Miastro.Astronomy.Abstractions.Models;
using Miastro.Domain.Calculation;
using Miastro.Domain.Houses;
using Miastro.Domain.Natal;
using Miastro.Domain.Objects;
using Miastro.Infrastructure.SwissEphemeris.Calculation;
using Miastro.Infrastructure.SwissEphemeris.Configuration;
using Miastro.Infrastructure.SwissEphemeris.Houses;
using Miastro.Domain.DerivedPoints;

namespace Miastro.Tests.Phase6;

[TestClass]
public sealed class Phase6ExternalNatalGoldenCalculationTests
{
    private const string LibraryHash =
        "47e6fed985ccb5f067b7a0f6f746ec3567a7b54ce5f86140b2138616a8e6a653";

    [TestMethod]
    public void Five_charts_positions_match_external_swetest_goldens()
    {
        using var golden =
            LoadGolden();

        var root =
            RepositoryRoot();

        var calculator =
            CreatePositionCalculator(
                root);

        var tolerances =
            golden.RootElement
                .GetProperty(
                    "tolerances");

        var longitudeTolerance =
            tolerances
                .GetProperty(
                    "longitudeDegrees")
                .GetDouble();

        var latitudeTolerance =
            tolerances
                .GetProperty(
                    "latitudeDegrees")
                .GetDouble();

        var speedTolerance =
            tolerances
                .GetProperty(
                    "longitudeSpeedDegreesPerDay")
                .GetDouble();

        var caseCount = 0;

        foreach (
            var goldenCase
            in golden.RootElement
                .GetProperty("cases")
                .EnumerateArray())
        {
            caseCount++;

            var caseId =
                goldenCase
                    .GetProperty("id")
                    .GetString()
                ?? throw new InvalidOperationException();

            var instant =
                ParseInstant(
                    goldenCase);

            var expectedPositions =
                goldenCase
                    .GetProperty(
                        "positions")
                    .EnumerateArray()
                    .ToArray();

            Assert.AreEqual(
                17,
                expectedPositions.Length,
                caseId);

            foreach (
                var expected
                in expectedPositions)
            {
                var objectId =
                    Enum.Parse<
                        AstrologicalObjectId>(
                        expected
                            .GetProperty(
                                "object")
                            .GetString()!);

                var actual =
                    calculator.Calculate(
                        objectId,
                        instant,
                        CalculationProfile
                            .MiastroV1);

                AssertAngularClose(
                    expected
                        .GetProperty(
                            "longitude")
                        .GetDouble(),
                    actual.Longitude.Degrees,
                    longitudeTolerance,
                    $"{caseId} {objectId} longitude");

                Assert.AreEqual(
                    expected
                        .GetProperty(
                            "latitude")
                        .GetDouble(),
                    actual.LatitudeDegrees,
                    latitudeTolerance,
                    $"{caseId} {objectId} latitude");

                Assert.AreEqual(
                    expected
                        .GetProperty(
                            "longitudeSpeed")
                        .GetDouble(),
                    actual
                        .LongitudeSpeedDegreesPerDay,
                    speedTolerance,
                    $"{caseId} {objectId} speed");

                Assert.AreEqual(
                    "Swiss Ephemeris",
                    actual.EngineMetadata.Engine);

                Assert.AreEqual(
                    "2.10.03",
                    actual.EngineMetadata
                        .EngineVersion);
            }
        }

        Assert.AreEqual(
            5,
            caseCount);
    }

    [TestMethod]
    public void Five_charts_houses_asc_mc_match_external_swetest_goldens()
    {
        using var golden =
            LoadGolden();

        var root =
            RepositoryRoot();

        var calculator =
            CreateHouseCalculator(
                root);

        var tolerances =
            golden.RootElement
                .GetProperty(
                    "tolerances");

        var cuspTolerance =
            tolerances
                .GetProperty(
                    "houseCuspDegrees")
                .GetDouble();

        var ascTolerance =
            tolerances
                .GetProperty(
                    "ascendantDegrees")
                .GetDouble();

        var mcTolerance =
            tolerances
                .GetProperty(
                    "midheavenDegrees")
                .GetDouble();

        var caseCount = 0;

        foreach (
            var goldenCase
            in golden.RootElement
                .GetProperty("cases")
                .EnumerateArray())
        {
            caseCount++;

            var caseId =
                goldenCase
                    .GetProperty("id")
                    .GetString()
                ?? throw new InvalidOperationException();

            var latitude =
                goldenCase
                    .GetProperty(
                        "latitude")
                    .GetDouble();

            var longitude =
                goldenCase
                    .GetProperty(
                        "longitude")
                    .GetDouble();

            var houseSystem =
                Enum.Parse<HouseSystem>(
                    goldenCase
                        .GetProperty(
                            "houseSystem")
                        .GetString()!);

            var actual =
                calculator.Calculate(
                    ParseInstant(
                        goldenCase),
                    new GeographicLocation(
                        latitude,
                        longitude),
                    houseSystem);

            Assert.IsTrue(
                actual.Success,
                caseId);

            Assert.HasCount(
                12,
                actual.Cusps);

            var expected =
                goldenCase
                    .GetProperty(
                        "houses");

            var expectedCusps =
                expected
                    .GetProperty(
                        "cusps")
                    .EnumerateArray()
                    .ToArray();

            for (
                var index = 0;
                index < 12;
                index++)
            {
                Assert.AreEqual(
                    index + 1,
                    expectedCusps[index]
                        .GetProperty(
                            "house")
                        .GetInt32(),
                    $"{caseId} cusp ordinal");

                AssertAngularClose(
                    expectedCusps[index]
                        .GetProperty(
                            "longitude")
                        .GetDouble(),
                    actual.Cusps[index]
                        .Longitude
                        .Degrees,
                    cuspTolerance,
                    $"{caseId} house {index + 1}");
            }

            Assert.IsNotNull(
                actual.Ascendant,
                caseId);

            Assert.IsNotNull(
                actual.Midheaven,
                caseId);

            AssertAngularClose(
                expected
                    .GetProperty(
                        "ascendant")
                    .GetDouble(),
                actual.Ascendant!
                    .Value
                    .Degrees,
                ascTolerance,
                $"{caseId} ASC");

            AssertAngularClose(
                expected
                    .GetProperty(
                        "midheaven")
                    .GetDouble(),
                actual.Midheaven!
                    .Value
                    .Degrees,
                mcTolerance,
                $"{caseId} MC");

            Assert.AreEqual(
                houseSystem,
                actual.HouseSystem);
        }

        Assert.AreEqual(
            5,
            caseCount);
    }

    [TestMethod]
    public void Derived_south_node_matches_external_true_node_in_all_five_charts()
    {
        using var golden =
            LoadGolden();

        var calculator =
            CreatePositionCalculator(
                RepositoryRoot());

        var tolerance =
            golden.RootElement
                .GetProperty(
                    "tolerances")
                .GetProperty(
                    "longitudeDegrees")
                .GetDouble();

        var caseCount = 0;

        foreach (
            var goldenCase
            in golden.RootElement
                .GetProperty("cases")
                .EnumerateArray())
        {
            caseCount++;

            var caseId =
                goldenCase
                    .GetProperty("id")
                    .GetString()
                ?? throw new InvalidOperationException();

            var instant =
                ParseInstant(
                    goldenCase);

            var actualNorth =
                calculator.Calculate(
                    AstrologicalObjectId
                        .NorthTrueNode,
                    instant,
                    CalculationProfile
                        .MiastroV1);

            var actualSouth =
                LunarNodeCalculator
                    .CalculateSouthNode(
                        actualNorth.Longitude);

            var expectedSouth =
                goldenCase
                    .GetProperty(
                        "derived")
                    .GetProperty(
                        "southNodeLongitude")
                    .GetDouble();

            AssertAngularClose(
                expectedSouth,
                actualSouth.Degrees,
                tolerance,
                $"{caseId} South Node");
        }

        Assert.AreEqual(
            5,
            caseCount);
    }

    [TestMethod]
    public void External_coverage_is_observed_in_actual_miastro_results()
    {
        using var golden =
            LoadGolden();

        var calculator =
            CreatePositionCalculator(
                RepositoryRoot());

        var sawRetrograde = false;
        var sawNearZero = false;
        var sawModern = 0;
        var sawHistorical = 0;
        var sawNorth = false;
        var sawSouth = false;
        var sawPlacidus = false;
        var sawKoch = false;

        foreach (
            var goldenCase
            in golden.RootElement
                .GetProperty("cases")
                .EnumerateArray())
        {
            var category =
                goldenCase
                    .GetProperty(
                        "category")
                    .GetString();

            if (category == "modern")
            {
                sawModern++;
            }

            if (category == "historical")
            {
                sawHistorical++;
            }

            var latitude =
                goldenCase
                    .GetProperty(
                        "latitude")
                    .GetDouble();

            sawNorth |=
                latitude > 0.0;

            sawSouth |=
                latitude < 0.0;

            var system =
                goldenCase
                    .GetProperty(
                        "houseSystem")
                    .GetString();

            sawPlacidus |=
                system == "Placidus";

            sawKoch |=
                system == "Koch";

            var instant =
                ParseInstant(
                    goldenCase);

            foreach (
                var expected
                in goldenCase
                    .GetProperty(
                        "positions")
                    .EnumerateArray())
            {
                var objectId =
                    Enum.Parse<
                        AstrologicalObjectId>(
                        expected
                            .GetProperty(
                                "object")
                            .GetString()!);

                var actual =
                    calculator.Calculate(
                        objectId,
                        instant,
                        CalculationProfile
                            .MiastroV1);

                if (
                    actual
                        .LongitudeSpeedDegreesPerDay
                    < 0.0)
                {
                    sawRetrograde =
                        true;
                }

                var longitude =
                    actual.Longitude
                        .Degrees;

                var distance =
                    Math.Min(
                        longitude,
                        360.0 - longitude);

                if (distance <= 1.0)
                {
                    sawNearZero =
                        true;
                }
            }
        }

        Assert.AreEqual(
            3,
            sawModern);

        Assert.AreEqual(
            2,
            sawHistorical);

        Assert.IsTrue(
            sawNorth);

        Assert.IsTrue(
            sawSouth);

        Assert.IsTrue(
            sawPlacidus);

        Assert.IsTrue(
            sawKoch);

        Assert.IsTrue(
            sawRetrograde);

        Assert.IsTrue(
            sawNearZero);
    }

    private static AstronomicalInstant
        ParseInstant(
            JsonElement goldenCase)
        => AstronomicalInstant.FromUtc(
            DateTimeOffset.Parse(
                goldenCase
                    .GetProperty(
                        "instantUtc")
                    .GetString()!,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal
                | DateTimeStyles.AdjustToUniversal));

    private static void AssertAngularClose(
        double expected,
        double actual,
        double tolerance,
        string message)
    {
        var difference =
            Math.Abs(
                expected - actual);

        difference =
            Math.Min(
                difference,
                360.0 - difference);

        Assert.IsLessThanOrEqualTo(
            tolerance,
            difference,
            $"{message}: expected={expected:R}, actual={actual:R}");
    }

    private static JsonDocument
        LoadGolden()
    {
        var path =
            Path.Combine(
                RepositoryRoot(),
                "tests",
                "golden",
                "phase6",
                "golden-values.json");

        return JsonDocument.Parse(
            File.ReadAllText(
                path));
    }

    private static
        SwissEphemerisPositionCalculator
        CreatePositionCalculator(
            string root)
        => new(
            Options(root));

    private static
        SwissEphemerisHouseCalculator
        CreateHouseCalculator(
            string root)
        => new(
            Options(root));

    private static SwissEphemerisOptions
        Options(
            string root)
        => new(
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

    private static string RepositoryRoot()
    {
        var directory =
            new DirectoryInfo(
                AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (
                File.Exists(
                    Path.Combine(
                        directory.FullName,
                        "Miastro.sln")))
            {
                return directory.FullName;
            }

            directory =
                directory.Parent;
        }

        throw new
            DirectoryNotFoundException();
    }
}
