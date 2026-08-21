namespace Miastro.Tests.Phase5;

[TestClass]
public sealed class Phase5PrivacyArchitectureTests
{
    [TestMethod]
    public void Private_person_fields_are_not_logged()
    {
        var root =
            FindRepositoryRoot();

        var sourceRoots = new[]
        {
            Path.Combine(root, "src"),
            Path.Combine(root, "tools")
        };

        var forbiddenPatterns = new[]
        {
            "LogInformation($\"{person.Phone",
            "LogInformation($\"{person.Email",
            "LogInformation($\"{person.PrivateNote",
            "LogDebug($\"{person.Phone",
            "LogDebug($\"{person.Email",
            "LogDebug($\"{person.PrivateNote",
            "Console.WriteLine(person.Phone",
            "Console.WriteLine(person.Email",
            "Console.WriteLine(person.PrivateNote"
        };

        foreach (var sourceRoot in sourceRoots)
        {
            if (!Directory.Exists(sourceRoot))
            {
                continue;
            }

            foreach (var file in Directory
                .EnumerateFiles(
                    sourceRoot,
                    "*.*",
                    SearchOption.AllDirectories)
                .Where(
                    x => x.EndsWith(
                        ".cs",
                        StringComparison.OrdinalIgnoreCase)
                        || x.EndsWith(
                            ".sh",
                            StringComparison.OrdinalIgnoreCase)))
            {
                var text =
                    File.ReadAllText(file);

                foreach (var forbidden in forbiddenPatterns)
                {
                    Assert.IsFalse(
                        text.Contains(
                            forbidden,
                            StringComparison.Ordinal),
                        $"Private field may be logged in {file}");
                }
            }
        }
    }

    [TestMethod]
    public void Person_application_layer_has_no_network_client_dependency()
    {
        var root =
            FindRepositoryRoot();

        var peopleDirectory =
            Path.Combine(
                root,
                "src",
                "Miastro.Application",
                "People");

        foreach (var file in Directory
            .EnumerateFiles(
                peopleDirectory,
                "*.cs",
                SearchOption.AllDirectories))
        {
            var text =
                File.ReadAllText(file);

            Assert.IsFalse(
                text.Contains(
                    "HttpClient",
                    StringComparison.Ordinal));

            Assert.IsFalse(
                text.Contains(
                    "System.Net.Http",
                    StringComparison.Ordinal));

            Assert.IsFalse(
                text.Contains(
                    "WebClient",
                    StringComparison.Ordinal));
        }
    }

    private static string FindRepositoryRoot()
    {
        var directory =
            new DirectoryInfo(
                AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(
                Path.Combine(
                    directory.FullName,
                    "Miastro.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Miastro repository root not found.");
    }
}
