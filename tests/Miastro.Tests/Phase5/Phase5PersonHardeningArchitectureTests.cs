namespace Miastro.Tests.Phase5;

[TestClass]
public sealed class Phase5PersonHardeningArchitectureTests
{
    [TestMethod]
    public void Application_exposes_explicit_update_residence_use_case()
    {
        var root =
            FindRepositoryRoot();

        var source =
            File.ReadAllText(
                Path.Combine(
                    root,
                    "src",
                    "Miastro.Application",
                    "People",
                    "UpdateResidenceUseCase.cs"));

        StringAssert.Contains(
            source,
            "UpdateResidenceUseCase");

        StringAssert.Contains(
            source,
            "CurrentResidenceWriteModel");
    }

    [TestMethod]
    public void Cancel_requires_confirmation_when_editor_is_dirty()
    {
        var root =
            FindRepositoryRoot();

        var source =
            File.ReadAllText(
                Path.Combine(
                    root,
                    "src",
                    "Miastro.UI.Avalonia",
                    "ViewModels",
                    "MainWindowViewModel.cs"));

        StringAssert.Contains(
            source,
            "_cancelArmed");

        StringAssert.Contains(
            source,
            "Hay cambios sin guardar");

        StringAssert.Contains(
            source,
            "Confirmar descartar");
    }

    [TestMethod]
    public void Person_card_displays_relevant_history()
    {
        var root =
            FindRepositoryRoot();

        var xaml =
            File.ReadAllText(
                Path.Combine(
                    root,
                    "src",
                    "Miastro.UI.Avalonia",
                    "Views",
                    "MainWindow.axaml"));

        StringAssert.Contains(
            xaml,
            "Historial");

        StringAssert.Contains(
            xaml,
            "PersonHistory");
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
