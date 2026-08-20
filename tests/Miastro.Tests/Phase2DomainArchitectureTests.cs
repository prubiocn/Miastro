using Miastro.Domain;

namespace Miastro.Tests;

[TestClass]
public sealed class Phase2DomainArchitectureTests
{
    [TestMethod]
    public void Domain_has_no_forbidden_runtime_dependencies()
    {
        var references =
            typeof(DomainAssemblyMarker)
                .Assembly
                .GetReferencedAssemblies()
                .Select(x => x.Name ?? string.Empty)
                .ToArray();

        string[] forbidden =
        [
            "Avalonia",
            "Microsoft.EntityFrameworkCore",
            "SkiaSharp",
            "Miastro.Infrastructure.Persistence",
            "Miastro.Infrastructure.SwissEphemeris",
            "Miastro.Infrastructure.Geography",
            "Miastro.Infrastructure.Time",
            "Miastro.Infrastructure.Platform.Linux",
            "Miastro.Infrastructure.Printing.Linux"
        ];

        foreach (var dependency in forbidden)
        {
            Assert.IsFalse(
                references.Any(reference =>
                    reference.StartsWith(
                        dependency,
                        StringComparison.Ordinal)),
                $"Domain referencia una dependencia prohibida: {dependency}");
        }
    }
}
