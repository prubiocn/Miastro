namespace Miastro.Tests.Phase6;

[TestClass]
public sealed class Phase6ClosureCandidateTests
{
    [TestMethod]
    public void Phase6_debian_builder_has_phase6_version()
    {
        var builder =
            Read(
                "tools/dev/"
                + "build-fase6-deb.sh");

        StringAssert.Contains(
            builder,
            "0.6.0~phase6-1");

        StringAssert.Contains(
            builder,
            "fase6-linux-x64");

        StringAssert.Contains(
            builder,
            "fase6-deb");
    }

    [TestMethod]
    public void Ci_publishes_and_packages_phase6()
    {
        var workflow =
            Read(
                ".github/workflows/ci.yml");

        StringAssert.Contains(
            workflow,
            "artifacts/publish/fase6-linux-x64");

        StringAssert.Contains(
            workflow,
            "Build Phase 6 Debian package");

        StringAssert.Contains(
            workflow,
            "Install Phase 6 Debian package");

        StringAssert.Contains(
            workflow,
            "tools/dev/build-fase6-deb.sh");

        Assert.IsFalse(
            workflow.Contains(
                "artifacts/publish/fase5-linux-x64",
                StringComparison.Ordinal));
    }

    [TestMethod]
    public void Phase6_adr_file_numbers_match_headings()
    {
        for (
            var number = 28;
            number <= 40;
            number++)
        {
            var prefix =
                $"ADR-{number:000}-";

            var directory =
                Path.Combine(
                    Root,
                    "docs",
                    "architecture",
                    "ADR");

            var file =
                Directory
                    .GetFiles(
                        directory,
                        prefix + "*.md")
                    .Single();

            var firstLine =
                File.ReadLines(file)
                    .First();

            Assert.IsTrue(
                firstLine.StartsWith(
                    $"# ADR-{number:000} —",
                    StringComparison.Ordinal),
                $"Cabecera ADR incoherente: {file}: {firstLine}");
        }
    }

    [TestMethod]
    public void Phase6_report_still_blocks_premature_phase7()
    {
        var report =
            Read(
                "MIASTRO_Fase_6_Informe.md");

        StringAssert.Contains(
            report,
            "La Fase 7 no está iniciada.");

        Assert.IsFalse(
            report.Contains(
                "FASE 6 CERRADA",
                StringComparison.OrdinalIgnoreCase));
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
