namespace Miastro.Tests;

[TestClass]
public sealed class UiInfrastructureSourceTests
{
    private static readonly string Root = FindRepositoryRoot();

    [TestMethod]
    public void Error_service_does_not_expose_stack_trace_to_user()
    {
        var file = Path.Combine(
            Root,
            "src",
            "Miastro.UI.Avalonia",
            "Services",
            "UserErrorService.cs");

        var text = File.ReadAllText(file);

        Assert.IsFalse(
            text.Contains("StackTrace", StringComparison.Ordinal));

        Assert.IsFalse(
            text.Contains("exception.ToString", StringComparison.Ordinal));
    }

    [TestMethod]
    public void Required_ADRs_exist()
    {
        string[] files =
        [
            "ADR-001-linux-ubuntu-dotnet10-avalonia.md",
            "ADR-002-modular-monolith-clean-architecture.md",
            "ADR-007-sqlite-ef-core.md",
            "ADR-014-xdg-directories.md",
            "ADR-015-self-contained-deb.md",
            "ADR-018-reproducible-versioning.md"
        ];

        foreach (var file in files)
        {
            Assert.IsTrue(
                File.Exists(
                    Path.Combine(
                        Root,
                        "docs",
                        "architecture",
                        "ADR",
                        file)),
                $"Falta ADR requerido: {file}");
        }
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
            "No se encontró la raíz del repositorio.");
    }
}
