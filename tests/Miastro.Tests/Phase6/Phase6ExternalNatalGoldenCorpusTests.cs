using System.Security.Cryptography;
using System.Text.Json;

namespace Miastro.Tests.Phase6;

[TestClass]
public sealed class Phase6ExternalNatalGoldenCorpusTests
{
    [TestMethod]
    public void Golden_corpus_contains_five_external_charts()
    {
        using var document =
            LoadGolden();

        var root =
            document.RootElement;

        Assert.IsTrue(
            root
                .GetProperty("source")
                .GetProperty(
                    "independentFromMiastroAdapter")
                .GetBoolean());

        Assert.IsFalse(
            root
                .GetProperty("source")
                .GetProperty(
                    "expectedValuesGeneratedByMiastro")
                .GetBoolean());

        var cases =
            root
                .GetProperty("cases")
                .EnumerateArray()
                .ToArray();

        Assert.AreEqual(
            5,
            cases.Length);

        Assert.AreEqual(
            3,
            cases.Count(x =>
                x.GetProperty("category")
                    .GetString()
                == "modern"));

        Assert.AreEqual(
            2,
            cases.Count(x =>
                x.GetProperty("category")
                    .GetString()
                == "historical"));

        foreach (var goldenCase in cases)
        {
            Assert.AreEqual(
                17,
                goldenCase
                    .GetProperty("positions")
                    .GetArrayLength());

            Assert.AreEqual(
                12,
                goldenCase
                    .GetProperty("houses")
                    .GetProperty("cusps")
                    .GetArrayLength());
        }
    }

    [TestMethod]
    public void Raw_external_files_match_manifest_hashes()
    {
        var root =
            RepositoryRoot();

        var manifestPath =
            Path.Combine(
                root,
                "tests",
                "golden",
                "phase6",
                "external-reference-manifest.json");

        using var manifest =
            JsonDocument.Parse(
                File.ReadAllText(
                    manifestPath));

        foreach (
            var entry
            in manifest.RootElement
                .GetProperty("rawResponses")
                .EnumerateArray())
        {
            var relative =
                entry
                    .GetProperty("file")
                    .GetString();

            Assert.IsNotNull(relative);

            var file =
                Path.Combine(
                    root,
                    "tests",
                    "golden",
                    "phase6",
                    relative);

            Assert.IsTrue(
                File.Exists(file),
                file);

            var bytes =
                File.ReadAllBytes(file);

            Assert.AreEqual(
                entry
                    .GetProperty("size")
                    .GetInt32(),
                bytes.Length);

            var actual =
                Convert
                    .ToHexString(
                        SHA256.HashData(
                            bytes))
                    .ToLowerInvariant();

            Assert.AreEqual(
                entry
                    .GetProperty("sha256")
                    .GetString(),
                actual);
        }
    }

    private static JsonDocument LoadGolden()
    {
        var path =
            Path.Combine(
                RepositoryRoot(),
                "tests",
                "golden",
                "phase6",
                "golden-values.json");

        return JsonDocument.Parse(
            File.ReadAllText(path));
    }

    private static string RepositoryRoot()
        => Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "../../../../../"));
}
