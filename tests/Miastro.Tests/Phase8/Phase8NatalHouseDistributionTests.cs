using Miastro.Application.Natal.Reading;
using Miastro.Domain.Objects;
using Miastro.Domain.Placements;
using Miastro.Domain.Zodiac;

namespace Miastro.Tests.Phase8;

[TestClass]
public sealed class Phase8NatalHouseDistributionTests
{
    [TestMethod]
    public void East_hemisphere_uses_houses_ten_to_three()
    {
        var result =
            NatalHouseDistributionService
                .BuildFromFacts(
                    new[]
                    {
                        Fact(
                            AstrologicalObjectId.Sun,
                            10),

                        Fact(
                            AstrologicalObjectId.Moon,
                            11),

                        Fact(
                            AstrologicalObjectId.Mercury,
                            12),

                        Fact(
                            AstrologicalObjectId.Venus,
                            1),

                        Fact(
                            AstrologicalObjectId.Mars,
                            2),

                        Fact(
                            AstrologicalObjectId.Jupiter,
                            3),

                        Fact(
                            AstrologicalObjectId.Saturn,
                            4)
                    });

        Assert.AreEqual(
            6,
            Bucket(
                result.EastWest,
                NatalEastWestHemisphere.East)
                .Count);

        Assert.AreEqual(
            1,
            Bucket(
                result.EastWest,
                NatalEastWestHemisphere.West)
                .Count);
    }

    [TestMethod]
    public void Upper_and_lower_hemispheres_follow_house_numbers()
    {
        var result =
            NatalHouseDistributionService
                .BuildFromFacts(
                    new[]
                    {
                        Fact(
                            AstrologicalObjectId.Sun,
                            1),

                        Fact(
                            AstrologicalObjectId.Moon,
                            6),

                        Fact(
                            AstrologicalObjectId.Mercury,
                            7),

                        Fact(
                            AstrologicalObjectId.Venus,
                            12)
                    });

        Assert.AreEqual(
            2,
            Bucket(
                result.UpperLower,
                NatalUpperLowerHemisphere.Lower)
                .Count);

        Assert.AreEqual(
            2,
            Bucket(
                result.UpperLower,
                NatalUpperLowerHemisphere.Upper)
                .Count);

        Assert.IsTrue(
            result.UpperLower.IsBalanced);

        Assert.IsNull(
            result.UpperLower.Predominant);
    }

    [TestMethod]
    public void Quadrants_follow_three_house_groups()
    {
        var result =
            NatalHouseDistributionService
                .BuildFromFacts(
                    new[]
                    {
                        Fact(
                            AstrologicalObjectId.Sun,
                            1),

                        Fact(
                            AstrologicalObjectId.Moon,
                            3),

                        Fact(
                            AstrologicalObjectId.Mercury,
                            4),

                        Fact(
                            AstrologicalObjectId.Venus,
                            7),

                        Fact(
                            AstrologicalObjectId.Mars,
                            10)
                    });

        Assert.AreEqual(
            2,
            Bucket(
                result.Quadrants,
                NatalHouseQuadrant.First)
                .Count);

        Assert.AreEqual(
            1,
            Bucket(
                result.Quadrants,
                NatalHouseQuadrant.Second)
                .Count);

        Assert.AreEqual(
            1,
            Bucket(
                result.Quadrants,
                NatalHouseQuadrant.Third)
                .Count);

        Assert.AreEqual(
            1,
            Bucket(
                result.Quadrants,
                NatalHouseQuadrant.Fourth)
                .Count);

        Assert.AreEqual(
            NatalHouseQuadrant.First,
            result.Quadrants.Predominant);
    }

    [TestMethod]
    public void Angular_succedent_and_cadent_are_classified_correctly()
    {
        var result =
            NatalHouseDistributionService
                .BuildFromFacts(
                    new[]
                    {
                        Fact(
                            AstrologicalObjectId.Sun,
                            1),

                        Fact(
                            AstrologicalObjectId.Moon,
                            4),

                        Fact(
                            AstrologicalObjectId.Mercury,
                            2),

                        Fact(
                            AstrologicalObjectId.Venus,
                            5),

                        Fact(
                            AstrologicalObjectId.Mars,
                            3),

                        Fact(
                            AstrologicalObjectId.Jupiter,
                            6)
                    });

        Assert.AreEqual(
            2,
            Bucket(
                result.HouseModes,
                NatalHouseMode.Angular)
                .Count);

        Assert.AreEqual(
            2,
            Bucket(
                result.HouseModes,
                NatalHouseMode.Succedent)
                .Count);

        Assert.AreEqual(
            2,
            Bucket(
                result.HouseModes,
                NatalHouseMode.Cadent)
                .Count);

        Assert.IsTrue(
            result.HouseModes.IsBalanced);

        Assert.IsNull(
            result.HouseModes.Predominant);
    }

