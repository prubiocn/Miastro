using System.Text.RegularExpressions;
namespace Miastro.Tests.Phase6;

[TestClass]
public sealed class Phase6NatalUiArchitectureTests
{
    [TestMethod]
    public void Person_card_exposes_minimum_natal_workflow()
    {
        var xaml =
            Read(
                "src/Miastro.UI.Avalonia/Views/"
                + "MainWindow.axaml");

        foreach (
            var required
            in new[]
            {
                "Carta natal",
                "Calcular carta natal",
                "Sistema de casas",
                "NatalHouseSystems",
                "NatalPlacements",
                "Objeto",
                "Posición",
                "Casa",
                "Movimiento"
            })
        {
            StringAssert.Contains(
                xaml,
                required);
        }
    }

    [TestMethod]
    public void Natal_ui_supports_only_placidus_and_koch_selection()
    {
        var vm =
            Read(
                "src/Miastro.UI.Avalonia/ViewModels/"
                + "MainWindowViewModel.Natal.cs");

        StringAssert.Contains(
            vm,
            "\"Placidus\"");

        StringAssert.Contains(
            vm,
            "HouseSystem.Placidus");

        StringAssert.Contains(
            vm,
            "\"Koch\"");

        StringAssert.Contains(
            vm,
            "HouseSystem.Koch");

        StringAssert.Contains(
            vm,
            "CalculateNatalChartUseCase");

        StringAssert.Contains(
            vm,
            "INatalChartStore");
    }

    [TestMethod]
    public void Natal_ui_does_not_expose_diagnostic_implementation_details()
    {
        var xaml =
            Read(
                "src/Miastro.UI.Avalonia/Views/"
                + "MainWindow.axaml");

        foreach (
            var forbidden
            in new[]
            {
                "InputHash",
                "BirthDataHash",
                "Julian",
                "ABI",
                "libswe",
                "EphemerisVersion",
                "EngineVersion",
                "AdapterVersion",
                "GeoNameId",
                "TzdbVersion"
            })
        {
            var pattern =
                $@"(?<![A-Za-z0-9_]){Regex.Escape(forbidden)}(?![A-Za-z0-9_])";

            Assert.IsFalse(
                Regex.IsMatch(
                    xaml,
                    pattern,
                    RegexOptions.IgnoreCase
                    | RegexOptions.CultureInvariant),
                $"Detalle técnico expuesto: {forbidden}");
        }
    }

    [TestMethod]
    public void Phase6_ui_has_no_direct_swiss_dependency()
    {
        var uiRoot =
            Path.Combine(
                Root,
                "src",
                "Miastro.UI.Avalonia");

        var files =
            Directory
                .GetFiles(
                    uiRoot,
                    "*",
                    SearchOption.AllDirectories)
                .Where(x =>
                    (
                        x.EndsWith(
                            ".cs",
                            StringComparison.OrdinalIgnoreCase)
                        || x.EndsWith(
                            ".axaml",
                            StringComparison.OrdinalIgnoreCase)
                    )
                    && !x.Contains(
                        "/bin/",
                        StringComparison.Ordinal)
                    && !x.Contains(
                        "/obj/",
                        StringComparison.Ordinal));

        foreach (
            var file
            in files)
        {
            var text =
                File.ReadAllText(
                    file);

            Assert.IsFalse(
                text.Contains(
                    "Infrastructure.SwissEphemeris",
                    StringComparison.Ordinal),
                $"UI llama directamente a Swiss: {file}");

                    }
    }

    private static string Read(
        string relativePath)
        => File.ReadAllText(
            Path.Combine(
                Root,
                relativePath));

    private static readonly string Root =
        Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "../../../../../"));
}
