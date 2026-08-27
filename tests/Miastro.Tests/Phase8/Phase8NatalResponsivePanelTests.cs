namespace Miastro.Tests.Phase8;

[TestClass]
public sealed class Phase8NatalResponsivePanelTests
{
    [TestMethod]
    public void Natal_layout_gives_remaining_space_to_wheel()
    {
        var xaml =
            ReadXaml();

        StringAssert.Contains(
            xaml,
            "Name=\"NatalWheelPanelLayout\"");

        StringAssert.Contains(
            xaml,
            "ColumnDefinitions=\"*,Auto\"");
    }

    [TestMethod]
    public void Natal_panel_has_explicit_collapse_control()
    {
        var xaml =
            ReadXaml();

        StringAssert.Contains(
            xaml,
            "Name=\"NatalPhase8PanelToggleButton\"");

        StringAssert.Contains(
            xaml,
            "Click=\"OnNatalPhase8PanelToggleClick\"");

        StringAssert.Contains(
            xaml,
            "Content=\"Ocultar panel\"");
    }

    [TestMethod]
    public void Natal_panel_preserves_legible_width()
    {
        var xaml =
            ReadXaml();

        StringAssert.Contains(
            xaml,
            "Name=\"NatalPhase8PanelHost\"");

        StringAssert.Contains(
            xaml,
            "Width=\"360\"");

        StringAssert.Contains(
            xaml,
            "MinWidth=\"300\"");

        StringAssert.Contains(
            xaml,
            "MaxWidth=\"420\"");
    }

    [TestMethod]
    public void Layout_has_responsive_size_changed_handler()
    {
        var xaml =
            ReadXaml();

        StringAssert.Contains(
            xaml,
            "SizeChanged=\"OnNatalWheelPanelLayoutSizeChanged\"");
    }

    [TestMethod]
    public void Narrow_layout_collapses_panel_automatically()
    {
        var code =
            ReadCodeBehind();

        StringAssert.Contains(
            code,
            "NatalPanelResponsiveCollapseWidth");

        StringAssert.Contains(
            code,
            "width");

        StringAssert.Contains(
            code,
            "< NatalPanelResponsiveCollapseWidth");

        StringAssert.Contains(
            code,
            "SetNatalPhase8PanelExpanded");

        StringAssert.Contains(
            code,
            "false");
    }

    [TestMethod]
    public void Automatic_collapse_is_distinguished_from_manual_collapse()
    {
        var code =
            ReadCodeBehind();

        StringAssert.Contains(
            code,
            "_natalPanelAutoCollapsed");

        StringAssert.Contains(
            code,
            "if (_natalPanelAutoCollapsed)");
    }

    [TestMethod]
    public void Manual_toggle_clears_auto_collapse_state()
    {
        var code =
            ReadCodeBehind();

        var start =
            code.IndexOf(
                "OnNatalPhase8PanelToggleClick(",
                StringComparison.Ordinal);

        var end =
            code.IndexOf(
                "private void SetNatalPhase8PanelExpanded(",
                start,
                StringComparison.Ordinal);

        Assert.IsTrue(
            start >= 0);

        Assert.IsTrue(
            end > start);

        var handler =
            code[
                start..
                end];

        StringAssert.Contains(
            handler,
            "_natalPanelAutoCollapsed");

        StringAssert.Contains(
            handler,
            "false;");
    }

    [TestMethod]
    public void Matrix_keeps_horizontal_scrolling()
    {
        var block =
            Tab(
                "Aspectos",
                "Distribución");

        StringAssert.Contains(
            block,
            "HorizontalScrollBarVisibility=\"Auto\"");
    }

    [TestMethod]
    public void Positions_keep_horizontal_scrolling()
    {
        var block =
            Tab(
                "Posiciones",
                "Aspectos");

        StringAssert.Contains(
            block,
            "HorizontalScrollBarVisibility=\"Auto\"");
    }

    [TestMethod]
    public void Wheel_keeps_minimum_readable_width()
    {
        var xaml =
            ReadXaml();

        var host =
            xaml.IndexOf(
                "Name=\"NatalWheelViewportHost\"",
                StringComparison.Ordinal);

        Assert.IsTrue(
            host >= 0);

        var window =
            xaml.Substring(
                host,
                Math.Min(
                    800,
                    xaml.Length - host));

        StringAssert.Contains(
            window,
            "MinWidth=\"280\"");
    }

    [TestMethod]
    public void Responsive_behavior_contains_no_domain_calculation()
    {
        var code =
            ReadCodeBehind();

        foreach (
            var forbidden
            in new[]
            {
                "SwissEphemeris",
                "NatalAspectCalculator",
                "AspectEngine",
                "HousePlacementResolver"
            })
        {
            Assert.IsFalse(
                code.Contains(
                    forbidden,
                    StringComparison.Ordinal));
        }
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
        =>
            File.ReadAllText(
                Path.Combine(
                    FindRepoRoot(),
                    "src",
                    "Miastro.UI.Avalonia",
                    "Views",
                    "MainWindow.axaml"));

    private static string ReadCodeBehind()
        =>
            File.ReadAllText(
                Path.Combine(
                    FindRepoRoot(),
                    "src",
                    "Miastro.UI.Avalonia",
                    "Views",
                    "MainWindow.axaml.cs"));

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
