using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Miastro.Tests.Phase7;

[TestClass]
public sealed class Phase7InteractionSelectionHardeningTests
{
    private static readonly string RepoRoot =
        FindRepoRoot();

    [TestMethod]
    public void Visual_rebuild_preserves_previous_selection_id()
    {
        var source =
            Read(
                "src/Miastro.UI.Avalonia/ViewModels/MainWindowViewModel.Natal.cs");

        var rebuild =
            Extract(
                source,
                "private void RebuildNatalWheel()",
                "private void ClearNatalWheel()");

        StringAssert.Contains(
            rebuild,
            "previousSelectionId");

        StringAssert.Contains(
            rebuild,
            "RestoreNatalWheelSelection");

        StringAssert.Contains(
            rebuild,
            "_natalWheelViewportWidth");

        StringAssert.Contains(
            rebuild,
            "_natalWheelRenderScaling");
    }

    [TestMethod]
    public void Hidden_selected_object_is_cleared_after_rebuild()
    {
        var source =
            Read(
                "src/Miastro.UI.Avalonia/ViewModels/MainWindowViewModel.Natal.cs");

        var restore =
            Extract(
                source,
                "private void RestoreNatalWheelSelection(",
                "private void ClearNatalWheel()");

        StringAssert.Contains(
            restore,
            "GetSelectableObjectIds");

        StringAssert.Contains(
            restore,
            "remainsVisible");

        StringAssert.Contains(
            restore,
            "ApplyNatalWheelSelection");
    }

    [TestMethod]
    public void New_snapshot_resets_selection_before_rebuild()
    {
        var source =
            Read(
                "src/Miastro.UI.Avalonia/ViewModels/MainWindowViewModel.Natal.cs");

        var apply =
            Extract(
                source,
                "private void ApplyNatalSnapshot(",
                "private void ResetNatalDisplay()");

        var resetPosition =
            apply.IndexOf(
                "_selectedNatalObjectId =",
                StringComparison.Ordinal);

        var rebuildPosition =
            apply.IndexOf(
                "RebuildNatalWheel();",
                StringComparison.Ordinal);

        Assert.IsTrue(
            resetPosition >= 0);

        Assert.IsTrue(
            rebuildPosition > resetPosition);
    }

    [TestMethod]
    public void Selection_api_rejects_objects_absent_from_visible_scene()
    {
        var source =
            Read(
                "src/Miastro.UI.Avalonia/ViewModels/MainWindowViewModel.Natal.cs");

        var apply =
            Extract(
                source,
                "private void ApplyNatalWheelSelection(",
                "private NatalWheelSceneConfiguration");

        StringAssert.Contains(
            apply,
            "GetSelectableObjectIds");

        StringAssert.Contains(
            apply,
            "selectableIds.Contains");

        StringAssert.Contains(
            apply,
            "objectId =\n                    null");
    }

    [TestMethod]
    public void Pointer_interaction_focuses_wheel_for_keyboard_continuation()
    {
        var source =
            Read(
                "src/Miastro.UI.Avalonia/Views/MainWindow.axaml.cs");

        var handler =
            Extract(
                source,
                "private void OnNatalWheelPointerPressed(",
                "private void OnNatalWheelViewportHostSizeChanged(");

        StringAssert.Contains(
            handler,
            "image.Focus();");

        var focusPosition =
            handler.IndexOf(
                "image.Focus();",
                StringComparison.Ordinal);

        var selectionPosition =
            handler.IndexOf(
                "SelectNatalWheelAt(",
                StringComparison.Ordinal);

        Assert.IsTrue(
            focusPosition >= 0);

        Assert.IsTrue(
            selectionPosition > focusPosition);
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
