using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Miastro.Tests.Phase7;

[TestClass]
public sealed class Phase7ResponsiveAccessibilityUiTests
{
    private static readonly string RepoRoot =
        FindRepoRoot();

    [TestMethod]
    public void Natal_wheel_has_no_fixed_560_size_and_uses_uniform_stretch()
    {
        var xaml =
            Read(
                "src/Miastro.UI.Avalonia/Views/MainWindow.axaml");

        Assert.IsFalse(
            xaml.Contains(
                "Width=\"560\"",
                StringComparison.Ordinal));

        Assert.IsFalse(
            xaml.Contains(
                "Height=\"560\"",
                StringComparison.Ordinal));

        StringAssert.Contains(
            xaml,
            "Stretch=\"Uniform\"");
    }

    [TestMethod]
    public void Wheel_host_drives_responsive_viewport()
    {
        var xaml =
            Read(
                "src/Miastro.UI.Avalonia/Views/MainWindow.axaml");

        var codeBehind =
            Read(
                "src/Miastro.UI.Avalonia/Views/MainWindow.axaml.cs");

        StringAssert.Contains(
            xaml,
            "NatalWheelViewportHost");

        StringAssert.Contains(
            xaml,
            "OnNatalWheelViewportHostSizeChanged");

        StringAssert.Contains(
            codeBehind,
            "UpdateNatalWheelViewport");
    }

    [TestMethod]
    public void Avalonia_uses_top_level_render_scaling()
    {
        var codeBehind =
            Read(
                "src/Miastro.UI.Avalonia/Views/MainWindow.axaml.cs");

        var service =
            Read(
                "src/Miastro.UI.Avalonia/Services/NatalWheelPresentationService.cs");

        StringAssert.Contains(
            codeBehind,
            "RenderScaling");

        StringAssert.Contains(
            service,
            "renderScaling");

        StringAssert.Contains(
            service,
            "width\n                    * renderScaling");
    }

    [TestMethod]
    public void Natal_wheel_is_keyboard_focusable_and_navigable()
    {
        var xaml =
            Read(
                "src/Miastro.UI.Avalonia/Views/MainWindow.axaml");

        var codeBehind =
            Read(
                "src/Miastro.UI.Avalonia/Views/MainWindow.axaml.cs");

        var viewModel =
            Read(
                "src/Miastro.UI.Avalonia/ViewModels/MainWindowViewModel.Natal.cs");

        StringAssert.Contains(
            xaml,
            "Focusable=\"True\"");

        StringAssert.Contains(
            xaml,
            "OnNatalWheelKeyDown");

        StringAssert.Contains(
            codeBehind,
            "Key.Right");

        StringAssert.Contains(
            codeBehind,
            "Key.Left");

        StringAssert.Contains(
            codeBehind,
            "Key.Home");

        StringAssert.Contains(
            codeBehind,
            "Key.End");

        StringAssert.Contains(
            viewModel,
            "MoveNatalWheelSelection");
    }

    [TestMethod]
    public void Natal_wheel_exposes_accessible_equivalent_text()
    {
        var xaml =
            Read(
                "src/Miastro.UI.Avalonia/Views/MainWindow.axaml");

        var viewModel =
            Read(
                "src/Miastro.UI.Avalonia/ViewModels/MainWindowViewModel.Natal.cs");

        StringAssert.Contains(
            xaml,
            "AutomationProperties.HelpText");

        StringAssert.Contains(
            xaml,
            "NatalWheelAccessibilityText");

        StringAssert.Contains(
            viewModel,
            "NatalWheelAccessibilityText");

        StringAssert.Contains(
            viewModel,
            "PositionText");

        StringAssert.Contains(
            viewModel,
            "HouseText");

        StringAssert.Contains(
            viewModel,
            "MotionText");
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
