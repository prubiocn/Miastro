using Miastro.Application.Natal;
using Miastro.Application.Natal.Reading;
using Miastro.Domain.Aspects;
using Miastro.Domain.Objects;
using Miastro.Domain.Placements;
using Miastro.Domain.Zodiac;

namespace Miastro.Tests.Phase8;

[TestClass]
public sealed class Phase8NatalSummaryTests
{
    [TestMethod]
    public void Summary_contains_sun_moon_asc_and_mc()
    {
        var summary =
            BuildSummary(
                BaseFacts());

        Assert.AreEqual(
            "Sol: Aries, Casa 1.",
            summary.SunText);

        Assert.AreEqual(
            "Luna: Tauro, Casa 2.",
            summary.MoonText);

        Assert.AreEqual(
            "ASC: Libra.",
            summary.AscendantText);

        Assert.AreEqual(
            "MC: Cáncer.",
            summary.MidheavenText);
    }

    [TestMethod]
    public void Summary_contains_distribution_predominance()
    {
        var facts =
            BaseFacts()
                .Select(
                    fact =>
                        fact.ObjectId switch
                        {
                            AstrologicalObjectId.Sun
                                or AstrologicalObjectId.Mercury
                                or AstrologicalObjectId.Venus
                                => fact with
                                {
                                    Sign =
                                        ZodiacSign.Taurus
                                },

                            _ =>
                                fact
                        })
                .ToArray();

        var distribution =
            NatalDistributionService
                .BuildFromFacts(
                    facts);

        var summary =
            NatalSummaryBuilder.Build(
                facts,
                Array.Empty<NatalAspectSnapshot>(),
                distribution);

        Assert.AreEqual(
            "Elemento: Tierra (6).",
            summary.ElementText);
    }

    [TestMethod]
    public void Main_aspects_are_ordered_by_smallest_deviation()
    {
        var aspects =
            new[]
            {
                Aspect(
                    AstrologicalObjectId.Sun,
                    AstrologicalObjectId.Mars,
                    AspectKind.Square,
                    deviation: 3.0),

                Aspect(
                    AstrologicalObjectId.Moon,
                    AstrologicalObjectId.Saturn,
                    AspectKind.Trine,
                    deviation: 0.5),

                Aspect(
                    AstrologicalObjectId.Venus,
                    AstrologicalObjectId.Jupiter,
                    AspectKind.Sextile,
                    deviation: 1.0)
            };

        var summary =
            BuildSummary(
                BaseFacts(),
                aspects);

        Assert.AreEqual(
            3,
            summary.MainAspects.Count);

        Assert.AreEqual(
            AstrologicalObjectId.Moon,
            summary.MainAspects[0]
                .FirstObjectId);

        Assert.AreEqual(
            AstrologicalObjectId.Venus,
            summary.MainAspects[1]
                .FirstObjectId);

        Assert.AreEqual(
            AstrologicalObjectId.Sun,
            summary.MainAspects[2]
                .FirstObjectId);
    }

    [TestMethod]
    public void Main_aspects_are_limited_to_five()
    {
        var aspects =
            new[]
            {
                Aspect(
                    AstrologicalObjectId.Sun,
                    AstrologicalObjectId.Moon,
                    AspectKind.Conjunction,
                    0.1),

                Aspect(
                    AstrologicalObjectId.Sun,
                    AstrologicalObjectId.Mercury,
                    AspectKind.Sextile,
                    0.2),

                Aspect(
                    AstrologicalObjectId.Sun,
                    AstrologicalObjectId.Venus,
                    AspectKind.Square,
                    0.3),

                Aspect(
                    AstrologicalObjectId.Sun,
                    AstrologicalObjectId.Mars,
                    AspectKind.Trine,
                    0.4),

                Aspect(
                    AstrologicalObjectId.Sun,
                    AstrologicalObjectId.Jupiter,
                    AspectKind.Opposition,
                    0.5),

                Aspect(
                    AstrologicalObjectId.Sun,
                    AstrologicalObjectId.Saturn,
                    AspectKind.Quincunx,
                    0.6)
            };

        var summary =
            BuildSummary(
                BaseFacts(),
                aspects);

        Assert.AreEqual(
            5,
            summary.MainAspects.Count);
    }

