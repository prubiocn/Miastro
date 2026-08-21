namespace Miastro.Tests.Phase5;

[TestClass]
public sealed class Phase5PeopleUiValidationArchitectureTests
{
    [TestMethod]
    public void Person_editor_has_inline_validation_messages()
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

        foreach (var required in new[]
        {
            "FirstNameError",
            "LastNameError",
            "EmailError",
            "BirthDateError",
            "BirthTimeError",
            "BirthLocationError",
            "ResidenceLocationError"
        })
        {
            StringAssert.Contains(
                xaml,
                required);
        }
    }

    [TestMethod]
    public void Save_validates_before_persistence()
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
            "if (!ValidateEditor())");
    }

    [TestMethod]
    public void Critical_person_fields_have_accessible_names()
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

        foreach (var accessibleName in new[]
        {
            "AutomationProperties.Name=\"Nombre\"",
            "AutomationProperties.Name=\"Apellidos\"",
            "AutomationProperties.Name=\"Email\"",
            "AutomationProperties.Name=\"Fecha de nacimiento\"",
            "AutomationProperties.Name=\"Hora de nacimiento\"",
            "AutomationProperties.Name=\"Eliminar persona\"",
            "AutomationProperties.Name=\"Cancelar edición\""
        })
        {
            StringAssert.Contains(
                xaml,
                accessibleName);
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

            directory =
                directory.Parent;
        }

        throw new DirectoryNotFoundException();
    }
}
