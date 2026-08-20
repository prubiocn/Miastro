using Miastro.Astronomy.Abstractions.Errors;
using Miastro.Astronomy.Abstractions.Models;
using Miastro.Domain.Angles;
using Miastro.Domain.Houses;
using Miastro.Infrastructure.SwissEphemeris.Configuration;
using Miastro.Infrastructure.SwissEphemeris.Houses;

namespace Miastro.Tests;

[TestClass]
public sealed class Phase3SwissHouseTests
{
    private const string ExpectedHash =
        "47e6fed985ccb5f067b7a0f6f746ec3567a7b54ce5f86140b2138616a8e6a653";

    [TestMethod]
    [DataRow(HouseSystem.Placidus)]
    [DataRow(HouseSystem.Koch)]
    public void Normal_latitude_returns_twelve_real_cusps(
        HouseSystem system)
    {
        var calculator =
            CreateCalculator();

        var result =
            calculator.Calculate(
                ReferenceInstant(),
                new GeographicLocation(
                    40.4168,
                    -3.7038),
                system);

        Assert.IsTrue(result.Success);
        Assert.IsNull(result.Error);
        Assert.HasCount(12, result.Cusps);
        Assert.IsNotNull(result.Ascendant);
        Assert.IsNotNull(result.Midheaven);
        Assert.IsNotNull(result.Descendant);
        Assert.IsNotNull(result.ImumCoeli);

        foreach (var cusp in result.Cusps)
        {
            Assert.IsGreaterThanOrEqualTo(
                0.0,
                cusp.Longitude.Degrees);

            Assert.IsLessThan(
                360.0,
                cusp.Longitude.Degrees);
        }
    }

    [TestMethod]
    [DataRow(HouseSystem.Placidus)]
    [DataRow(HouseSystem.Koch)]
    public void Asc_mc_ic_dsc_are_normalized_and_coherent(
        HouseSystem system)
    {
        var result =
            CreateCalculator()
                .Calculate(
                    ReferenceInstant(),
                    new GeographicLocation(
                        -33.8688,
                        151.2093),
                    system);

        Assert.IsTrue(result.Success);

        var asc =
            result.Ascendant!.Value;

        var dsc =
            result.Descendant!.Value;

        var mc =
            result.Midheaven!.Value;

        var ic =
            result.ImumCoeli!.Value;

        Assert.AreEqual(
            180.0,
            AngularSeparation
                .Between(asc, dsc)
                .Degrees,
            1e-10);

        Assert.AreEqual(
            180.0,
            AngularSeparation
                .Between(mc, ic)
                .Degrees,
            1e-10);

        Assert.IsGreaterThanOrEqualTo(
            0.0,
            dsc.Degrees);

        Assert.IsLessThan(
            360.0,
            dsc.Degrees);

        Assert.IsGreaterThanOrEqualTo(
            0.0,
            ic.Degrees);

        Assert.IsLessThan(
            360.0,
            ic.Degrees);
    }

    [TestMethod]
    public void House_calculation_is_reproducible()
    {
        var calculator =
            CreateCalculator();

        var instant =
            ReferenceInstant();

        var location =
            new GeographicLocation(
                40.4168,
                -3.7038);

        var first =
            calculator.Calculate(
                instant,
                location,
                HouseSystem.Placidus);

        var second =
            calculator.Calculate(
                instant,
                location,
                HouseSystem.Placidus);

        Assert.AreEqual(
            first.Success,
            second.Success);

        Assert.AreEqual(
            first.HouseSystem,
            second.HouseSystem);

        Assert.AreEqual(
            first.Location,
            second.Location);

        Assert.AreEqual(
            first.Instant,
            second.Instant);

        Assert.AreEqual(
            first.Ascendant,
            second.Ascendant);

        Assert.AreEqual(
            first.Midheaven,
            second.Midheaven);

        Assert.AreEqual(
            first.Descendant,
            second.Descendant);

        Assert.AreEqual(
            first.ImumCoeli,
            second.ImumCoeli);

        Assert.AreEqual(
            first.EngineMetadata,
            second.EngineMetadata);

        Assert.AreEqual(
            first.Error,
            second.Error);

        Assert.HasCount(
            first.Cusps.Count,
            second.Cusps);

        for (var i = 0; i < first.Cusps.Count; i++)
        {
            Assert.AreEqual(
                first.Cusps[i],
                second.Cusps[i]);
        }
    }

    [TestMethod]
    [DataRow(HouseSystem.Placidus)]
    [DataRow(HouseSystem.Koch)]
    public void Polar_failure_is_explicit_when_swiss_reports_unavailable(
        HouseSystem system)
    {
        var result =
            CreateCalculator()
                .Calculate(
                    ReferenceInstant(),
                    new GeographicLocation(
                        89.0,
                        0.0),
                    system);

        Assert.IsFalse(result.Success);
        Assert.HasCount(0, result.Cusps);
        Assert.IsNull(result.Ascendant);
        Assert.IsNull(result.Midheaven);
        Assert.IsNull(result.Descendant);
        Assert.IsNull(result.ImumCoeli);
        Assert.IsNotNull(result.Error);

        Assert.AreEqual(
            AstronomyErrorCode
                .HouseCalculationUnavailable,
            result.Error.Code);

        Assert.AreEqual(
            "SWISS_HOUSES_UNAVAILABLE",
            result.Error.TechnicalCode);
    }

    private static AstronomicalInstant
        ReferenceInstant() =>
        AstronomicalInstant.FromUtc(
            new DateTimeOffset(
                2024, 1, 1,
                12, 0, 0,
                TimeSpan.Zero));

    private static SwissEphemerisHouseCalculator
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
