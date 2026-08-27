using Miastro.Application.Natal;
using Miastro.Application.Natal.Reading;
using Miastro.Domain.Objects;
using Miastro.Domain.Placements;
using Miastro.Domain.Zodiac;

namespace Miastro.Tests.Phase8;

[TestClass]
public sealed class Phase8NatalDistributionTests
{
    [TestMethod]
    public void Miastro_v1_profile_counts_exactly_ten_planets()
    {
        CollectionAssert.AreEqual(
            new[]
            {
                AstrologicalObjectId.Sun,
                AstrologicalObjectId.Moon,
                AstrologicalObjectId.Mercury,
                AstrologicalObjectId.Venus,
                AstrologicalObjectId.Mars,
                AstrologicalObjectId.Jupiter,
                AstrologicalObjectId.Saturn,
                AstrologicalObjectId.Uranus,
                AstrologicalObjectId.Neptune,
                AstrologicalObjectId.Pluto
            },
            NatalDistributionProfile
                .MiastroV1
                .CountedObjects
                .ToArray());
    }

    [TestMethod]
    public void Additional_points_do_not_affect_v1_distribution()
    {
        var facts =
            new[]
            {
                Fact(
                    AstrologicalObjectId.Sun,
                    ZodiacSign.Aries),

                Fact(
                    AstrologicalObjectId.Moon,
                    ZodiacSign.Taurus),

                Fact(
                    AstrologicalObjectId.NorthTrueNode,
                    ZodiacSign.Aries),

                Fact(
                    AstrologicalObjectId.MeanLilith,
                    ZodiacSign.Aries),

                Fact(
                    AstrologicalObjectId.PartOfFortune,
                    ZodiacSign.Aries),

                Fact(
                    AstrologicalObjectId.Ascendant,
                    ZodiacSign.Aries)
            };

        var result =
            NatalDistributionService
                .BuildFromFacts(
                    facts);

        CollectionAssert.AreEqual(
            new[]
            {
                AstrologicalObjectId.Sun,
                AstrologicalObjectId.Moon
            },
            result.CountedObjects.ToArray());

        Assert.AreEqual(
            1,
            Bucket(
                result.Elements,
                NatalDistributionElement.Fire)
                .Count);

        Assert.AreEqual(
            1,
            Bucket(
                result.Elements,
                NatalDistributionElement.Earth)
                .Count);
    }

    [TestMethod]
    public void Elements_are_calculated_from_signs()
    {
        var result =
            NatalDistributionService
                .BuildFromFacts(
                    new[]
                    {
                        Fact(
                            AstrologicalObjectId.Sun,
                            ZodiacSign.Aries),

                        Fact(
                            AstrologicalObjectId.Moon,
                            ZodiacSign.Leo),

                        Fact(
                            AstrologicalObjectId.Mercury,
                            ZodiacSign.Taurus),

                        Fact(
                            AstrologicalObjectId.Venus,
                            ZodiacSign.Gemini),

                        Fact(
                            AstrologicalObjectId.Mars,
                            ZodiacSign.Cancer)
                    });

        Assert.AreEqual(
            2,
            Bucket(
                result.Elements,
                NatalDistributionElement.Fire)
                .Count);

        Assert.AreEqual(
            1,
            Bucket(
                result.Elements,
                NatalDistributionElement.Earth)
                .Count);

        Assert.AreEqual(
            1,
            Bucket(
                result.Elements,
                NatalDistributionElement.Air)
                .Count);

        Assert.AreEqual(
            1,
            Bucket(
                result.Elements,
                NatalDistributionElement.Water)
                .Count);

        Assert.AreEqual(
            NatalDistributionElement.Fire,
            result.Elements.Predominant);
    }

    [TestMethod]
    public void Modalities_are_calculated_from_signs()
    {
        var result =
            NatalDistributionService
                .BuildFromFacts(
                    new[]
                    {
                        Fact(
                            AstrologicalObjectId.Sun,
                            ZodiacSign.Aries),

                        Fact(
                            AstrologicalObjectId.Moon,
                            ZodiacSign.Cancer),

                        Fact(
                            AstrologicalObjectId.Mercury,
                            ZodiacSign.Taurus),

                        Fact(
                            AstrologicalObjectId.Venus,
                            ZodiacSign.Leo),

                        Fact(
                            AstrologicalObjectId.Mars,
                            ZodiacSign.Gemini)
                    });

        Assert.AreEqual(
            2,
            Bucket(
                result.Modalities,
                NatalDistributionModality.Cardinal)
                .Count);

        Assert.AreEqual(
            2,
            Bucket(
                result.Modalities,
                NatalDistributionModality.Fixed)
                .Count);

        Assert.AreEqual(
            1,
            Bucket(
                result.Modalities,
                NatalDistributionModality.Mutable)
                .Count);

        Assert.IsNull(
            result.Modalities.Predominant);
    }