    [TestMethod]
    public void Tie_in_deviation_uses_stable_aspect_priority()
    {
        var aspects =
            new[]
            {
                Aspect(
                    AstrologicalObjectId.Sun,
                    AstrologicalObjectId.Mars,
                    AspectKind.Sextile,
                    1.0),

                Aspect(
                    AstrologicalObjectId.Moon,
                    AstrologicalObjectId.Saturn,
                    AspectKind.Square,
                    1.0)
            };

        var summary =
            BuildSummary(
                BaseFacts(),
                aspects);

        Assert.AreEqual(
            AspectKind.Square,
            summary.MainAspects[0].Kind);

        Assert.AreEqual(
            AspectKind.Sextile,
            summary.MainAspects[1].Kind);
    }

    [TestMethod]
    public void Main_aspect_text_contains_readable_orb()
    {
        var summary =
            BuildSummary(
                BaseFacts(),
                new[]
                {
                    Aspect(
                        AstrologicalObjectId.Sun,
                        AstrologicalObjectId.Saturn,
                        AspectKind.Square,
                        2.233333)
                });

        StringAssert.Contains(
            summary.MainAspects
                .Single()
                .Text,
            "orbe 2°14′");
    }

    [TestMethod]
    public void Unique_house_concentration_is_reported()
    {
        var facts =
            BaseFacts()
                .Select(
                    fact =>
                        fact.ObjectId switch
                        {
                            AstrologicalObjectId.Sun
                                or AstrologicalObjectId.Moon
                                or AstrologicalObjectId.Mercury
                                or AstrologicalObjectId.Venus
                                => fact with
                                {
                                    HouseNumber = 10
                                },

                            _ =>
                                fact
                        })
                .ToArray();

        var summary =
            BuildSummary(
                facts);

        Assert.AreEqual(
            "Concentración de casas: Casa 10 (5/10).",
            summary.HouseConcentrationText);
    }

    [TestMethod]
    public void House_concentration_tie_does_not_choose_arbitrary_house()
    {
        var facts =
            BaseFacts()
                .Select(
                    fact =>
                        fact.ObjectId switch
                        {
                            AstrologicalObjectId.Sun
                                or AstrologicalObjectId.Moon
                                => fact with
                                {
                                    HouseNumber = 1
                                },

                            AstrologicalObjectId.Mercury
                                or AstrologicalObjectId.Venus
                                => fact with
                                {
                                    HouseNumber = 2
                                },

                            _ =>
                                fact
                        })
                .ToArray();

        var summary =
            BuildSummary(
                facts);

        Assert.AreEqual(
            "Concentración de casas: sin casa única predominante.",
            summary.HouseConcentrationText);
    }

    [TestMethod]
    public void Retrogrades_use_persisted_motion_and_canonical_order()
    {
        var facts =
            BaseFacts()
                .Select(
                    fact =>
                        fact.ObjectId switch
                        {
                            AstrologicalObjectId.Mercury
                                or AstrologicalObjectId.Saturn
                                => fact with
                                {
                                    Motion =
                                        MotionState.Retrograde
                                },

                            _ =>
                                fact
                        })
                .ToArray();

        var summary =
            BuildSummary(
                facts);

        Assert.AreEqual(
            "Retrógrados: Mercurio, Saturno.",
            summary.RetrogradesText);
    }

    [TestMethod]
    public void Additional_points_do_not_appear_as_summary_retrogrades()
    {
        var facts =
            BaseFacts()
                .Concat(
                    new[]
                    {
                        Fact(
                            AstrologicalObjectId.NorthTrueNode,
                            ZodiacSign.Gemini,
                            3,
                            MotionState.Retrograde)
                    })
                .ToArray();

        var summary =
            BuildSummary(
                facts);

        Assert.AreEqual(
            "Retrógrados: ninguno.",
            summary.RetrogradesText);
    }

    [TestMethod]
    public void Summary_remains_short()
    {
        var summary =
            BuildSummary(
                BaseFacts(),
                new[]
                {
                    Aspect(
                        AstrologicalObjectId.Sun,
                        AstrologicalObjectId.Moon,
                        AspectKind.Conjunction,
                        0.1),

                    Aspect(
                        AstrologicalObjectId.Sun,
                        AstrologicalObjectId.Mercury,
                        AspectKind.Sextile,
                        0.2),

                    Aspect(
                        AstrologicalObjectId.Sun,
                        AstrologicalObjectId.Venus,
                        AspectKind.Square,
                        0.3),

                    Aspect(
                        AstrologicalObjectId.Sun,
                        AstrologicalObjectId.Mars,
                        AspectKind.Trine,
                        0.4),

                    Aspect(
                        AstrologicalObjectId.Sun,
                        AstrologicalObjectId.Jupiter,
                        AspectKind.Opposition,
                        0.5)
                });

        Assert.IsTrue(
            summary.Lines.Count <= 13);
    }

