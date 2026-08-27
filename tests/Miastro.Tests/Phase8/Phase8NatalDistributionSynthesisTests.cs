using Miastro.Application.Natal.Reading;
using Miastro.Domain.Objects;

namespace Miastro.Tests.Phase8;

[TestClass]
public sealed class Phase8NatalDistributionSynthesisTests
{
    [TestMethod]
    public void Synthesis_contains_single_element_predominance()
    {
        var model =
            NatalDistributionSynthesisBuilder.Build(
                Zodiac(
                    ElementSection(
                        fire: 1,
                        earth: 6,
                        air: 2,
                        water: 1,
                        NatalDistributionElement.Earth),
                    ModalitySection(
                        cardinal: 3,
                        fixedCount: 4,
                        mutable: 3,
                        NatalDistributionModality.Fixed),
                    PolaritySection(
                        positive: 5,
                        negative: 5,
                        null,
                        true)),
                Houses());

        Assert.IsTrue(
            model.Lines.Any(
                x =>
                    x == "Elemento predominante: Tierra (6/10)."));
    }

    [TestMethod]
    public void Balanced_section_is_reported_factually()
    {
        var model =
            NatalDistributionSynthesisBuilder.Build(
                Zodiac(
                    ElementSection(
                        fire: 3,
                        earth: 3,
                        air: 2,
                        water: 2,
                        null,
                        true),
                    ModalitySection(
                        cardinal: 4,
                        fixedCount: 3,
                        mutable: 3,
                        NatalDistributionModality.Cardinal),
                    PolaritySection(
                        positive: 5,
                        negative: 5,
                        null,
                        true)),
                Houses());

        Assert.IsTrue(
            model.Lines.Any(
                x =>
                    x == "Elementos: distribución equilibrada."));
    }

    [TestMethod]
    public void Tied_non_balanced_section_does_not_choose_arbitrary_winner()
    {
        var model =
            NatalDistributionSynthesisBuilder.Build(
                Zodiac(
                    ElementSection(
                        fire: 4,
                        earth: 4,
                        air: 1,
                        water: 1,
                        null,
                        false),
                    ModalitySection(
                        cardinal: 4,
                        fixedCount: 3,
                        mutable: 3,
                        NatalDistributionModality.Cardinal),
                    PolaritySection(
                        positive: 5,
                        negative: 5,
                        null,
                        true)),
                Houses());

        var line =
            model.Lines.Single(
                x =>
                    x.StartsWith(
                        "Elementos:",
                        StringComparison.Ordinal));

        StringAssert.Contains(
            line,
            "sin predominio único");

        StringAssert.Contains(
            line,
            "Fuego / Tierra");
    }

    [TestMethod]
    public void Synthesis_contains_house_geometry_facts()
    {
        var model =
            NatalDistributionSynthesisBuilder.Build(
                Zodiac(),
                Houses(
                    EastWestSection(
                        east: 7,
                        west: 3,
                        NatalEastWestHemisphere.East),
                    UpperLowerSection(
                        upper: 8,
                        lower: 2,
                        NatalUpperLowerHemisphere.Upper),
                    QuadrantSection(
                        q1: 1,
                        q2: 1,
                        q3: 2,
                        q4: 6,
                        NatalHouseQuadrant.Fourth),
                    HouseModeSection(
                        angular: 6,
                        succedent: 2,
                        cadent: 2,
                        NatalHouseMode.Angular)));

        CollectionAssert.Contains(
            model.Lines.ToArray(),
            "Hemisferio Este/Oeste predominante: Este (7/10).");

        CollectionAssert.Contains(
            model.Lines.ToArray(),
            "Hemisferio Superior/Inferior predominante: Superior (8/10).");

        CollectionAssert.Contains(
            model.Lines.ToArray(),
            "Cuadrante predominante: IV (6/10).");

        CollectionAssert.Contains(
            model.Lines.ToArray(),
            "Naturaleza de casas predominante: Angulares (6/10).");
    }

    [TestMethod]
    public void Synthesis_has_fixed_short_length()
    {
        var model =
            NatalDistributionSynthesisBuilder.Build(
                Zodiac(),
                Houses());

        Assert.AreEqual(
            7,
            model.Lines.Count);
    }

