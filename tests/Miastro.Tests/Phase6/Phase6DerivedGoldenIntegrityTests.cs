using System.Security.Cryptography;
using System.Text.Json;

namespace Miastro.Tests.Phase6;

[TestClass]
public sealed class Phase6DerivedGoldenIntegrityTests
{
    [TestMethod]
    public void Derived_golden_is_bound_to_external_primary_corpus()
    {
        var root =
            RepositoryRoot();

        var primaryPath =
            Path.Combine(
                root,
                "tests",
                "golden",
                "phase6",
                "golden-values.json");

        var derivedPath =
            Path.Combine(
                root,
                "tests",
                "golden",
                "phase6",
                "derived-golden-values.json");

        var primaryBytes =
            File.ReadAllBytes(
                primaryPath);

        var actualHash =
            Convert
                .ToHexString(
                    SHA256.HashData(
                        primaryBytes))
                .ToLowerInvariant();

        using var document =
            JsonDocument.Parse(
                File.ReadAllText(
                    derivedPath));

        var provenance =
            document.RootElement
                .GetProperty(
                    "provenance");

        Assert.AreEqual(
            provenance
                .GetProperty(
                    "primaryGoldenSha256")
                .GetString(),
            actualHash);

        Assert.IsFalse(
            provenance
                .GetProperty(
                    "miastroCodeUsedToGenerateExpected")
                .GetBoolean());

        Assert.AreEqual(
            5,
            document.RootElement
                .GetProperty("cases")
                .GetArrayLength());
    }

    private static string RepositoryRoot()
        => Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "../../../../../"));
}
