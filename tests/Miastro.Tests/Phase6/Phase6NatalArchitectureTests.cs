namespace Miastro.Tests.Phase6;

[TestClass]
public sealed class Phase6NatalArchitectureTests
{
    private static readonly string Root =
        Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "../../../../../"));

    [TestMethod]
    public void Natal_application_does_not_reference_swiss_infrastructure()
    {
        var natalDirectory =
            Path.Combine(
                Root,
                "src",
                "Miastro.Application",
                "Natal");

        foreach (var file in Directory.GetFiles(
            natalDirectory,
            "*.cs",
            SearchOption.AllDirectories))
        {
            var text =
                File.ReadAllText(file);

            Assert.IsFalse(
                text.Contains(
                    "Miastro.Infrastructure.SwissEphemeris",
                    StringComparison.Ordinal),
                $"Application/Natal references Swiss infrastructure: {file}");
        }
    }

    [TestMethod]
    public void Calculate_natal_uses_astronomy_ports()
    {
        var file =
            Path.Combine(
                Root,
                "src",
                "Miastro.Application",
                "Natal",
                "CalculateNatalChartUseCase.cs");

        var text =
            File.ReadAllText(file);

        StringAssert.Contains(
            text,
            "IEclipticPositionCalculator");

        StringAssert.Contains(
            text,
            "IHouseCalculator");

        Assert.IsFalse(
            text.Contains(
                "SwissEphemerisPositionCalculator",
                StringComparison.Ordinal));
    }
}