    [TestMethod]
    public void Synthesis_contains_no_personality_or_destiny_language()
    {
        var model =
            NatalDistributionSynthesisBuilder.Build(
                Zodiac(),
                Houses());

        var text =
            model.Text.ToLowerInvariant();

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
    public void Mismatched_profiles_are_rejected()
    {
        var rejected =
            false;

        try
        {
            _ =
                NatalDistributionSynthesisBuilder.Build(
                    Zodiac() with
                    {
                        ProfileId = "A"
                    },
                    Houses() with
                    {
                        ProfileId = "B"
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
    public void Synthesis_is_deterministic()
    {
        var zodiac =
            Zodiac();

        var houses =
            Houses();

        var first =
            NatalDistributionSynthesisBuilder.Build(
                zodiac,
                houses);

        var second =
            NatalDistributionSynthesisBuilder.Build(
                zodiac,
                houses);

        CollectionAssert.AreEqual(
            first.Lines.ToArray(),
            second.Lines.ToArray());
    }

    private static NatalDistributionReadModel Zodiac(
        NatalDistributionSection<NatalDistributionElement>? elements = null,
        NatalDistributionSection<NatalDistributionModality>? modalities = null,
        NatalDistributionSection<NatalDistributionPolarity>? polarities = null)
        => new(
            "MiastroV1",
            Array.Empty<AstrologicalObjectId>(),
            elements
                ?? ElementSection(
                    3,
                    3,
                    2,
                    2,
                    null,
                    true),
            modalities
                ?? ModalitySection(
                    4,
                    3,
                    3,
                    NatalDistributionModality.Cardinal),
            polarities
                ?? PolaritySection(
                    5,
                    5,
                    null,
                    true));

    private static NatalHouseDistributionReadModel Houses(
        NatalDistributionSection<NatalEastWestHemisphere>? eastWest = null,
        NatalDistributionSection<NatalUpperLowerHemisphere>? upperLower = null,
        NatalDistributionSection<NatalHouseQuadrant>? quadrants = null,
        NatalDistributionSection<NatalHouseMode>? houseModes = null)
        => new(
            "MiastroV1",
            eastWest
                ?? EastWestSection(
                    5,
                    5,
                    null,
                    true),
            upperLower
                ?? UpperLowerSection(
                    5,
                    5,
                    null,
                    true),
            quadrants
                ?? QuadrantSection(
                    3,
                    3,
                    2,
                    2,
                    null,
                    true),
            houseModes
                ?? HouseModeSection(
                    4,
                    3,
                    3,
                    NatalHouseMode.Angular));

    private static NatalDistributionSection<NatalDistributionElement>
        ElementSection(
            int fire,
            int earth,
            int air,
            int water,
            NatalDistributionElement? predominant,
            bool balanced = false)
        => new(
            new[]
            {
                Bucket(
                    NatalDistributionElement.Fire,
                    "Fuego",
                    fire),

                Bucket(
                    NatalDistributionElement.Earth,
                    "Tierra",
                    earth),

                Bucket(
                    NatalDistributionElement.Air,
                    "Aire",
                    air),

                Bucket(
                    NatalDistributionElement.Water,
                    "Agua",
                    water)
            },
            predominant,
            balanced);

    private static NatalDistributionSection<NatalDistributionModality>
        ModalitySection(
            int cardinal,
            int fixedCount,
            int mutable,
            NatalDistributionModality? predominant,
            bool balanced = false)
        => new(
            new[]
            {
                Bucket(
                    NatalDistributionModality.Cardinal,
                    "Cardinal",
                    cardinal),

                Bucket(
                    NatalDistributionModality.Fixed,
                    "Fijo",
                    fixedCount),

                Bucket(
                    NatalDistributionModality.Mutable,
                    "Mutable",
                    mutable)
            },
            predominant,
            balanced);

    private static NatalDistributionSection<NatalDistributionPolarity>
        PolaritySection(
            int positive,
            int negative,
            NatalDistributionPolarity? predominant,
            bool balanced = false)
        => new(
            new[]
            {
                Bucket(
                    NatalDistributionPolarity.Positive,
                    "Positiva",
                    positive),

                Bucket(
                    NatalDistributionPolarity.Negative,
                    "Negativa",
                    negative)
            },
            predominant,
            balanced);

    private static NatalDistributionSection<NatalEastWestHemisphere>
        EastWestSection(
            int east,
            int west,
            NatalEastWestHemisphere? predominant,
            bool balanced = false)
        => new(
            new[]
            {
                Bucket(
                    NatalEastWestHemisphere.East,
                    "Este",
                    east),

                Bucket(
                    NatalEastWestHemisphere.West,
                    "Oeste",
                    west)
            },
            predominant,
            balanced);

    private static NatalDistributionSection<NatalUpperLowerHemisphere>
        UpperLowerSection(
            int upper,
            int lower,
            NatalUpperLowerHemisphere? predominant,
            bool balanced = false)
        => new(
            new[]
            {
                Bucket(
                    NatalUpperLowerHemisphere.Upper,
                    "Superior",
                    upper),

                Bucket(
                    NatalUpperLowerHemisphere.Lower,
                    "Inferior",
                    lower)
            },
            predominant,
            balanced);

    private static NatalDistributionSection<NatalHouseQuadrant>
        QuadrantSection(
            int q1,
            int q2,
            int q3,
            int q4,
            NatalHouseQuadrant? predominant,
            bool balanced = false)
        => new(
            new[]
            {
                Bucket(
                    NatalHouseQuadrant.First,
                    "I",
                    q1),

                Bucket(
                    NatalHouseQuadrant.Second,
                    "II",
                    q2),

                Bucket(
                    NatalHouseQuadrant.Third,
                    "III",
                    q3),

                Bucket(
                    NatalHouseQuadrant.Fourth,
                    "IV",
                    q4)
            },
            predominant,
            balanced);

    private static NatalDistributionSection<NatalHouseMode>
        HouseModeSection(
            int angular,
            int succedent,
            int cadent,
            NatalHouseMode? predominant,
            bool balanced = false)
        => new(
            new[]
            {
                Bucket(
                    NatalHouseMode.Angular,
                    "Angulares",
                    angular),

                Bucket(
                    NatalHouseMode.Succedent,
                    "Sucedentes",
                    succedent),

                Bucket(
                    NatalHouseMode.Cadent,
                    "Cadentes",
                    cadent)
            },
            predominant,
            balanced);

    private static NatalDistributionBucket<T>
        Bucket<T>(
            T category,
            string label,
            int count)
        where T : struct, Enum
        => new(
            category,
            label,
            count,
            Array.Empty<AstrologicalObjectId>(),
            Array.Empty<string>());
}
