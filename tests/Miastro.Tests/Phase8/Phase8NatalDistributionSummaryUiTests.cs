using Miastro.Application.Natal.Reading;
using Miastro.Domain.Aspects;
using Miastro.Domain.Objects;
using Miastro.UI.Avalonia.ViewModels.NatalPanels;

namespace Miastro.Tests.Phase8;

[TestClass]
public sealed class Phase8NatalDistributionSummaryUiTests
{
    [TestMethod]
    public void Distribution_viewmodel_exposes_seven_structured_sections()
    {
        var vm =
            Distribution();

        Assert.AreEqual(
            7,
            vm.Sections.Count);

        CollectionAssert.AreEqual(
            new[]
            {
                "Elementos",
                "Modalidades",
                "Polaridad",
                "Hemisferio Este / Oeste",
                "Hemisferio Superior / Inferior",
                "Cuadrantes",
                "Casas angulares / sucedentes / cadentes"
            },
            vm.Sections
                .Select(
                    section =>
                        section.Title)
                .ToArray());
    }

    [TestMethod]
    public void Distribution_rows_preserve_counts_and_names()
    {
        var vm =
            Distribution();

        var fire =
            vm.Elements.Rows
                .Single(
                    row =>
                        row.Label
                            == "Fuego");

        Assert.AreEqual(
            4,
            fire.Count);

        StringAssert.Contains(
            fire.ObjectsText,
            "Sol");

        StringAssert.Contains(
            fire.ObjectsText,
            "Marte");
    }

    [TestMethod]
    public void Distribution_marks_predominance_without_using_bars()
    {
        var vm =
            Distribution();

        var fire =
            vm.Elements.Rows
                .Single(
                    row =>
                        row.Label
                            == "Fuego");

        Assert.IsTrue(
            fire.IsPredominant);

        Assert.AreEqual(
            "Predominio: Fuego",
            vm.Elements.StatusText);

        var xaml =
            DistributionXaml();

        foreach (var forbidden in new[]
        {
            "ProgressBar",
            "ProgressRing",
            "BarChart",
            "ChartView"
        })
        {
            Assert.IsFalse(
                xaml.Contains(
                    forbidden,
                    StringComparison.Ordinal));
        }
    }

    [TestMethod]
    public void Distribution_xaml_contains_all_required_sections()
    {
        var xaml =
            DistributionXaml();

        StringAssert.Contains(
            xaml,
            "NatalPanels.Distribution.Sections");

        StringAssert.Contains(
            xaml,
            "{Binding Title}");

        StringAssert.Contains(
            xaml,
            "{Binding CountText}");

        StringAssert.Contains(
            xaml,
            "{Binding ObjectsText}");

        StringAssert.Contains(
            xaml,
            "NatalPanels.Distribution.SynthesisLines");
    }

    [TestMethod]
    public void Summary_viewmodel_exposes_required_factual_fields()
    {
        var vm =
            Summary();

        StringAssert.StartsWith(
            vm.SunText,
            "Sol:");

        StringAssert.StartsWith(
            vm.MoonText,
            "Luna:");

        StringAssert.StartsWith(
            vm.AscendantText,
            "ASC:");

        StringAssert.StartsWith(
            vm.MidheavenText,
            "MC:");

        StringAssert.StartsWith(
            vm.ElementText,
            "Elemento:");

        StringAssert.StartsWith(
            vm.ModalityText,
            "Modalidad:");

        StringAssert.StartsWith(
            vm.HouseConcentrationText,
            "Concentración de casas:");

        StringAssert.StartsWith(
            vm.RetrogradesText,
            "Retrógrados:");
    }

    [TestMethod]
    public void Summary_keeps_at_most_five_main_aspects()
    {
        var vm =
            Summary();

        Assert.IsTrue(
            vm.MainAspects.Count <= 5);
    }

    [TestMethod]
    public void Summary_xaml_displays_every_required_field()
    {
        var xaml =
            SummaryXaml();

        foreach (var property in new[]
        {
            "SunText",
            "MoonText",
            "AscendantText",
            "MidheavenText",
            "ElementText",
            "ModalityText",
            "HouseConcentrationText",
            "RetrogradesText",
            "MainAspects"
        })
        {
            StringAssert.Contains(
                xaml,
                $"NatalPanels.Summary.{property}");
        }
    }

