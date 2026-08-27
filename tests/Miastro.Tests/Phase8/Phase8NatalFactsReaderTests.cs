using Miastro.Application.Natal;
using Miastro.Application.Natal.Reading;
using Miastro.Domain.Natal;
using Miastro.Domain.Objects;
using Miastro.Domain.Placements;
using Miastro.Domain.Zodiac;

namespace Miastro.Tests.Phase8;

[TestClass]
public sealed class Phase8NatalFactsReaderTests
{
    [TestMethod]
    public void Facts_are_sorted_by_canonical_natal_object_order()
    {
        var placements =
            new[]
            {
                Placement(
                    AstrologicalObjectId.Mars,
                    15.0,
                    ZodiacSign.Aries,
                    1),

                Placement(
                    AstrologicalObjectId.Sun,
                    45.0,
                    ZodiacSign.Taurus,
                    2),

                Placement(
                    AstrologicalObjectId.Moon,
                    75.0,
                    ZodiacSign.Gemini,
                    3)
            };

        var result =
            NatalFactsReader.Read(
                placements,
                StandardCusps());

        CollectionAssert.AreEqual(
            new[]
            {
                AstrologicalObjectId.Sun,
                AstrologicalObjectId.Moon,
                AstrologicalObjectId.Mars
            },
            result
                .Select(x => x.ObjectId)
                .ToArray());
    }

    [TestMethod]
    public void Double_rulerships_are_preserved()
    {
        var placements =
            new[]
            {
                Placement(
                    AstrologicalObjectId.Mars,
                    215.0,
                    ZodiacSign.Scorpio,
                    8),

                Placement(
                    AstrologicalObjectId.Saturn,
                    305.0,
                    ZodiacSign.Aquarius,
                    11),

                Placement(
                    AstrologicalObjectId.Jupiter,
                    335.0,
                    ZodiacSign.Pisces,
                    12)
            };

        var result =
            NatalFactsReader.Read(
                placements,
                StandardCusps());

        CollectionAssert.AreEqual(
            new[]
            {
                AstrologicalObjectId.Mars,
                AstrologicalObjectId.Pluto
            },
            result
                .Single(
                    x => x.ObjectId
                        == AstrologicalObjectId.Mars)
                .SignRulers
                .ToArray());

        CollectionAssert.AreEqual(
            new[]
            {
                AstrologicalObjectId.Saturn,
                AstrologicalObjectId.Uranus
            },
            result
                .Single(
                    x => x.ObjectId
                        == AstrologicalObjectId.Saturn)
                .SignRulers
                .ToArray());

        CollectionAssert.AreEqual(
            new[]
            {
                AstrologicalObjectId.Jupiter,
                AstrologicalObjectId.Neptune
            },
            result
                .Single(
                    x => x.ObjectId
                        == AstrologicalObjectId.Jupiter)
                .SignRulers
                .ToArray());
    }

    [TestMethod]
    public void House_ruler_comes_from_actual_house_cusp_sign()
    {
        var cusps =
            StandardCusps()
                .Select(
                    cusp =>
                        cusp.HouseNumber == 7
                            ? new NatalHouseCuspSnapshot(
                                7,
                                35.0)
                            : cusp)
                .ToArray();

        var result =
            NatalFactsReader.Read(
                new[]
                {
                    Placement(
                        AstrologicalObjectId.Mars,
                        210.0,
                        ZodiacSign.Scorpio,
                        7)
                },
                cusps);

        var mars =
            result.Single();

        Assert.AreEqual(
            ZodiacSign.Taurus,
            mars.HouseCuspSign);

        CollectionAssert.AreEqual(
            new[]
            {
                AstrologicalObjectId.Venus
            },
            mars.HouseRulers.ToArray());
    }

