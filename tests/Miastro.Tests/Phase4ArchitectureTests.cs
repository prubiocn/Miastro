namespace Miastro.Tests;

[TestClass]
public sealed class Phase4ArchitectureTests
{
    private static readonly string Root =
        Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "../../../../../"));

    [TestMethod]
    public void Domain_DoesNotReferenceNodaTime()
    {
        var files = Directory.GetFiles(
            Path.Combine(Root, "src/Miastro.Domain"),
            "*",
            SearchOption.AllDirectories)
            .Where(x =>
                x.EndsWith(".cs", StringComparison.OrdinalIgnoreCase) ||
                x.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
            .Where(x =>
                !x.Contains("/bin/", StringComparison.Ordinal) &&
                !x.Contains("/obj/", StringComparison.Ordinal));

        foreach (var file in files)
        {
            var text = File.ReadAllText(file);

            Assert.IsFalse(
                text.Contains(
                    "NodaTime",
                    StringComparison.Ordinal),
                $"Domain references NodaTime: {file}");
        }
    }

    [TestMethod]
    public void Ui_DoesNotUseNodaTimeOrSqliteDirectly()
    {
        var files = Directory.GetFiles(
            Path.Combine(Root, "src/Miastro.UI.Avalonia"),
            "*",
            SearchOption.AllDirectories)
            .Where(x =>
                x.EndsWith(".cs", StringComparison.OrdinalIgnoreCase) ||
                x.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
            .Where(x =>
                !x.Contains("/bin/", StringComparison.Ordinal) &&
                !x.Contains("/obj/", StringComparison.Ordinal));

        foreach (var file in files)
        {
            var text = File.ReadAllText(file);

            Assert.IsFalse(
                text.Contains(
                    "NodaTime",
                    StringComparison.Ordinal),
                $"UI references NodaTime: {file}");

            Assert.IsFalse(
                text.Contains(
                    "SqliteConnection",
                    StringComparison.Ordinal),
                $"UI references SqliteConnection: {file}");

            Assert.IsFalse(
                text.Contains(
                    "Microsoft.Data.Sqlite",
                    StringComparison.Ordinal),
                $"UI references Microsoft.Data.Sqlite: {file}");
        }
    }
}
