using Miastro.Domain;

namespace Miastro.Tests;

[TestClass]
public sealed class DomainArchitectureTests
{
    [TestMethod]
    public void Domain_has_no_forbidden_technical_dependencies()
    {
        var references = typeof(DomainAssemblyMarker)
            .Assembly
            .GetReferencedAssemblies()
            .Select(x => x.Name ?? string.Empty)
            .ToArray();

        string[] forbidden =
        [
            "Avalonia",
            "Microsoft.EntityFrameworkCore",
            "SkiaSharp",
            "Miastro.Infrastructure",
            "Miastro.UI"
        ];

        foreach (var prefix in forbidden)
        {
            Assert.IsFalse(
                references.Any(x =>
                    x.StartsWith(prefix, StringComparison.Ordinal)),
                $"Domain depende de una tecnología prohibida: {prefix}");
        }
    }
}
