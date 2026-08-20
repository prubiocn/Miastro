using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Miastro.Application.Configuration;
using Miastro.Application.Platform;
using Miastro.Bootstrap;
using Miastro.Infrastructure.Persistence;

namespace Miastro.Tests;

[TestClass]
public sealed class BootstrapTests
{
    [TestMethod]
    public void DI_container_builds_and_resolves_core_services()
    {
        var root = CreateTemporaryRoot();

        XdgApplicationPathsTests.WithXdgEnvironment(root, () =>
        {
            var services = MiastroBootstrap.CreateServiceCollection();

            using var provider = services.BuildServiceProvider(
                new ServiceProviderOptions
                {
                    ValidateOnBuild = true,
                    ValidateScopes = true
                });

            Assert.IsNotNull(
                provider.GetRequiredService<IApplicationPaths>());

            Assert.IsNotNull(
                provider.GetRequiredService<ApplicationSettings>());

            Assert.IsNotNull(
                provider.GetRequiredService<DatabaseInitializer>());
        });

        Directory.Delete(root, recursive: true);
    }

    [TestMethod]
    public void Bootstrap_creates_minimal_settings_file()
    {
        var root = CreateTemporaryRoot();

        XdgApplicationPathsTests.WithXdgEnvironment(root, () =>
        {
            var services = MiastroBootstrap.CreateServiceCollection();

            using var provider = services.BuildServiceProvider();

            var paths =
                provider.GetRequiredService<IApplicationPaths>();

            Assert.IsTrue(File.Exists(paths.SettingsPath));

            var text = File.ReadAllText(paths.SettingsPath);

            StringAssert.Contains(text, "\"schemaVersion\"");
            StringAssert.Contains(text, "\"language\"");
        });

        Directory.Delete(root, recursive: true);
    }

    [TestMethod]
    public async Task SQLite_migration_and_write_probe_work()
    {
        var root = CreateTemporaryRoot();

        try
        {
            await WithXdgEnvironmentAsync(root, async () =>
            {
                var services =
                    MiastroBootstrap.CreateServiceCollection();

                await using var provider =
                    services.BuildServiceProvider(
                        new ServiceProviderOptions
                        {
                            ValidateOnBuild = true,
                            ValidateScopes = true
                        });

                await MiastroBootstrap.InitializeAsync(provider);

                var paths =
                    provider.GetRequiredService<IApplicationPaths>();

                Assert.IsTrue(File.Exists(paths.DatabasePath));

                await using var scope =
                    provider.CreateAsyncScope();

                var db = scope.ServiceProvider
                    .GetRequiredService<MiastroDbContext>();

                Assert.IsTrue(
                    await db.Database.CanConnectAsync());

                var applied =
                    await db.Database.GetAppliedMigrationsAsync();

                Assert.IsTrue(applied.Any());
            });
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, recursive: true);
            }
        }
    }

    private static string CreateTemporaryRoot()
    {
        var root = Path.Combine(
            Path.GetTempPath(),
            "miastro-tests",
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(root);
        return root;
    }

    private static async Task WithXdgEnvironmentAsync(
        string root,
        Func<Task> action)
    {
        var names = new[]
        {
            "XDG_DATA_HOME",
            "XDG_CONFIG_HOME",
            "XDG_CACHE_HOME",
            "XDG_STATE_HOME",
            "XDG_RUNTIME_DIR"
        };

        var previous = names.ToDictionary(
            x => x,
            Environment.GetEnvironmentVariable);

        try
        {
            Environment.SetEnvironmentVariable(
                "XDG_DATA_HOME",
                Path.Combine(root, "data"));

            Environment.SetEnvironmentVariable(
                "XDG_CONFIG_HOME",
                Path.Combine(root, "config"));

            Environment.SetEnvironmentVariable(
                "XDG_CACHE_HOME",
                Path.Combine(root, "cache"));

            Environment.SetEnvironmentVariable(
                "XDG_STATE_HOME",
                Path.Combine(root, "state"));

            Environment.SetEnvironmentVariable(
                "XDG_RUNTIME_DIR",
                Path.Combine(root, "runtime"));

            await action();
        }
        finally
        {
            foreach (var item in previous)
            {
                Environment.SetEnvironmentVariable(
                    item.Key,
                    item.Value);
            }
        }
    }
}
