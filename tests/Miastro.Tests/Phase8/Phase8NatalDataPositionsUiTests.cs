using Miastro.Application.Natal.Reading;
using Miastro.Domain.Objects;
using Miastro.Domain.Placements;

namespace Miastro.Tests.Phase8;

[TestClass]
public sealed class Phase8NatalDataPositionsUiTests
{
    [TestMethod]
    public void Glyph_catalog_covers_all_supported_natal_objects()
    {
        foreach (
            var objectId
            in Enum.GetValues<
                AstrologicalObjectId>())
        {
            var glyph =
                NatalFactsPresentationCatalog
                    .ObjectGlyphText(
                        objectId);

            Assert.IsFalse(
                string.IsNullOrWhiteSpace(
                    glyph),
                objectId.ToString());
        }
    }

    [TestMethod]
    public void Asc_and_mc_are_textually_distinct_from_planet_glyphs()
    {
        Assert.AreEqual(
            "ASC",
            NatalFactsPresentationCatalog
                .ObjectGlyphText(
                    AstrologicalObjectId.Ascendant));

        Assert.AreEqual(
            "MC",
            NatalFactsPresentationCatalog
                .ObjectGlyphText(
                    AstrologicalObjectId.Midheaven));

        Assert.AreNotEqual(
            NatalFactsPresentationCatalog
                .ObjectGlyphText(
                    AstrologicalObjectId.Sun),
            NatalFactsPresentationCatalog
                .ObjectGlyphText(
                    AstrologicalObjectId.Ascendant));
    }

    [TestMethod]
    public void Data_accessibility_contains_name_degree_sign_and_ruler()
    {
        var row =
            new NatalDataRowReadModel(
                AstrologicalObjectId.Sun,
                "Sol",
                "12° 34′",
                "Escorpio",
                "Marte / Plutón",
                false);

        StringAssert.Contains(
            row.AccessibleName,
            "Sol");

        StringAssert.Contains(
            row.AccessibleName,
            "12° 34′");

        StringAssert.Contains(
            row.AccessibleName,
            "Escorpio");

        StringAssert.Contains(
            row.AccessibleName,
            "Marte / Plutón");
    }

    [TestMethod]
    public void Position_accessibility_uses_persisted_motion_text()
    {
        var row =
            Position(
                MotionState.Retrograde,
                "Retrógrado");

        StringAssert.Contains(
            row.AccessibleName,
            "Retrógrado");

        Assert.AreEqual(
            MotionState.Retrograde,
            row.Motion);
    }

    [TestMethod]
    public void Xaml_data_has_required_five_columns()
    {
        var block =
            Tab(
                "Datos",
                "Posiciones");

        foreach (var text in new[]
        {
            "Glifo",
            "Nombre",
            "Grado",
            "Signo",
            "Regente"
        })
        {
            StringAssert.Contains(
                block,
                $"Text=\"{text}\"");
        }

        foreach (var binding in new[]
        {
            "GlyphText",
            "ObjectName",
            "DegreeText",
            "SignName",
            "SignRulersText"
        })
        {
            StringAssert.Contains(
                block,
                $"{{Binding {binding}}}");
        }
    }

    [TestMethod]
    public void Xaml_positions_has_compact_required_fields()
    {
        var block =
            Tab(
                "Posiciones",
                "Aspectos");

        foreach (var binding in new[]
        {
            "GlyphText",
            "ObjectName",
            "ExactPositionText",
            "HouseText",
            "MotionText"
        })
        {
            StringAssert.Contains(
                block,
                $"{{Binding {binding}}}");
        }
    }

    [TestMethod]
    public void Xaml_positions_is_expandable_and_factual()
    {
        var block =
            Tab(
                "Posiciones",
                "Aspectos");

        StringAssert.Contains(
            block,
            "<Expander");

        StringAssert.Contains(
            block,
            "Text=\"Regente(s) del signo\"");

        StringAssert.Contains(
            block,
            "Text=\"Signo de la cúspide\"");

        StringAssert.Contains(
            block,
            "Text=\"Regente(s) de la casa\"");

        StringAssert.Contains(
            block,
            "{Binding HouseCuspSignText}");

        StringAssert.Contains(
            block,
            "{Binding HouseRulersText}");
    }

    [TestMethod]
    public void Xaml_marks_angles_explicitly()
    {
        var xaml =
            ReadXaml();

        Assert.IsTrue(
            Count(
                xaml,
                "Text=\"ÁNGULO\"")
            >= 2);

        Assert.IsTrue(
            Count(
                xaml,
                "IsVisible=\"{Binding IsAngle}\"")
            >= 2);
    }

    [TestMethod]
    public void Data_and_positions_keep_selection_bindings()
    {
        var xaml =
            ReadXaml();

        StringAssert.Contains(
            xaml,
            "NatalPanels.SelectedDataRow, Mode=TwoWay");

        StringAssert.Contains(
            xaml,
            "NatalPanels.SelectedPositionRow, Mode=TwoWay");
    }

    [TestMethod]
    public void Positions_ui_contains_no_interpretive_language()
    {
        var block =
            Tab(
                "Posiciones",
                "Aspectos");

        foreach (var forbidden in new[]
        {
            "personalidad",
            "destino",
            "misión",
            "carácter",
            "psicológ"
        })
        {
            Assert.IsFalse(
                block.Contains(
                    forbidden,
                    StringComparison.OrdinalIgnoreCase),
                forbidden);
        }
    }

    private static NatalPositionRowReadModel Position(
        MotionState motion,
        string motionText)
        => new(
            AstrologicalObjectId.Saturn,
            "Saturno",
            "17° 21′ Piscis",
            "Piscis",
            "Casa 4",
            motionText,
            "Júpiter / Neptuno",
            "Acuario",
            "Saturno / Urano",
            motion,
            false);

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
            start >= 0,
            header);

        Assert.IsTrue(
            end > start,
            nextHeader);

        return xaml[
            start..
            end];
    }

    private static int Count(
        string text,
        string value)
    {
        var count =
            0;

        var index =
            0;

        while (
            (
                index =
                    text.IndexOf(
                        value,
                        index,
                        StringComparison.Ordinal)
            ) >= 0)
        {
            count++;
            index +=
                value.Length;
        }

        return count;
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
