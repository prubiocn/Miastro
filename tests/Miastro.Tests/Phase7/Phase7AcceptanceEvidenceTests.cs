using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Miastro.Tests.Phase7;

[TestClass]
public sealed class Phase7AcceptanceEvidenceTests
{
    private static readonly string RepoRoot =
        FindRepoRoot();

    [TestMethod]
    public void Natal_wheel_lives_in_single_main_application_window()
    {
        var uiRoot =
            Path.Combine(
                RepoRoot,
                "src",
                "Miastro.UI.Avalonia");

        var windowFiles =
            Directory
                .EnumerateFiles(
                    uiRoot,
                    "*.cs",
                    SearchOption.AllDirectories)
                .Where(
                    file =>
                    {
                        var source =
                            File.ReadAllText(
                                file);

                        return source.Contains(
                                   "class MainWindow",
                                   StringComparison.Ordinal)
                               && source.Contains(
                                   ": Window",
                                   StringComparison.Ordinal);
                    })
                .ToArray();

        Assert.AreEqual(
            1,
            windowFiles.Length,
            string.Join(
                Environment.NewLine,
                windowFiles));

        Assert.IsTrue(
            windowFiles[0]
                .EndsWith(
                    "MainWindow.axaml.cs",
                    StringComparison.Ordinal));

        var xaml =
            Read(
                "src/Miastro.UI.Avalonia/Views/MainWindow.axaml");

        StringAssert.Contains(
            xaml,
            "Name=\"NatalWheelImage\"");

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
    public void Graphics_and_natal_ui_do_not_log_private_chart_data()
    {
        var roots =
            new[]
            {
                Path.Combine(
                    RepoRoot,
                    "src",
                    "Miastro.Graphics"),

                Path.Combine(
                    RepoRoot,
                    "src",
                    "Miastro.Graphics.Skia"),

                Path.Combine(
                    RepoRoot,
                    "src",
                    "Miastro.UI.Avalonia")
            };

        var loggingLines =
            roots
                .SelectMany(
                    root =>
                        Directory
                            .EnumerateFiles(
                                root,
                                "*.cs",
                                SearchOption.AllDirectories))
                .SelectMany(
                    file =>
                        File.ReadAllLines(file)
                            .Where(
                                line =>
                                    line.Contains(
                                        "LogInformation(",
                                        StringComparison.Ordinal)
                                    || line.Contains(
                                        "LogDebug(",
                                        StringComparison.Ordinal)
                                    || line.Contains(
                                        "LogWarning(",
                                        StringComparison.Ordinal)
                                    || line.Contains(
                                        "LogError(",
                                        StringComparison.Ordinal)
                                    || line.Contains(
                                        "LogCritical(",
                                        StringComparison.Ordinal))
                            .Select(
                                line =>
                                    (
                                        File: file,
                                        Line: line
                                    )))
                .ToArray();

        foreach (var item in loggingLines)
        {
            var combined =
                item.File
                + " "
                + item.Line;

            foreach (
                var forbidden
                in new[]
                {
                    "PersonId",
                    "Locality",
                    "BirthLocalDate",
                    "BirthLocalTime",
                    "Latitude",
                    "Longitude",
                    "Email",
                    "Phone",
                    "Note",
                    "NatalPlacement",
                    "NatalAspect",
                    "InputHash"
                })
            {
                Assert.IsFalse(
                    combined.Contains(
                        forbidden,
                        StringComparison.OrdinalIgnoreCase),
                    $"{forbidden}: {combined}");
            }
        }

        var graphicsLogs =
            loggingLines
                .Where(
                    x =>
                        x.File.Contains(
                            "Miastro.Graphics",
                            StringComparison.Ordinal))
                .ToArray();

        Assert.AreEqual(
            0,
            graphicsLogs.Length);
    }

    [TestMethod]
    public void Phase7_graphics_architecture_boundaries_are_explicit()
    {
        var graphicsProject =
            Read(
                "src/Miastro.Graphics/Miastro.Graphics.csproj");

        var skiaProject =
            Read(
                "src/Miastro.Graphics.Skia/Miastro.Graphics.Skia.csproj");

        Assert.IsFalse(
            graphicsProject.Contains(
                "SkiaSharp",
                StringComparison.Ordinal));

        Assert.IsFalse(
            graphicsProject.Contains(
                "Miastro.UI.Avalonia",
                StringComparison.Ordinal));

        Assert.IsFalse(
            graphicsProject.Contains(
                "SwissEphemeris",
                StringComparison.Ordinal));

        Assert.IsFalse(
            graphicsProject.Contains(
                "Astronomy",
                StringComparison.Ordinal));

        Assert.IsFalse(
            skiaProject.Contains(
                "SwissEphemeris",
                StringComparison.Ordinal));

        Assert.IsFalse(
            skiaProject.Contains(
                "Miastro.Astronomy",
                StringComparison.Ordinal));

        var uiRoot =
            Path.Combine(
                RepoRoot,
                "src",
                "Miastro.UI.Avalonia");

        foreach (
            var file
            in Directory.EnumerateFiles(
                uiRoot,
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
