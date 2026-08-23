using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Miastro.Application.Natal;
using Miastro.Application.People;
using Miastro.Bootstrap;
using Miastro.Domain.Houses;
using Miastro.Domain.People;
using Miastro.Infrastructure.Persistence;

namespace Miastro.Tests.Phase6;

[TestClass]
public sealed class Phase6RecalculateNatalChartTests
{
    [TestMethod]
    public async Task Recalculate_same_current_input_does_not_duplicate_snapshot()
    {
        var root =
            Path.Combine(
                Path.GetTempPath(),
                "miastro-phase6-recalc",
                Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(root);

        var previous =
            new Dictionary<string, string?>();

        try
        {
            SetXdg(
                previous,
                "XDG_DATA_HOME",
                Path.Combine(root, "data"));

            SetXdg(
                previous,
                "XDG_CONFIG_HOME",
                Path.Combine(root, "config"));

            SetXdg(
                previous,
                "XDG_CACHE_HOME",
                Path.Combine(root, "cache"));

            SetXdg(
                previous,
                "XDG_STATE_HOME",
                Path.Combine(root, "state"));

            var services =
                MiastroBootstrap
                    .CreateServiceCollection();

            await using var provider =
                services.BuildServiceProvider(
                    new ServiceProviderOptions
                    {
                        ValidateOnBuild = true,
                        ValidateScopes = true
                    });

            await using var scope =
                provider.CreateAsyncScope();

            var db =
                scope.ServiceProvider
                    .GetRequiredService<
                        MiastroDbContext>();

            await db.Database.MigrateAsync();

            var create =
                scope.ServiceProvider
                    .GetRequiredService<
                        CreatePersonUseCase>();

            var personId =
                await create.ExecuteAsync(
                    new CreatePersonCommand(
                        "Persona",
                        "Recalculo",
                        null,
                        null,
                        null,
                        false,
                        Birth(),
                        null),
                    new DateTimeOffset(
                        2026, 8, 21,
                        10, 0, 0,
                        TimeSpan.Zero));

            var calculate =
                scope.ServiceProvider
                    .GetRequiredService<
                        CalculateNatalChartUseCase>();

            var first =
                await calculate.ExecuteAsync(
                    personId,
                    HouseSystem.Placidus,
                    new DateTimeOffset(
                        2026, 8, 21,
                        10, 1, 0,
                        TimeSpan.Zero));

            Assert.IsTrue(
                first.Success,
                first.Message);

            var recalculate =
                scope.ServiceProvider
                    .GetRequiredService<
                        RecalculateNatalChartUseCase>();

            var second =
                await recalculate.ExecuteAsync(
                    personId,
                    HouseSystem.Placidus,
                    new DateTimeOffset(
                        2026, 8, 21,
                        10, 2, 0,
                        TimeSpan.Zero));

            Assert.AreEqual(
                NatalCalculationResultCode
                    .ExistingCurrentSnapshot,
                second.Code);

            Assert.AreEqual(
                first.Snapshot!.Id,
                second.Snapshot!.Id);

            Assert.AreEqual(
                1,
                await db.NatalCharts
                    .CountAsync());
        }
        finally
        {
            foreach (var item in previous)
            {
                Environment.SetEnvironmentVariable(
                    item.Key,
                    item.Value);
            }

            try
            {
                Directory.Delete(
                    root,
                    recursive: true);
            }
            catch
            {
            }
        }
    }

    private static BirthDataWriteModel Birth()
        => new(
            new DateOnly(
                2000, 1, 1),
            BirthTimePrecision.Exact,
            new TimeOnly(
                12, 0),
            null,
            null,
            null,
            3117735,
            "Madrid",
            "España",
            "Madrid",
            null,
            40.4168,
            -3.7038,
            "Europe/Madrid",
            "TZDB: 2026c",
            BirthTemporalResolutionState.Resolved,
            3600,
            new DateTimeOffset(
                2000, 1, 1,
                11, 0, 0,
                TimeSpan.Zero),
            null,
            null,
            null,
            null,
            null,
            null,
            false,
            null,
            null);

    private static void SetXdg(
        IDictionary<string, string?> previous,
        string name,
        string value)
    {
        previous[name] =
            Environment.GetEnvironmentVariable(
                name);

        Directory.CreateDirectory(
            value);

        Environment.SetEnvironmentVariable(
            name,
            value);
    }
}
