namespace Miastro.Tests.Phase5;

[TestClass]
public sealed class Phase5BirthResidenceUiArchitectureTests
{
    [TestMethod]
    public void Person_card_exposes_birth_and_residence_workflow()
    {
        var xaml = ReadMainWindow();

        foreach (var required in new[]
        {
            "Nacimiento",
            "Fecha",
            "Precisión de la hora",
            "Lugar de nacimiento",
            "Buscar localidad",
            "Resolver hora histórica",
            "Elegir primera posibilidad",
            "Elegir segunda posibilidad",
            "Residencia actual"
        })
        {
            StringAssert.Contains(
                xaml,
                required);
        }
    }

    [TestMethod]
    public void Person_card_does_not_expose_technical_location_fields()
    {
        var xaml = ReadMainWindow();

        foreach (var forbidden in new[]
        {
            "GeoNameId",
            "IanaTimeZoneId",
            "TzdbVersion",
            "HistoricalOffsetSeconds",
            "ResolvedInstantUtc",
            "SQLite",
            "XDG"
        })
        {
            Assert.IsFalse(
                xaml.Contains(
                    forbidden,
                    StringComparison.OrdinalIgnoreCase),
                $"Technical field exposed: {forbidden}");
        }
    }

    [TestMethod]
    public void Birth_ui_has_explicit_ambiguity_choice()
    {
        var xaml = ReadMainWindow();

        StringAssert.Contains(
            xaml,
            "Elegir primera posibilidad");

        StringAssert.Contains(
            xaml,
            "Elegir segunda posibilidad");

        var vm =
            ReadDetailsViewModel();

        StringAssert.Contains(
            vm,
            "selectedCandidate");

        StringAssert.Contains(
            vm,
            "DateTimeOffset.UtcNow");
    }

    [TestMethod]
    public void Unknown_and_range_do_not_require_historical_resolution()
    {
        var vm =
            ReadDetailsViewModel();

        StringAssert.Contains(
            vm,
            "BirthTimePrecision.Unknown");

        StringAssert.Contains(
            vm,
            "BirthTimePrecision.Range");

        StringAssert.Contains(
            vm,
            "BirthTemporalResolutionState.NotApplicable");
    }

    private static string ReadMainWindow()
        => File.ReadAllText(
            Path.Combine(
                FindRepositoryRoot(),
                "src",
                "Miastro.UI.Avalonia",
                "Views",
                "MainWindow.axaml"));

    private static string ReadDetailsViewModel()
        => File.ReadAllText(
            Path.Combine(
                FindRepositoryRoot(),
                "src",
                "Miastro.UI.Avalonia",
                "ViewModels",
                "MainWindowViewModel.PeopleDetails.cs"));

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

        throw new DirectoryNotFoundException();
    }
}
