using System.Text.Json;
using Miastro.Domain.Angles;
using Miastro.Domain.Charts;
using Miastro.Domain.DerivedPoints;
using Miastro.Domain.Natal;
using Miastro.Domain.Objects;
using Miastro.Domain.Placements;
using Miastro.Domain.Houses;

namespace Miastro.Tests.Phase6;

[TestClass]
public sealed class Phase6DerivedNatalGoldenTests
{
    private const double DerivedTolerance =
        1e-9;

    [TestMethod]
    public void Five_charts_sect_and_fortune_match_independent_reference()
    {
        using var primary =
            Load(
                "golden-values.json");

        using var derived =
            Load(
                "derived-golden-values.json");

        foreach (
            var expectedCase
            in derived.RootElement
                .GetProperty("cases")
                .EnumerateArray())
        {
            var id =
                expectedCase
                    .GetProperty("id")
                    .GetString()!;

            var primaryCase =
                FindCase(
                    primary.RootElement,
                    id);

            var positions =
                Positions(
                    primaryCase);

            var cusps =
                Cusps(
                    primaryCase);

            var sect =
                NatalChartSectResolver.Resolve(
                    EclipticLongitude
                        .FromDegrees(
                            positions["Sun"]),
                    cusps);

            var expectedSect =
                expectedCase
                    .GetProperty("sect")
                    .GetString();

            Assert.AreEqual(
                expectedSect,
                sect.ToString(),
                id);

            var fortune =
                PartOfFortuneCalculator
                    .Calculate(
                        EclipticLongitude
                            .FromDegrees(
                                primaryCase
                                    .GetProperty(
                                        "houses")
                                    .GetProperty(
                                        "ascendant")
                                    .GetDouble()),
                        EclipticLongitude
                            .FromDegrees(
                                positions["Sun"]),
                        EclipticLongitude
                            .FromDegrees(
                                positions["Moon"]),
                        sect);

            AssertAngularClose(
                expectedCase
                    .GetProperty(
                        "partOfFortuneLongitude")
                    .GetDouble(),
                fortune.Degrees,
                DerivedTolerance,
                $"{id} Fortuna");
        }
    }

    [TestMethod]
    public void Five_charts_house_occupations_match_independent_reference()
    {
        using var primary =
            Load(
                "golden-values.json");

        using var derived =
            Load(
                "derived-golden-values.json");

        foreach (
            var expectedCase
            in derived.RootElement
                .GetProperty("cases")
                .EnumerateArray())
        {
            var id =
                expectedCase
                    .GetProperty("id")
                    .GetString()!;

            var primaryCase =
                FindCase(
                    primary.RootElement,
                    id);

            var longitudes =
                FullLongitudes(
                    primaryCase,
                    expectedCase);

            var cusps =
                Cusps(
                    primaryCase);

            var assignments =
                expectedCase
                    .GetProperty(
                        "houseAssignments")
                    .EnumerateArray()
                    .ToArray();

            Assert.AreEqual(
                21,
                assignments.Length,
                id);

            foreach (
                var expected
                in assignments)
            {
                var objectName =
                    expected
                        .GetProperty(
                            "object")
                        .GetString()!;

                var expectedHouse =
                    expected
                        .GetProperty(
                            "house")
                        .GetInt32();

                var actual =
                    NatalHousePlacementResolver
                        .Resolve(
                            EclipticLongitude
                                .FromDegrees(
                                    longitudes[
                                        objectName]),
                            cusps);

                Assert.AreEqual(
                    expectedHouse,
                    actual.Number,
                    $"{id} {objectName}");
            }
        }
    }

    [TestMethod]
    public void Five_charts_aspects_match_independent_reference()
    {
        using var primary =
            Load(
                "golden-values.json");

        using var derived =
            Load(
                "derived-golden-values.json");

        foreach (
            var expectedCase
            in derived.RootElement
                .GetProperty("cases")
                .EnumerateArray())
        {
            var id =
                expectedCase
                    .GetProperty("id")
                    .GetString()!;

            var primaryCase =
                FindCase(
                    primary.RootElement,
                    id);

            var longitudes =
                FullLongitudes(
                    primaryCase,
                    expectedCase);

            var placements =
                NatalObjectOrder.All
                    .Select(
                        objectId =>
                            new AstrologicalPlacement(
                                objectId,
                                EclipticLongitude
                                    .FromDegrees(
                                        longitudes[
                                            objectId
                                                .ToString()]),
                                speedDegreesPerDay:
                                    1.0))
                    .ToArray();

            var actual =
                NatalAspectCalculator
                    .Calculate(
                        placements);

            var expected =
                expectedCase
                    .GetProperty(
                        "aspects")
                    .EnumerateArray()
                    .ToArray();

            Assert.AreEqual(
                expected.Length,
                actual.Count,
                $"{id} aspect count");

            for (
                var index = 0;
                index < expected.Length;
                index++)
            {
                var e =
                    expected[index];

                var a =
                    actual[index];

                Assert.AreEqual(
                    e.GetProperty(
                        "firstObject")
                        .GetString(),
                    a.FirstObject
                        .ToString(),
                    $"{id} aspect {index} first");

                Assert.AreEqual(
                    e.GetProperty(
                        "secondObject")
                        .GetString(),
                    a.SecondObject
                        .ToString(),
                    $"{id} aspect {index} second");

                Assert.AreEqual(
                    e.GetProperty(
                        "kind")
                        .GetString(),
                    a.Definition.Kind
                        .ToString(),
                    $"{id} aspect {index} kind");

                Assert.AreEqual(
                    e.GetProperty(
                        "separationDegrees")
                        .GetDouble(),
                    a.Separation.Degrees,
                    DerivedTolerance,
                    $"{id} aspect {index} separation");

                Assert.AreEqual(
                    e.GetProperty(
                        "exactAngleDegrees")
                        .GetDouble(),
                    a.ExactAngleDegrees,
                    DerivedTolerance,
                    $"{id} aspect {index} exact");

                Assert.AreEqual(
                    e.GetProperty(
                        "deviationDegrees")
                        .GetDouble(),
                    a.DeviationDegrees,
                    DerivedTolerance,
                    $"{id} aspect {index} deviation");

                Assert.AreEqual(
                    e.GetProperty(
                        "allowedOrbDegrees")
                        .GetDouble(),
                    a.AllowedOrbDegrees,
                    DerivedTolerance,
                    $"{id} aspect {index} allowed orb");

                Assert.AreEqual(
                    e.GetProperty(
                        "usedOrbDegrees")
                        .GetDouble(),
                    a.UsedOrbDegrees,
                    DerivedTolerance,
                    $"{id} aspect {index} used orb");
            }
        }
    }

