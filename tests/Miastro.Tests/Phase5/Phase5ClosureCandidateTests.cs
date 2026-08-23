namespace Miastro.Tests.Phase5;

[TestClass]
public sealed class Phase5ClosureCandidateTests
{
    [TestMethod]
    public void Phase5_adrs_are_accepted()
    {
        var root =
            FindRepositoryRoot();

        foreach (var file in new[]
        {
            "ADR-007-persona-domain-and-persistence.md",
            "ADR-008-birth-time-precision.md",
            "ADR-009-birth-time-resolution-persistence.md",
            "ADR-010-person-delete-policy.md",
            "ADR-011-person-history-and-recalculation.md"
        })
        {
            var text =
                File.ReadAllText(
                    Path.Combine(
                        root,
                        "docs",
                        "architecture",
                        "ADR",
                        file));

            StringAssert.Contains(
                text,
                "Estado: Aceptado en Fase 5.");
        }
    }

    [TestMethod]
    public void Phase5_debian_builder_has_phase5_version()
    {
        var root =
            FindRepositoryRoot();

        var text =
            File.ReadAllText(
                Path.Combine(
                    root,
                    "tools",
                    "dev",
                    "build-fase5-deb.sh"));

        StringAssert.Contains(
            text,
            "0.5.0~phase5-1");

        StringAssert.Contains(
            text,
            "fase5-linux-x64");
    }

    [TestMethod]
    public void Ci_verifies_xdg_preservation_after_uninstall()
    {
        var root =
            FindRepositoryRoot();

        var text =
            File.ReadAllText(
                Path.Combine(
                    root,
                    ".github",
                    "workflows",
                    "ci.yml"));

        StringAssert.Contains(
            text,
            "XdgPreservedAfterUninstall=PASS");

        StringAssert.Contains(
            text,
            "XdgPreservedAfterReinstall=PASS");

        StringAssert.Contains(
            text,
            "PersonRecoveredAfterReinstall=PASS");

        StringAssert.Contains(
            text,
            "sudo dpkg -r miastro");
    }

    [TestMethod]
    public void Phase5_closure_records_phase6_as_not_started_at_that_time()
    {
        var repositoryRoot =
            Path.GetFullPath(
                Path.Combine(
                    AppContext.BaseDirectory,
                    "../../../../../"));

        var report =
            Path.Combine(
                repositoryRoot,
                "MIASTRO_Fase_5_Informe.md");

        Assert.IsTrue(
            File.Exists(report),
            "Debe existir el informe oficial de cierre de Fase 5.");

        var text =
            File.ReadAllText(report);

        StringAssert.Contains(
            text,
            "Fase 5: CERRADA");

        StringAssert.Contains(
            text,
            "Fase 6: NO INICIADA");
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

            directory =
                directory.Parent;
        }

        throw new DirectoryNotFoundException();
    }
}