    [TestMethod]
    public void Summary_xaml_is_short_structured_and_not_a_report()
    {
        var xaml =
            SummaryXaml();

        Assert.IsFalse(
            xaml.Contains(
                "Informe",
                StringComparison.OrdinalIgnoreCase));

        Assert.IsFalse(
            xaml.Contains(
                "personalidad",
                StringComparison.OrdinalIgnoreCase));

        Assert.IsFalse(
            xaml.Contains(
                "destino",
                StringComparison.OrdinalIgnoreCase));

        Assert.IsFalse(
            xaml.Contains(
                "misión",
                StringComparison.OrdinalIgnoreCase));
    }

    [TestMethod]
    public void Distribution_profile_is_visible()
    {
        var vm =
            Distribution();

        StringAssert.Contains(
            vm.ProfileText,
            "MiastroV1");

        StringAssert.Contains(
            vm.ProfileText,
            "10 objetos");
    }

    [TestMethod]
    public void Distribution_and_summary_use_structured_itemscontrols_not_selectable_lists()
    {
        var distribution =
            DistributionXaml();

        var summary =
            SummaryXaml();

        StringAssert.Contains(
            distribution,
            "<ItemsControl");

        StringAssert.Contains(
            summary,
            "<ItemsControl");

        Assert.IsFalse(
            distribution.Contains(
                "<ProgressBar",
                StringComparison.Ordinal));
    }

    private static NatalDistributionPanelViewModel
        Distribution()
    {
        var objects =
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
            };

        var zodiac =
            new NatalDistributionReadModel(
                "MiastroV1",
                objects,
                Section(
                    new[]
                    {
                        Bucket(
                            NatalDistributionElement.Fire,
                            "Fuego",
                            4,
                            "Sol",
                            "Marte",
                            "Júpiter",
                            "Plutón"),

                        Bucket(
                            NatalDistributionElement.Earth,
                            "Tierra",
                            2,
                            "Mercurio",
                            "Venus"),

                        Bucket(
                            NatalDistributionElement.Air,
                            "Aire",
                            2,
                            "Saturno",
                            "Urano"),

                        Bucket(
                            NatalDistributionElement.Water,
                            "Agua",
                            2,
                            "Luna",
                            "Neptuno")
                    },
                    NatalDistributionElement.Fire),
                Section(
                    new[]
                    {
                        Bucket(
                            NatalDistributionModality.Cardinal,
                            "Cardinal",
                            4,
                            "Sol"),

                        Bucket(
                            NatalDistributionModality.Fixed,
                            "Fija",
                            3,
                            "Luna"),

                        Bucket(
                            NatalDistributionModality.Mutable,
                            "Mutable",
                            3,
                            "Mercurio")
                    },
                    NatalDistributionModality.Cardinal),
                Section(
                    new[]
                    {
                        Bucket(
                            NatalDistributionPolarity.Positive,
                            "Positiva",
                            6,
                            "Sol"),

                        Bucket(
                            NatalDistributionPolarity.Negative,
                            "Negativa",
                            4,
                            "Luna")
                    },
                    NatalDistributionPolarity.Positive));

        var houses =
            new NatalHouseDistributionReadModel(
                "MiastroV1",
                Section(
                    new[]
                    {
                        Bucket(
                            NatalEastWestHemisphere.East,
                            "Este",
                            6,
                            "Sol"),

                        Bucket(
                            NatalEastWestHemisphere.West,
                            "Oeste",
                            4,
                            "Luna")
                    },
                    NatalEastWestHemisphere.East),
                Section(
                    new[]
                    {
                        Bucket(
                            NatalUpperLowerHemisphere.Upper,
                            "Superior",
                            5,
                            "Sol"),

                        Bucket(
                            NatalUpperLowerHemisphere.Lower,
                            "Inferior",
                            5,
                            "Luna")
                    },
                    predominant: null,
                    balanced: true),
                Section(
                    new[]
                    {
                        Bucket(
                            NatalHouseQuadrant.First,
                            "I",
                            4,
                            "Sol"),

                        Bucket(
                            NatalHouseQuadrant.Second,
                            "II",
                            2,
                            "Luna"),

                        Bucket(
                            NatalHouseQuadrant.Third,
                            "III",
                            2,
                            "Marte"),

                        Bucket(
                            NatalHouseQuadrant.Fourth,
                            "IV",
                            2,
                            "Saturno")
                    },
                    NatalHouseQuadrant.First),
                Section(
                    new[]
                    {
                        Bucket(
                            NatalHouseMode.Angular,
                            "Angulares",
                            4,
                            "Sol"),

                        Bucket(
                            NatalHouseMode.Succedent,
                            "Sucedentes",
                            3,
                            "Luna"),

                        Bucket(
                            NatalHouseMode.Cadent,
                            "Cadentes",
                            3,
                            "Marte")
                    },
                    NatalHouseMode.Angular));