    [TestMethod]
    public void Independent_reference_covers_day_night_and_unequal_houses()
    {
        using var primary =
            Load(
                "golden-values.json");

        using var derived =
            Load(
                "derived-golden-values.json");

        var cases =
            derived.RootElement
                .GetProperty("cases")
                .EnumerateArray()
                .ToArray();

        Assert.IsTrue(
            cases.Any(x =>
                x.GetProperty("sect")
                    .GetString()
                == "Day"));

        Assert.IsTrue(
            cases.Any(x =>
                x.GetProperty("sect")
                    .GetString()
                == "Night"));

        foreach (
            var goldenCase
            in primary.RootElement
                .GetProperty("cases")
                .EnumerateArray())
        {
            var cusps =
                goldenCase
                    .GetProperty(
                        "houses")
                    .GetProperty(
                        "cusps")
                    .EnumerateArray()
                    .Select(x =>
                        x.GetProperty(
                            "longitude")
                            .GetDouble())
                    .ToArray();

            var widths =
                Enumerable
                    .Range(0, 12)
                    .Select(index =>
                    {
                        var next =
                            (index + 1) % 12;

                        var width =
                            (
                                cusps[next]
                                - cusps[index]
                            )
                            % 360.0;

                        if (width < 0.0)
                        {
                            width += 360.0;
                        }

                        return width;
                    })
                    .ToArray();

            Assert.IsTrue(
                widths.Max()
                - widths.Min()
                > 0.001,
                goldenCase
                    .GetProperty("id")
                    .GetString());
        }
    }

    private static Dictionary<
        string,
        double>
        Positions(
            JsonElement primaryCase)
        => primaryCase
            .GetProperty("positions")
            .EnumerateArray()
            .ToDictionary(
                x =>
                    x.GetProperty(
                        "object")
                        .GetString()!,
                x =>
                    x.GetProperty(
                        "longitude")
                        .GetDouble(),
                StringComparer.Ordinal);

    private static Dictionary<
        string,
        double>
        FullLongitudes(
            JsonElement primaryCase,
            JsonElement derivedCase)
    {
        var result =
            Positions(
                primaryCase);

        var houses =
            primaryCase
                .GetProperty(
                    "houses");

        result["Ascendant"] =
            houses
                .GetProperty(
                    "ascendant")
                .GetDouble();

        result["Midheaven"] =
            houses
                .GetProperty(
                    "midheaven")
                .GetDouble();

        result["SouthNode"] =
            (
                result[
                    "NorthTrueNode"]
                + 180.0
            )
            % 360.0;

        result["PartOfFortune"] =
            derivedCase
                .GetProperty(
                    "partOfFortuneLongitude")
                .GetDouble();

        return result;
    }

    private static IReadOnlyList<
        HouseCusp>
        Cusps(
            JsonElement primaryCase)
        => primaryCase
            .GetProperty("houses")
            .GetProperty("cusps")
            .EnumerateArray()
            .Select(x =>
                new HouseCusp(
                    AstrologicalHouse
                        .FromNumber(
                            x.GetProperty(
                                "house")
                                .GetInt32()),
                    EclipticLongitude
                        .FromDegrees(
                            x.GetProperty(
                                "longitude")
                                .GetDouble())))
            .ToArray();

    private static JsonElement FindCase(
        JsonElement root,
        string id)
        => root
            .GetProperty("cases")
            .EnumerateArray()
            .Single(x =>
                string.Equals(
                    x.GetProperty("id")
                        .GetString(),
                    id,
                    StringComparison.Ordinal));

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

    private static JsonDocument Load(
        string file)
        => JsonDocument.Parse(
            File.ReadAllText(
                Path.Combine(
                    RepositoryRoot(),
                    "tests",
                    "golden",
                    "phase6",
                    file)));

    private static string RepositoryRoot()
        => Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "../../../../../"));
}
