using System.Text.RegularExpressions;

namespace Miastro.Tests.Phase8;

[TestClass]
public sealed class Phase8NatalXamlNameScopeTests
{
    [TestMethod]
    public void MainWindow_has_no_duplicate_real_Name_attributes()
    {
        var xaml =
            ReadXaml();

        var names =
            Regex.Matches(
                xaml,
                "(?<![\\w.:])Name=\\\"([^\\\"]+)\\\"")
                .Select(
                    match =>
                        match.Groups[1].Value)
                .ToArray();

        var duplicates =
            names
                .GroupBy(
                    name =>
                        name,
                    StringComparer.Ordinal)
                .Where(
                    group =>
                        group.Count() > 1)
                .Select(
                    group =>
                        $"{group.Key} ({group.Count()})")
                .OrderBy(
                    value =>
                        value,
                    StringComparer.Ordinal)
                .ToArray();

        Assert.AreEqual(
            0,
            duplicates.Length,
            "Name duplicados: "
            + string.Join(
                ", ",
                duplicates));
    }

    [TestMethod]
    public void AutomationProperties_Name_is_not_treated_as_control_Name()
    {
        var sample =
            """
            <Border
                Name="RealControl"
                AutomationProperties.Name="Nombre accesible" />
            """;

        var names =
            Regex.Matches(
                sample,
                "(?<![\\w.:])Name=\\\"([^\\\"]+)\\\"")
                .Select(
                    match =>
                        match.Groups[1].Value)
                .ToArray();

        CollectionAssert.AreEqual(
            new[]
            {
                "RealControl"
            },
            names);
    }

    [TestMethod]
    public void Phase8_positions_list_keeps_canonical_name()
    {
        var xaml =
            ReadXaml();

        Assert.AreEqual(
            1,
            Regex.Matches(
                xaml,
                "(?<![\\w.:])Name=\\\"NatalPositionsList\\\"")
                .Count);

        Assert.AreEqual(
            1,
            Regex.Matches(
                xaml,
                "(?<![\\w.:])Name=\\\"NatalPositionsLegacyList\\\"")
                .Count);
    }

    [TestMethod]
    public void Phase8_positions_name_is_inside_positions_tab()
    {
        var xaml =
            ReadXaml();

        var tabStart =
            xaml.IndexOf(
                "Header=\"Posiciones\"",
                StringComparison.Ordinal);

        var tabEnd =
            xaml.IndexOf(
                "Header=\"Aspectos\"",
                tabStart,
                StringComparison.Ordinal);

        Assert.IsTrue(
            tabStart >= 0);

        Assert.IsTrue(
            tabEnd > tabStart);

        var block =
            xaml[
                tabStart..
                tabEnd];

        StringAssert.Contains(
            block,
            "Name=\"NatalPositionsList\"");

        Assert.IsFalse(
            block.Contains(
                "Name=\"NatalPositionsLegacyList\"",
                StringComparison.Ordinal));
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