    [TestMethod]
    public void Extreme_house_concentration_is_deterministic()
    {
        var result =
            NatalHouseDistributionService
                .BuildFromFacts(
                    new[]
                    {
                        Fact(
                            AstrologicalObjectId.Sun,
                            10),

                        Fact(
                            AstrologicalObjectId.Moon,
                            10),

                        Fact(
                            AstrologicalObjectId.Mercury,
                            10),

                        Fact(
                            AstrologicalObjectId.Venus,
                            10),

                        Fact(
                            AstrologicalObjectId.Mars,
                            10)
                    });

        Assert.AreEqual(
            NatalEastWestHemisphere.East,
            result.EastWest.Predominant);

        Assert.AreEqual(
            NatalUpperLowerHemisphere.Upper,
            result.UpperLower.Predominant);

        Assert.AreEqual(
            NatalHouseQuadrant.Fourth,
            result.Quadrants.Predominant);

        Assert.AreEqual(
            NatalHouseMode.Angular,
            result.HouseModes.Predominant);
    }

    [TestMethod]
    public void Distribution_uses_persisted_house_not_longitude_proximity()
    {
        var fact =
            new NatalObjectFacts(
                AstrologicalObjectId.Sun,
                LongitudeDegrees:
                    29.999999,
                Sign:
                    ZodiacSign.Aries,
                DegreeInSign:
                    29.999999,
                HouseNumber:
                    12,
                Motion:
                    MotionState.Direct,
                SignRulers:
                    Array.Empty<
                        AstrologicalObjectId>(),
                HouseCuspSign:
                    ZodiacSign.Pisces,
                HouseRulers:
                    Array.Empty<
                        AstrologicalObjectId>());

        var result =
            NatalHouseDistributionService
                .BuildFromFacts(
                    new[]
                    {
                        fact
                    });

        Assert.AreEqual(
            1,
            Bucket(
                result.Quadrants,
                NatalHouseQuadrant.Fourth)
                .Count);

        Assert.AreEqual(
            1,
            Bucket(
                result.HouseModes,
                NatalHouseMode.Cadent)
                .Count);
    }

    [TestMethod]
    public void Additional_points_do_not_affect_house_distribution()
    {
        var result =
            NatalHouseDistributionService
                .BuildFromFacts(
                    new[]
                    {
                        Fact(
                            AstrologicalObjectId.Sun,
                            1),

                        Fact(
                            AstrologicalObjectId.Moon,
                            7),

                        Fact(
                            AstrologicalObjectId.NorthTrueNode,
                            1),

                        Fact(
                            AstrologicalObjectId.Ascendant,
                            1)
                    });

        var total =
            result.Quadrants.Buckets
                .Sum(x => x.Count);

        Assert.AreEqual(
            2,
            total);
    }

    [TestMethod]
    public void Missing_house_for_counted_planet_is_rejected()
    {
        var rejected =
            false;

        try
        {
            _ =
                NatalHouseDistributionService
                    .BuildFromFacts(
                        new[]
                        {
                            new NatalObjectFacts(
                                AstrologicalObjectId.Sun,
                                0.0,
                                ZodiacSign.Aries,
                                0.0,
                                null,
                                MotionState.Direct,
                                Array.Empty<
                                    AstrologicalObjectId>(),
                                null,
                                Array.Empty<
                                    AstrologicalObjectId>())
                        });
        }
        catch (InvalidOperationException)
        {
            rejected =
                true;
        }

        Assert.IsTrue(
            rejected);
    }

    [TestMethod]
    public void Object_order_inside_house_buckets_is_canonical()
    {
        var result =
            NatalHouseDistributionService
                .BuildFromFacts(
                    new[]
                    {
                        Fact(
                            AstrologicalObjectId.Mars,
                            1),

                        Fact(
                            AstrologicalObjectId.Sun,
                            1),

                        Fact(
                            AstrologicalObjectId.Moon,
                            1)
                    });

        CollectionAssert.AreEqual(
            new[]
            {
                AstrologicalObjectId.Sun,
                AstrologicalObjectId.Moon,
                AstrologicalObjectId.Mars
            },
            Bucket(
                result.Quadrants,
                NatalHouseQuadrant.First)
                .Objects
                .ToArray());
    }

    private static NatalDistributionBucket<T>
        Bucket<T>(
            NatalDistributionSection<T> section,
            T category)
        where T : struct, Enum
        => section.Buckets.Single(
            x =>
                EqualityComparer<T>
                    .Default
                    .Equals(
                        x.Category,
                        category));

    private static NatalObjectFacts Fact(
        AstrologicalObjectId objectId,
        int houseNumber)
        => new(
            objectId,
            LongitudeDegrees:
                0.0,
            Sign:
                ZodiacSign.Aries,
            DegreeInSign:
                0.0,
            HouseNumber:
                houseNumber,
            Motion:
                MotionState.Direct,
            SignRulers:
                Array.Empty<
                    AstrologicalObjectId>(),
            HouseCuspSign:
                ZodiacSign.Aries,
            HouseRulers:
                Array.Empty<
                    AstrologicalObjectId>());
}