    [TestMethod]
    public void Polarities_are_calculated_from_signs()
    {
        var result =
            NatalDistributionService
                .BuildFromFacts(
                    new[]
                    {
                        Fact(
                            AstrologicalObjectId.Sun,
                            ZodiacSign.Aries),

                        Fact(
                            AstrologicalObjectId.Moon,
                            ZodiacSign.Gemini),

                        Fact(
                            AstrologicalObjectId.Mercury,
                            ZodiacSign.Taurus),

                        Fact(
                            AstrologicalObjectId.Venus,
                            ZodiacSign.Cancer)
                    });

        Assert.AreEqual(
            2,
            Bucket(
                result.Polarities,
                NatalDistributionPolarity.Positive)
                .Count);

        Assert.AreEqual(
            2,
            Bucket(
                result.Polarities,
                NatalDistributionPolarity.Negative)
                .Count);

        Assert.IsTrue(
            result.Polarities.IsBalanced);

        Assert.IsNull(
            result.Polarities.Predominant);
    }

    [TestMethod]
    public void Extreme_concentration_has_single_predominant_category()
    {
        var result =
            NatalDistributionService
                .BuildFromFacts(
                    new[]
                    {
                        Fact(
                            AstrologicalObjectId.Sun,
                            ZodiacSign.Taurus),

                        Fact(
                            AstrologicalObjectId.Moon,
                            ZodiacSign.Virgo),

                        Fact(
                            AstrologicalObjectId.Mercury,
                            ZodiacSign.Capricorn),

                        Fact(
                            AstrologicalObjectId.Venus,
                            ZodiacSign.Taurus),

                        Fact(
                            AstrologicalObjectId.Mars,
                            ZodiacSign.Virgo)
                    });

        Assert.AreEqual(
            NatalDistributionElement.Earth,
            result.Elements.Predominant);

        Assert.AreEqual(
            5,
            Bucket(
                result.Elements,
                NatalDistributionElement.Earth)
                .Count);

        Assert.IsFalse(
            result.Elements.IsBalanced);
    }

    [TestMethod]
    public void Equal_top_counts_do_not_choose_arbitrary_predominant()
    {
        var result =
            NatalDistributionService
                .BuildFromFacts(
                    new[]
                    {
                        Fact(
                            AstrologicalObjectId.Sun,
                            ZodiacSign.Aries),

                        Fact(
                            AstrologicalObjectId.Moon,
                            ZodiacSign.Leo),

                        Fact(
                            AstrologicalObjectId.Mercury,
                            ZodiacSign.Taurus),

                        Fact(
                            AstrologicalObjectId.Venus,
                            ZodiacSign.Virgo)
                    });

        Assert.IsNull(
            result.Elements.Predominant);
    }

    [TestMethod]
    public void Object_names_inside_buckets_follow_canonical_order()
    {
        var result =
            NatalDistributionService
                .BuildFromFacts(
                    new[]
                    {
                        Fact(
                            AstrologicalObjectId.Mars,
                            ZodiacSign.Aries),

                        Fact(
                            AstrologicalObjectId.Sun,
                            ZodiacSign.Leo),

                        Fact(
                            AstrologicalObjectId.Moon,
                            ZodiacSign.Sagittarius)
                    });

        var fire =
            Bucket(
                result.Elements,
                NatalDistributionElement.Fire);

        CollectionAssert.AreEqual(
            new[]
            {
                AstrologicalObjectId.Sun,
                AstrologicalObjectId.Moon,
                AstrologicalObjectId.Mars
            },
            fire.Objects.ToArray());

        CollectionAssert.AreEqual(
            new[]
            {
                "Sol",
                "Luna",
                "Marte"
            },
            fire.ObjectNames.ToArray());
    }

    [TestMethod]
    public void Distribution_does_not_mutate_input_order()
    {
        var facts =
            new[]
            {
                Fact(
                    AstrologicalObjectId.Mars,
                    ZodiacSign.Aries),

                Fact(
                    AstrologicalObjectId.Sun,
                    ZodiacSign.Taurus)
            };

        var original =
            facts
                .Select(x => x.ObjectId)
                .ToArray();

        _ =
            NatalDistributionService
                .BuildFromFacts(
                    facts);

        CollectionAssert.AreEqual(
            original,
            facts
                .Select(x => x.ObjectId)
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
        ZodiacSign sign)
        => new(
            objectId,
            LongitudeDegrees:
                (int)sign * 30.0,
            Sign:
                sign,
            DegreeInSign:
                0.0,
            HouseNumber:
                1,
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
