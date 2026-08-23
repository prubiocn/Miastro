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
    public void Phase6_closed_report_does_not_start_phase7()
    {
        var current =
            new DirectoryInfo(
                AppContext.BaseDirectory);

        FileInfo? report = null;

        while (current is not null)
        {
            var candidate =
                new FileInfo(
                    Path.Combine(
                        current.FullName,
                        "MIASTRO_Fase_6_Informe.md"));

            if (candidate.Exists)
            {
                report = candidate;
                break;
            }

            current = current.Parent;
        }

        Assert.IsNotNull(
            report,
            "Debe encontrarse MIASTRO_Fase_6_Informe.md desde el árbol de ejecución de tests.");

        var contents =
            File.ReadAllText(report!.FullName);

        Assert.IsTrue(
            contents.Contains(
                "FASE 6 CERRADA",
                StringComparison.Ordinal),
            "El informe debe registrar el cierre formal de Fase 6.");

        Assert.IsTrue(
            contents.Contains(
                "Fase 7 no está iniciada",
                StringComparison.OrdinalIgnoreCase),
            "El cierre de Fase 6 no debe iniciar automáticamente Fase 7.");

        var phase7Started =
            System.Text.RegularExpressions.Regex.IsMatch(
                contents,
                @"(?im)^\s*(?:[-*]\s*)?(?:Fase\s+7\s+iniciada|Phase7Started)\s*[:=]\s*(?:SI|SÍ|YES|TRUE)\s*[;.]?\s*$");

        Assert.IsFalse(
            phase7Started,
            "Fase 7 no debe figurar con estado positivo de inicio.");
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