    [TestMethod]
    public void Summary_contains_no_personality_or_destiny_interpretation()
    {
        var summary =
            BuildSummary(
                BaseFacts());

        var text =
            string.Join(
                " ",
                summary.Lines)
                .ToLowerInvariant();

        foreach (var forbidden in new[]
        {
            "eres una persona",
            "tu personalidad",
            "tu destino",
            "tu misión",
            "debes ",
            "esto indica que"
        })
        {
            Assert.IsFalse(
                text.Contains(
                    forbidden,
                    StringComparison.Ordinal));
        }
    }

    [TestMethod]
    public void Missing_required_angle_is_rejected()
    {
        var facts =
            BaseFacts()
                .Where(
                    fact =>
                        fact.ObjectId
                        != AstrologicalObjectId.Ascendant)
                .ToArray();

        var rejected =
            false;

        try
        {
            _ =
                BuildSummary(
                    facts);
        }
        catch (InvalidOperationException)
        {
            rejected =
                true;
        }

        Assert.IsTrue(
            rejected);
    }

    private static NatalSummaryReadModel BuildSummary(
        IReadOnlyList<NatalObjectFacts> facts,
        IReadOnlyList<NatalAspectSnapshot>? aspects = null)
    {
        var distribution =
            NatalDistributionService
                .BuildFromFacts(
                    facts);

        return NatalSummaryBuilder.Build(
            facts,
            aspects
                ?? Array.Empty<
                    NatalAspectSnapshot>(),
            distribution);
    }

    private static NatalAspectSnapshot Aspect(
        AstrologicalObjectId first,
        AstrologicalObjectId second,
        AspectKind kind,
        double deviation)
        => new(
            first,
            second,
            kind,
            SeparationDegrees:
                ExactAngle(kind)
                + deviation,
            ExactAngleDegrees:
                ExactAngle(kind),
            DeviationDegrees:
                deviation,
            AllowedOrbDegrees:
                8.0,
            UsedOrbDegrees:
                deviation);

    private static double ExactAngle(
        AspectKind kind)
        => kind switch
        {
            AspectKind.Conjunction => 0.0,
            AspectKind.Semisextile => 30.0,
            AspectKind.Sextile => 60.0,
            AspectKind.Square => 90.0,
            AspectKind.Trine => 120.0,
            AspectKind.Quincunx => 150.0,
            AspectKind.Opposition => 180.0,
            AspectKind.Quintile => 72.0,
            AspectKind.Biquintile => 144.0,
            _ => 0.0
        };

    private static IReadOnlyList<
        NatalObjectFacts>
        BaseFacts()
        => new[]
        {
            Fact(
                AstrologicalObjectId.Sun,
                ZodiacSign.Aries,
                1),

            Fact(
                AstrologicalObjectId.Moon,
                ZodiacSign.Taurus,
                2),

            Fact(
                AstrologicalObjectId.Mercury,
                ZodiacSign.Gemini,
                3),

            Fact(
                AstrologicalObjectId.Venus,
                ZodiacSign.Cancer,
                4),

            Fact(
                AstrologicalObjectId.Mars,
                ZodiacSign.Leo,
                5),

            Fact(
                AstrologicalObjectId.Jupiter,
                ZodiacSign.Virgo,
                6),

            Fact(
                AstrologicalObjectId.Saturn,
                ZodiacSign.Libra,
                7),

            Fact(
                AstrologicalObjectId.Uranus,
                ZodiacSign.Scorpio,
                8),

            Fact(
                AstrologicalObjectId.Neptune,
                ZodiacSign.Sagittarius,
                9),

            Fact(
                AstrologicalObjectId.Pluto,
                ZodiacSign.Capricorn,
                10),

            Fact(
                AstrologicalObjectId.Ascendant,
                ZodiacSign.Libra,
                1,
                null),

            Fact(
                AstrologicalObjectId.Midheaven,
                ZodiacSign.Cancer,
                10,
                null)
        };

    private static NatalObjectFacts Fact(
        AstrologicalObjectId objectId,
        ZodiacSign sign,
        int house,
        MotionState? motion =
            MotionState.Direct)
        => new(
            objectId,
            LongitudeDegrees:
                (int)sign * 30.0,
            Sign:
                sign,
            DegreeInSign:
                0.0,
            HouseNumber:
                house,
            Motion:
                motion,
            SignRulers:
                Array.Empty<
                    AstrologicalObjectId>(),
            HouseCuspSign:
                ZodiacSign.Aries,
            HouseRulers:
                Array.Empty<
                    AstrologicalObjectId>());
}
