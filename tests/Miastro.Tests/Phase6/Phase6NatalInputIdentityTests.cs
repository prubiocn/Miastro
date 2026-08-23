using Miastro.Domain.Houses;
using Miastro.Domain.Natal;
using Miastro.Domain.People;

namespace Miastro.Tests.Phase6;

[TestClass]
public sealed class Phase6NatalInputIdentityTests
{
    [TestMethod]
    public void Exact_and_approximate_have_different_hashes()
    {
        var exact =
            Input(
                BirthTimePrecision.Exact);

        var approximate =
            exact with
            {
                TimePrecision =
                    BirthTimePrecision.Approximate
            };

        Assert.AreNotEqual(
            NatalInputHash.Compute(exact),
            NatalInputHash.Compute(approximate));
    }

    [TestMethod]
    public void Locality_identity_change_has_different_hash()
    {
        var first =
            Input(
                BirthTimePrecision.Exact);

        var second =
            first with
            {
                GeoNameId = 123456,
                Locality = "Otra localidad"
            };

        Assert.AreNotEqual(
            NatalInputHash.Compute(first),
            NatalInputHash.Compute(second));
    }

    [TestMethod]
    public void Historical_resolution_change_has_different_hash()
    {
        var first =
            Input(
                BirthTimePrecision.Exact);

        var second =
            first with
            {
                HistoricalOffsetSeconds = 7200,
                InstantUtc =
                    first.InstantUtc
                        .AddHours(-1)
            };

        Assert.AreNotEqual(
            NatalInputHash.Compute(first),
            NatalInputHash.Compute(second));
    }

    [TestMethod]
    public void Identical_semantic_input_has_identical_hash()
    {
        var first =
            Input(
                BirthTimePrecision.Exact);

        var second =
            first with { };

        Assert.AreEqual(
            NatalInputHash.Compute(first),
            NatalInputHash.Compute(second));
    }

    private static NatalInputFingerprint Input(
        BirthTimePrecision precision)
        => new(
            new DateOnly(
                2000, 1, 1),
            new TimeOnly(
                12, 0),
            new DateTimeOffset(
                2000, 1, 1,
                11, 0, 0,
                TimeSpan.Zero),
            40.4168,
            -3.7038,
            "Europe/Madrid",
            "TZDB: 2026c",
            HouseSystem.Placidus,
            "miastro-v1",
            "Swiss Ephemeris",
            "2.10.03",
            "ephemeris-test",
            precision,
            3117735,
            "Madrid",
            3600,
            null);
}
