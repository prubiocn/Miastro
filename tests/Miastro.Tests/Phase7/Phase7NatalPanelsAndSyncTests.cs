using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miastro.Application.Natal;
using Miastro.Domain.Aspects;
using Miastro.Domain.Objects;
using Miastro.Domain.Placements;
using Miastro.UI.Avalonia.ViewModels;

namespace Miastro.Tests.Phase7;

[TestClass]
public sealed class Phase7NatalPanelsAndSyncTests
{
    private static readonly string RepoRoot =
        FindRepoRoot();

    [TestMethod]
    public void Ui_contains_explicit_data_positions_and_aspects_panels()
    {
        var xaml =
            Read(
                "src/Miastro.UI.Avalonia/Views/MainWindow.axaml");

        StringAssert.Contains(
            xaml,
            "Name=\"NatalDataPanel\"");

        StringAssert.Contains(
            xaml,
            "Name=\"NatalPositionsPanel\"");

        StringAssert.Contains(
            xaml,
            "Name=\"NatalAspectsPanel\"");
    }

    [TestMethod]
    public void Positions_panel_uses_two_way_selected_item_binding()
    {
        var xaml =
            Read(
                "src/Miastro.UI.Avalonia/Views/MainWindow.axaml");

        StringAssert.Contains(
            xaml,
            "Name=\"NatalPositionsList\"");

        StringAssert.Contains(
            xaml,
            "SelectedItem=\"{Binding SelectedNatalPlacement, Mode=TwoWay}\"");
    }

    [TestMethod]
    public void Placement_row_preserves_astrological_object_identity()
    {
        var placement =
            new NatalPlacementSnapshot(
                AstrologicalObjectId.Mercury,
                123.5,
                null,
                null,
                null,
                null,
                null,
                MotionState.Retrograde,
                4,
                3.5,
                10);

        var row =
            NatalPlacementRowViewModel
                .From(
                    placement);

        Assert.AreEqual(
            AstrologicalObjectId.Mercury,
            row.ObjectId);

        Assert.AreEqual(
            "Mercurio",
            row.ObjectName);

        Assert.AreEqual(
            "Retrógrado",
            row.MotionText);
    }

    [TestMethod]
    public void Aspect_row_uses_persisted_snapshot_without_recalculation()
    {
        var snapshot =
            new NatalAspectSnapshot(
                AstrologicalObjectId.Sun,
                AstrologicalObjectId.Moon,
                AspectKind.Trine,
                119.25,
                120.0,
                0.75,
                8.0,
                0.75);

        var row =
            NatalAspectRowViewModel
                .From(
                    snapshot);

        Assert.AreEqual(
            AstrologicalObjectId.Sun,
            row.FirstObjectId);

        Assert.AreEqual(
            AstrologicalObjectId.Moon,
            row.SecondObjectId);

        Assert.AreEqual(
            "Sol",
            row.FirstObjectName);

        Assert.AreEqual(
            "Trígono",
            row.AspectName);

        Assert.AreEqual(
            "Luna",
            row.SecondObjectName);

        Assert.AreEqual(
            "Orbe 0,75°",
            row.OrbText);
    }

    [TestMethod]
    public void Panel_selection_routes_through_same_wheel_selection_method()
    {
        var source =
            Read(
                "src/Miastro.UI.Avalonia/ViewModels/MainWindowViewModel.Natal.cs");

        var property =
            Extract(
                source,
                "public NatalPlacementRowViewModel?",
                "public bool HasSelectedNatalObject");

        StringAssert.Contains(
            property,
            "SelectedNatalPlacement");

        StringAssert.Contains(
            property,
            "ApplyNatalWheelSelection");

        StringAssert.Contains(
            property,
            "value?.ObjectId.ToString()");
    }

    [TestMethod]
    public void Wheel_selection_returns_existing_panel_row_instance()
    {
        var source =
            Read(
                "src/Miastro.UI.Avalonia/ViewModels/MainWindowViewModel.Natal.cs");

        var finder =
            Extract(
                source,
                "private NatalPlacementRowViewModel?",
                "private static string HumanNatalFailure");

        StringAssert.Contains(
            finder,
            "NatalPlacements");

        StringAssert.Contains(
            finder,
            "x.ObjectId == parsed");

        Assert.IsFalse(
            finder.Contains(
                "NatalPlacementRowViewModel\n                .From",
                StringComparison.Ordinal));
    }

    private static string Extract(
        string source,
        string startMarker,
        string endMarker)
    {
        var start =
            source.IndexOf(
                startMarker,
                StringComparison.Ordinal);

        var end =
            source.IndexOf(
                endMarker,
                start >= 0
                    ? start
                    : 0,
                StringComparison.Ordinal);

        Assert.IsTrue(
            start >= 0,
            startMarker);

        Assert.IsTrue(
            end > start,
            endMarker);

        return source[
            start..end];
    }

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
