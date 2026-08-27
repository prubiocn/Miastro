using System.Xml.Linq;

namespace Miastro.Tests.Phase8;

[TestClass]
public sealed class Phase8NatalPanelShellXamlTests
{
    private static readonly string RepoRoot =
        FindRepoRoot();

    [TestMethod]
    public void Xaml_is_well_formed()
    {
        _ =
            XDocument.Load(
                Path.Combine(
                    RepoRoot,
                    "src",
                    "Miastro.UI.Avalonia",
                    "Views",
                    "MainWindow.axaml"));
    }

    [TestMethod]
    public void Natal_shell_contains_exactly_five_required_tabs()
    {
        var xaml =
            ReadXaml();

        foreach (var header in new[]
        {
            "Datos",
            "Posiciones",
            "Aspectos",
            "Distribución",
            "Resumen"
        })
        {
            StringAssert.Contains(
                xaml,
                $"Header=\"{header}\"");
        }

        Assert.AreEqual(
            5,
            Count(
                xaml,
                "<TabItem"));
    }

    [TestMethod]
    public void Positions_is_controlled_by_default_host_index()
    {
        var xaml =
            ReadXaml();

        var host =
            File.ReadAllText(
                Path.Combine(
                    RepoRoot,
                    "src",
                    "Miastro.UI.Avalonia",
                    "ViewModels",
                    "NatalPanels",
                    "NatalPanelHostViewModel.cs"));

        StringAssert.Contains(
            xaml,
            "NatalPanels.SelectedIndex");

        StringAssert.Contains(
            host,
            "NatalPanelKind.Positions");
    }

    [TestMethod]
    public void Shell_consumes_all_five_panel_viewmodels()
    {
        var xaml =
            ReadXaml();

        foreach (var binding in new[]
        {
            "NatalPanels.Data.Rows",
            "NatalPanels.Positions.Rows",
            "NatalPanels.Aspects.Cells",
            "NatalPanels.Distribution.SynthesisLines",
            "NatalPanels.Summary.SunText"
        })
        {
            StringAssert.Contains(
                xaml,
                binding);
        }
    }

    [TestMethod]
    public void Aspect_rows_have_accessible_fact_names()
    {
        var xaml =
            ReadXaml();

        StringAssert.Contains(
            xaml,
            "AutomationProperties.Name=\"{Binding AccessibleName}\"");

        StringAssert.Contains(
            xaml,
            "AspectSymbol");

        StringAssert.Contains(
            xaml,
            "OrbText");
    }

    [TestMethod]
    public void Distribution_uses_no_bar_chart_controls()
    {
        var xaml =
            ReadXaml();

        Assert.IsFalse(
            xaml.Contains(
                "ProgressBar",
                StringComparison.Ordinal));

        Assert.IsFalse(
            xaml.Contains(
                "BarChart",
                StringComparison.Ordinal));
    }

    [TestMethod]
    public void Phase7_wheel_controls_remain_present()
    {
        var xaml =
            ReadXaml();

        foreach (var required in new[]
        {
            "NatalWheelImage",
            "ShowNatalPlanets",
            "ShowNatalPoints",
            "ShowNatalAspects",
            "ShowNatalCusps",
            "ShowNatalLabels",
            "SelectedNatalWheelMode"
        })
        {
            StringAssert.Contains(
                xaml,
                required);
        }
    }

    [TestMethod]
    public void Right_panel_uses_two_thirds_one_third_ratio()
    {
        var xaml =
            ReadXaml();

        StringAssert.Contains(
            xaml,
            "ColumnDefinitions=\"*,Auto\"");
    }

    private static string ReadXaml()
        => File.ReadAllText(
            Path.Combine(
                RepoRoot,
                "src",
                "Miastro.UI.Avalonia",
                "Views",
                "MainWindow.axaml"));

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