    [TestMethod]
    public void House_twelve_to_house_one_wrap_does_not_change_cusp_rule()
    {
        var cusps =
            StandardCusps()
                .Select(
                    cusp =>
                        cusp.HouseNumber switch
                        {
                            12 =>
                                new NatalHouseCuspSnapshot(
                                    12,
                                    350.0),

                            1 =>
                                new NatalHouseCuspSnapshot(
                                    1,
                                    10.0),

                            _ =>
                                cusp
                        })
                .ToArray();

        var result =
            NatalFactsReader.Read(
                new[]
                {
                    Placement(
                        AstrologicalObjectId.Sun,
                        355.0,
                        ZodiacSign.Pisces,
                        12),

                    Placement(
                        AstrologicalObjectId.Moon,
                        12.0,
                        ZodiacSign.Aries,
                        1)
                },
                cusps);

        var sun =
            result.Single(
                x => x.ObjectId
                    == AstrologicalObjectId.Sun);

        var moon =
            result.Single(
                x => x.ObjectId
                    == AstrologicalObjectId.Moon);

        Assert.AreEqual(
            ZodiacSign.Pisces,
            sun.HouseCuspSign);

        CollectionAssert.AreEqual(
            new[]
            {
                AstrologicalObjectId.Jupiter,
                AstrologicalObjectId.Neptune
            },
            sun.HouseRulers.ToArray());

        Assert.AreEqual(
            ZodiacSign.Aries,
            moon.HouseCuspSign);

        CollectionAssert.AreEqual(
            new[]
            {
                AstrologicalObjectId.Mars
            },
            moon.HouseRulers.ToArray());
    }

    [TestMethod]
    public void Persisted_motion_is_exposed_without_inference()
    {
        var result =
            NatalFactsReader.Read(
                new[]
                {
                    Placement(
                        AstrologicalObjectId.Mercury,
                        100.0,
                        ZodiacSign.Cancer,
                        4,
                        MotionState.Stationary)
                },
                StandardCusps());

        Assert.AreEqual(
            MotionState.Stationary,
            result.Single().Motion);
    }

    [TestMethod]
    public void Reader_does_not_mutate_snapshot_collections()
    {
        var placements =
            new[]
            {
                Placement(
                    AstrologicalObjectId.Mars,
                    15.0,
                    ZodiacSign.Aries,
                    1),

                Placement(
                    AstrologicalObjectId.Sun,
                    45.0,
                    ZodiacSign.Taurus,
                    2)
            };

        var originalOrder =
            placements
                .Select(x => x.ObjectId)
                .ToArray();

        _ =
            NatalFactsReader.Read(
                placements,
                StandardCusps());

        CollectionAssert.AreEqual(
            originalOrder,
            placements
                .Select(x => x.ObjectId)
                .ToArray());
    }

    [TestMethod]
    public void Ascendant_and_midheaven_remain_regular_factual_entries()
    {
        var result =
            NatalFactsReader.Read(
                new[]
                {
                    Placement(
                        AstrologicalObjectId.Midheaven,
                        280.0,
                        ZodiacSign.Capricorn,
                        10),

                    Placement(
                        AstrologicalObjectId.Ascendant,
                        100.0,
                        ZodiacSign.Cancer,
                        1)
                },
                StandardCusps());

        Assert.IsTrue(
            result.Any(
                x => x.ObjectId
                    == AstrologicalObjectId.Ascendant));

        Assert.IsTrue(
            result.Any(
                x => x.ObjectId
                    == AstrologicalObjectId.Midheaven));
    }

    private static NatalPlacementSnapshot Placement(
        AstrologicalObjectId objectId,
        double longitude,
        ZodiacSign sign,
        int? house,
        MotionState? motion = MotionState.Direct)
        => new(
            objectId,
            longitude,
            null,
            null,
            null,
            null,
            null,
            motion,
            (int)sign,
            longitude % 30.0,
            house);

    private static IReadOnlyList<
        NatalHouseCuspSnapshot>
        StandardCusps()
        => Enumerable
            .Range(1, 12)
            .Select(
                house =>
                    new NatalHouseCuspSnapshot(
                        house,
                        (house - 1) * 30.0))
            .ToArray();
}
