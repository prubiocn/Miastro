using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Miastro.Tests.Phase7;

[TestClass]
public sealed class Phase7UiWheelConfigurationSourceTests
{
    private static readonly string RepoRoot =
        FindRepoRoot();

    [TestMethod]
    public void Avalonia_exposes_all_required_visibility_controls()
    {
        var xaml =
            Read(
                "src/Miastro.UI.Avalonia/Views/MainWindow.axaml");

        StringAssert.Contains(
            xaml,
            "ShowNatalPlanets");

        StringAssert.Contains(
            xaml,
            "ShowNatalPoints");

        StringAssert.Contains(
            xaml,
            "ShowNatalAspects");

        StringAssert.Contains(
            xaml,
            "ShowNatalCusps");

        StringAssert.Contains(
            xaml,
            "ShowNatalLabels");
    }

    [TestMethod]
    public void Avalonia_exposes_consultation_and_presentation_modes()
    {
        var source =
            Read(
                "src/Miastro.UI.Avalonia/ViewModels/MainWindowViewModel.Natal.cs");

        StringAssert.Contains(
            source,
            "NatalWheelViewMode.Consultation");

        StringAssert.Contains(
            source,
            "NatalWheelViewMode.Presentation");

        StringAssert.Contains(
            source,
            "NatalWheelSceneConfiguration");
    }

    [TestMethod]
    public void Selection_panel_consumes_graphics_hit_testing()
    {
        var source =
            Read(
                "src/Miastro.UI.Avalonia/ViewModels/MainWindowViewModel.Natal.cs");

        StringAssert.Contains(
            source,
            "NatalSceneHitTester");

        StringAssert.Contains(
            source,
            "SelectedNatalPlacement");

        StringAssert.Contains(
            source,
            "FindNatalPlacementRow");
    }

    [TestMethod]
    public void Ui_contains_no_central_geometry_algorithm()
    {
        var root =
            Path.Combine(
                RepoRoot,
                "src",
                "Miastro.UI.Avalonia");

        foreach (
            var file
            in Directory.EnumerateFiles(
                root,
                "*.cs",
                SearchOption.AllDirectories))
        {
            var source =
                File.ReadAllText(file);

            Assert.IsFalse(
                source.Contains(
                    "NatalWheelCoordinates",
                    StringComparison.Ordinal),
                file);

            Assert.IsFalse(
                source.Contains(
                    "NatalObjectPlacementEngine",
                    StringComparison.Ordinal),
                file);

            Assert.IsFalse(
                source.Contains(
                    "NatalWheelLayoutBuilder",
                    StringComparison.Ordinal),
                file);
        }
    }

    [TestMethod]
    public void Presentation_service_passes_semantic_configuration()
    {
        var source =
            Read(
                "src/Miastro.UI.Avalonia/Services/NatalWheelPresentationService.cs");

        StringAssert.Contains(
            source,
            "NatalWheelSceneConfiguration?");

        StringAssert.Contains(
            source,
            "NatalWheelSceneComposer");

        StringAssert.Contains(
            source,
            "configuration");
    }

    [TestMethod]
    public void Natal_wheel_is_not_nested_inside_location_or_other_listbox()
    {
        var path =
            Path.Combine(
                RepoRoot,
                "src",
                "Miastro.UI.Avalonia",
                "Views",
                "MainWindow.axaml");

        var document =
            XDocument.Load(path);

        var wheel =
            document
                .Descendants()
                .Single(
                    x =>
                        AttributeValue(
                            x,
                            "Name")
                        == "NatalWheelImage");

        Assert.IsFalse(
            wheel
                .Ancestors()
                .Any(
                    x =>
                        x.Name.LocalName
                            == "ListBox"),
            "NatalWheelImage no puede estar dentro de un ListBox.");

        var wheelBorder =
            wheel
                .Ancestors()
                .FirstOrDefault(
                    x =>
                        x.Name.LocalName
                            == "Border"
                        && AttributeValue(
                                x,
                                "IsVisible")
                            .Contains(
                                "HasNatalWheel",
                                StringComparison.Ordinal));

        Assert.IsNotNull(
            wheelBorder);

        var positionsPanel =
            document
                .Descendants()
                .Single(
                    x =>
                        AttributeValue(
                            x,
                            "Name")
                        == "NatalPositionsPanel");

        var natalSection =
            wheelBorder
                .Ancestors()
                .FirstOrDefault(
                    x =>
                        x.Name.LocalName
                            == "StackPanel"
                        && x.Descendants()
                            .Any(
                                child =>
                                    ReferenceEquals(
                                        child,
                                        positionsPanel)));

        Assert.IsNotNull(
            natalSection,
            "Rueda y panel Posiciones deben pertenecer a la misma sección natal.");
    }

    private static string AttributeValue(
        XElement element,
        string localName)
        =>
            element
                .Attributes()
                .FirstOrDefault(
                    x =>
                        x.Name.LocalName
                            == localName)
                ?.Value
            ?? string.Empty;

    private static string Read(
        string relativePath)
        =>
            File.ReadAllText(
                Path.Combine(
                    RepoRoot,
                    relativePath));

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

        throw new DirectoryNotFoundException(
            "Miastro repository root not found.");
    }
}
