namespace Miastro.Tests.Phase8;

[TestClass]
public sealed class Phase8NatalMainWindowPanelIntegrationTests
{
    private static readonly string RepoRoot =
        FindRepoRoot();

    [TestMethod]
    public void Main_window_builds_panels_from_applied_snapshot()
    {
        var source =
            Read(
                "src/Miastro.UI.Avalonia/ViewModels/"
                + "MainWindowViewModel.Natal.cs");

        StringAssert.Contains(
            source,
            "BuildNatalPanels(");

        StringAssert.Contains(
            source,
            "snapshot);");
    }

    [TestMethod]
    public void Main_window_clears_panels_when_natal_display_resets()
    {
        var source =
            Read(
                "src/Miastro.UI.Avalonia/ViewModels/"
                + "MainWindowViewModel.Natal.cs");

        StringAssert.Contains(
            source,
            "ClearNatalPanels();");
    }

    [TestMethod]
    public void Panel_integration_exposes_single_host()
    {
        var source =
            Read(
                "src/Miastro.UI.Avalonia/ViewModels/"
                + "MainWindowViewModel.NatalPanels.cs");

        StringAssert.Contains(
            source,
            "NatalPanelHostViewModel?");

        StringAssert.Contains(
            source,
            "NatalPanels =>");

        StringAssert.Contains(
            source,
            "NatalPanelHostViewModel.From(");
    }

    [TestMethod]
    public void Panel_integration_notifies_host_and_visibility()
    {
        var source =
            Read(
                "src/Miastro.UI.Avalonia/ViewModels/"
                + "MainWindowViewModel.NatalPanels.cs");

        StringAssert.Contains(
            source,
            "nameof(NatalPanels)");

        StringAssert.Contains(
            source,
            "nameof(HasNatalPanels)");
    }

    [TestMethod]
    public void Panel_integration_contains_no_astronomy_or_domain_recalculation()
    {
        var source =
            Read(
                "src/Miastro.UI.Avalonia/ViewModels/"
                + "MainWindowViewModel.NatalPanels.cs");

        foreach (var forbidden in new[]
        {
            "SwissEphemeris",
            "CalculateNatalChartUseCase",
            "NatalAspectCalculator",
            "AspectEngine",
            "NatalHousePlacementResolver",
            "MotionStateResolver",
            "RulershipCatalog"
        })
        {
            Assert.IsFalse(
                source.Contains(
                    forbidden,
                    StringComparison.Ordinal));
        }
    }

    [TestMethod]
    public void Existing_apply_snapshot_remains_single_entry_point()
    {
        var source =
            Read(
                "src/Miastro.UI.Avalonia/ViewModels/"
                + "MainWindowViewModel.Natal.cs");

        Assert.AreEqual(
            1,
            Count(
                source,
                "private void ApplyNatalSnapshot("));
    }

    private static int Count(
        string text,
        string value)
    {
        var count =
            0;

        var start =
            0;

        while (true)
        {
            var index =
                text.IndexOf(
                    value,
                    start,
                    StringComparison.Ordinal);

            if (index < 0)
            {
                return count;
            }

            count++;

            start =
                index + value.Length;
        }
    }

    private static string Read(
        string relativePath)
        => File.ReadAllText(
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

        throw new InvalidOperationException(
            "No se encontró la raíz del repositorio.");
    }
}
