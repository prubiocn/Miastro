using Microsoft.Extensions.DependencyInjection;
using Miastro.Application.Natal;
using Miastro.Astronomy.Abstractions.Contracts;
using Miastro.Bootstrap;

namespace Miastro.Tests.Phase6;

[TestClass]
public sealed class Phase6AstronomyCompositionTests
{
    [TestMethod]
    public void Bootstrap_registers_natal_astronomy_ports()
    {
        var services =
            MiastroBootstrap
                .CreateServiceCollection();

        using var provider =
            services.BuildServiceProvider(
                new ServiceProviderOptions
                {
                    ValidateOnBuild = true,
                    ValidateScopes = true
                });

        Assert.IsNotNull(
            provider.GetRequiredService<
                IEclipticPositionCalculator>());

        Assert.IsNotNull(
            provider.GetRequiredService<
                IHouseCalculator>());

        Assert.IsNotNull(
            provider.GetRequiredService<
                IAstronomyEngineDiagnostics>());

        Assert.IsNotNull(
            provider.GetRequiredService<
                INatalCalculationMetadataProvider>());

        using var scope =
            provider.CreateScope();

        Assert.IsNotNull(
            scope.ServiceProvider
                .GetRequiredService<
                    CalculateNatalChartUseCase>());
    }
}
