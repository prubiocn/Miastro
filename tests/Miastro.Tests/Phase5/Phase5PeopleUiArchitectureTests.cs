namespace Miastro.Tests.Phase5;

[TestClass]
public sealed class Phase5PeopleUiArchitectureTests
{
    [TestMethod]
    public void Main_window_contains_people_workflow()
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
            "Nueva persona",
            "Buscar por nombre o apellidos",
            "Identidad",
            "Contacto",
            "Nota privada",
            "Guardar",
            "Cancelar",
            "Eliminar",
            "Registrar consulta ahora"
        })
        {
            StringAssert.Contains(
                xaml,
                required);
        }
    }

    [TestMethod]
    public void Person_ui_does_not_display_internal_technical_fields()
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

        foreach (var forbidden in new[]
        {
            "GeoNameId",
            "TZDB",
            "Julian",
            "SQLite",
            "XDG",
            "__EFMigrationsHistory",
            "/usr/share",
            "/home/"
        })
        {
            Assert.IsFalse(
                xaml.Contains(
                    forbidden,
                    StringComparison.OrdinalIgnoreCase),
                $"Technical value exposed in UI: {forbidden}");
        }
    }

    [TestMethod]
    public void Main_window_viewmodel_does_not_capture_scoped_person_services()
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
            "IServiceScopeFactory");

        Assert.IsFalse(
            source.Contains(
                "MainWindowViewModel("
                + Environment.NewLine
                + "        CreatePersonUseCase",
                StringComparison.Ordinal));
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

        throw new DirectoryNotFoundException(
            "Miastro repository root not found.");
    }
}
