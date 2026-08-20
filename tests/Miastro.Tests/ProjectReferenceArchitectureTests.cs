using System.Xml.Linq;

namespace Miastro.Tests;

[TestClass]
public sealed class ProjectReferenceArchitectureTests
{
    private static readonly string Root =
        FindRepositoryRoot();

    [TestMethod]
    public void UI_has_no_direct_persistence_or_swiss_reference()
    {
        var refs = GetProjectReferences(
            "src/Miastro.UI.Avalonia/Miastro.UI.Avalonia.csproj");

        Assert.IsFalse(
            refs.Any(x => x.Contains(
                "Infrastructure.Persistence",
                StringComparison.Ordinal)));

        Assert.IsFalse(
            refs.Any(x => x.Contains(
                "Infrastructure.SwissEphemeris",
                StringComparison.Ordinal)));
    }

    [TestMethod]
    public void Domain_has_no_project_dependencies()
    {
        var refs = GetProjectReferences(
            "src/Miastro.Domain/Miastro.Domain.csproj");

        Assert.AreEqual(0, refs.Count);
    }

    [TestMethod]
    public void Interpretation_has_no_swiss_dependency()
    {
        var refs = GetProjectReferences(
            "src/Miastro.Interpretation/Miastro.Interpretation.csproj");

        Assert.IsFalse(
            refs.Any(x => x.Contains(
                "SwissEphemeris",
                StringComparison.Ordinal)));
    }

    [TestMethod]
    public void Persistence_has_no_UI_dependency()
    {
        var refs = GetProjectReferences(
            "src/Miastro.Infrastructure.Persistence/Miastro.Infrastructure.Persistence.csproj");

        Assert.IsFalse(
            refs.Any(x => x.Contains(
                "UI.Avalonia",
                StringComparison.Ordinal)));
    }

    [TestMethod]
    public void Graphics_has_no_persistence_dependency()
    {
        var refs = GetProjectReferences(
            "src/Miastro.Graphics/Miastro.Graphics.csproj");

        Assert.IsFalse(
            refs.Any(x =>
                x.Contains(
                    "Infrastructure.Persistence",
                    StringComparison.Ordinal)));
    }

    private static List<string> GetProjectReferences(
        string relativePath)
    {
        var file = Path.Combine(Root, relativePath);

        var document = XDocument.Load(file);

        return document
            .Descendants("ProjectReference")
            .Select(x =>
                x.Attribute("Include")?.Value ?? string.Empty)
            .Where(x => x.Length > 0)
            .ToList();
    }

    private static string FindRepositoryRoot()
    {
        var current =
            new DirectoryInfo(AppContext.BaseDirectory);

        while (current is not null)
        {
            if (File.Exists(
                Path.Combine(current.FullName, "Miastro.sln")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new InvalidOperationException(
            "No se encontró la raíz del repositorio Miastro.");
    }
}