        return new NatalDistributionPanelViewModel(
            zodiac,
            houses,
            new NatalDistributionSynthesisReadModel(
                "MiastroV1",
                new[]
                {
                    "Elemento predominante: Fuego (4/10).",
                    "Modalidad predominante: Cardinal (4/10)."
                }));
    }

    private static NatalSummaryPanelViewModel
        Summary()
        => new(
            new NatalSummaryReadModel(
                "Sol: Aries, Casa 1.",
                "Luna: Tauro, Casa 2.",
                "ASC: Géminis.",
                "MC: Acuario.",
                "Elemento: Fuego (4).",
                "Modalidad: Cardinal (4).",
                "Concentración de casas: Casa 1 (3/10).",
                "Retrógrados: Saturno.",
                new[]
                {
                    new NatalSummaryAspectReadModel(
                        AstrologicalObjectId.Sun,
                        AstrologicalObjectId.Saturn,
                        AspectKind.Square,
                        "Aspecto: Sol — cuadratura — Saturno — orbe 1°00′.",
                        1.0)
                }));

    private static NatalDistributionSection<T>
        Section<T>(
            IReadOnlyList<
                NatalDistributionBucket<T>> buckets,
            T? predominant,
            bool balanced = false)
        where T : struct, Enum
        => new(
            buckets,
            predominant,
            balanced);

    private static NatalDistributionBucket<T>
        Bucket<T>(
            T category,
            string label,
            int count,
            params string[] names)
        where T : struct, Enum
        => new(
            category,
            label,
            count,
            Array.Empty<
                AstrologicalObjectId>(),
            names);

    private static string DistributionXaml()
        => Tab(
            "Distribución",
            "Resumen");

    private static string SummaryXaml()
    {
        var xaml =
            ReadXaml();

        var start =
            xaml.IndexOf(
                "Header=\"Resumen\"",
                StringComparison.Ordinal);

        var end =
            xaml.IndexOf(
                "</TabControl>",
                start,
                StringComparison.Ordinal);

        Assert.IsTrue(
            start >= 0);

        Assert.IsTrue(
            end > start);

        return xaml[
            start..
            end];
    }

    private static string Tab(
        string header,
        string nextHeader)
    {
        var xaml =
            ReadXaml();

        var start =
            xaml.IndexOf(
                $"Header=\"{header}\"",
                StringComparison.Ordinal);

        var end =
            xaml.IndexOf(
                $"Header=\"{nextHeader}\"",
                start,
                StringComparison.Ordinal);

        Assert.IsTrue(
            start >= 0);

        Assert.IsTrue(
            end > start);

        return xaml[
            start..
            end];
    }

    private static string ReadXaml()
        => File.ReadAllText(
            Path.Combine(
                FindRepoRoot(),
                "src",
                "Miastro.UI.Avalonia",
                "Views",
                "MainWindow.axaml"));

    private static string FindRepoRoot()
    {
        var current =
            new DirectoryInfo(
                AppContext.BaseDirectory);

        while (current is not null)
        {
            if (File.Exists(
                Path.Combine(
                    current.FullName,
                    "Miastro.sln")))
            {
                return current.FullName;
            }

            current =
                current.Parent;
        }

        throw new InvalidOperationException(
            "No se encontró la raíz del repositorio.");
    }
}
