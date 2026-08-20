#!/usr/bin/env bash
set -euo pipefail

ROOT="/home/pablo/Aplicaciones/Miastro"
TEST="$ROOT/tests/Miastro.Tests"

cd "$ROOT"

mkdir -p "$TEST"

python3 - <<'PY'
from pathlib import Path

p = Path("/home/pablo/Aplicaciones/Miastro/Directory.Packages.props")
text = p.read_text()

entries = {
    "Microsoft.NET.Test.Sdk": "18.9.0",
    "MSTest": "4.3.3",
}

for package, version in entries.items():
    if f'Include="{package}"' not in text:
        text = text.replace(
            "</ItemGroup>",
            f'    <PackageVersion Include="{package}" Version="{version}" />\n'
            "</ItemGroup>",
            1
        )

p.write_text(text)
PY

cat > "$TEST/Miastro.Tests.csproj" <<'EOF'
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" />
    <PackageReference Include="MSTest" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="../../src/Miastro.Domain/Miastro.Domain.csproj" />
    <ProjectReference Include="../../src/Miastro.Application/Miastro.Application.csproj" />
    <ProjectReference Include="../../src/Miastro.Infrastructure.Platform.Linux/Miastro.Infrastructure.Platform.Linux.csproj" />
    <ProjectReference Include="../../src/Miastro.Infrastructure.Persistence/Miastro.Infrastructure.Persistence.csproj" />
    <ProjectReference Include="../../src/Miastro.Bootstrap/Miastro.Bootstrap.csproj" />
  </ItemGroup>

</Project>
EOF

if ! dotnet sln Miastro.sln list | grep -q 'tests/Miastro.Tests/Miastro.Tests.csproj'; then
    dotnet sln Miastro.sln add "$TEST/Miastro.Tests.csproj"
fi

cat > "$TEST/DomainArchitectureTests.cs" <<'EOF'
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
EOF

cat > "$TEST/XdgApplicationPathsTests.cs" <<'EOF'
using Miastro.Infrastructure.Platform.Linux.Xdg;

namespace Miastro.Tests;

[TestClass]
public sealed class XdgApplicationPathsTests
{
    [TestMethod]
    public void Uses_custom_XDG_locations()
    {
        var root = CreateTemporaryRoot();

        WithXdgEnvironment(root, () =>
        {
            var paths = new XdgApplicationPaths();

            Assert.AreEqual(
                Path.Combine(root, "data", "miastro"),
                paths.DataDirectory);

            Assert.AreEqual(
                Path.Combine(root, "config", "miastro"),
                paths.ConfigDirectory);

            Assert.AreEqual(
                Path.Combine(root, "cache", "miastro"),
                paths.CacheDirectory);

            Assert.AreEqual(
                Path.Combine(root, "state", "miastro"),
                paths.StateDirectory);

            Assert.AreEqual(
                Path.Combine(root, "runtime", "miastro"),
                paths.RuntimeDirectory);

            Assert.IsTrue(Directory.Exists(paths.DataDirectory));
            Assert.IsTrue(Directory.Exists(paths.ConfigDirectory));
            Assert.IsTrue(Directory.Exists(paths.CacheDirectory));
            Assert.IsTrue(Directory.Exists(paths.StateDirectory));
            Assert.IsTrue(Directory.Exists(paths.RuntimeDirectory));
        });

        Directory.Delete(root, recursive: true);
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

    internal static void WithXdgEnvironment(
        string root,
        Action action)
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

            action();
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
EOF

cat > "$TEST/BootstrapTests.cs" <<'EOF'
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
EOF

cat > "$TEST/ProjectReferenceArchitectureTests.cs" <<'EOF'
using System.Xml.Linq;

namespace Miastro.Tests;

[TestClass]
public sealed class ProjectReferenceArchitectureTests
{
    private static readonly string Root =
        FindRepositoryRoot();

    [TestMethod]
    public void UI_has_no_direct_persistence_or_swiss_reference()
    {
        var refs = GetProjectReferences(
            "src/Miastro.UI.Avalonia/Miastro.UI.Avalonia.csproj");

        Assert.IsFalse(
            refs.Any(x => x.Contains(
                "Infrastructure.Persistence",
                StringComparison.Ordinal)));

        Assert.IsFalse(
            refs.Any(x => x.Contains(
                "Infrastructure.SwissEphemeris",
                StringComparison.Ordinal)));
    }

    [TestMethod]
    public void Domain_has_no_project_dependencies()
    {
        var refs = GetProjectReferences(
            "src/Miastro.Domain/Miastro.Domain.csproj");

        Assert.AreEqual(0, refs.Count);
    }

    [TestMethod]
    public void Interpretation_has_no_swiss_dependency()
    {
        var refs = GetProjectReferences(
            "src/Miastro.Interpretation/Miastro.Interpretation.csproj");

        Assert.IsFalse(
            refs.Any(x => x.Contains(
                "SwissEphemeris",
                StringComparison.Ordinal)));
    }

    [TestMethod]
    public void Persistence_has_no_UI_dependency()
    {
        var refs = GetProjectReferences(
            "src/Miastro.Infrastructure.Persistence/Miastro.Infrastructure.Persistence.csproj");

        Assert.IsFalse(
            refs.Any(x => x.Contains(
                "UI.Avalonia",
                StringComparison.Ordinal)));
    }

    [TestMethod]
    public void Graphics_has_no_persistence_dependency()
    {
        var refs = GetProjectReferences(
            "src/Miastro.Graphics/Miastro.Graphics.csproj");

        Assert.IsFalse(
            refs.Any(x =>
                x.Contains(
                    "Infrastructure.Persistence",
                    StringComparison.Ordinal)));
    }

    private static List<string> GetProjectReferences(
        string relativePath)
    {
        var file = Path.Combine(Root, relativePath);

        var document = XDocument.Load(file);

        return document
            .Descendants("ProjectReference")
            .Select(x =>
                x.Attribute("Include")?.Value ?? string.Empty)
            .Where(x => x.Length > 0)
            .ToList();
    }

    private static string FindRepositoryRoot()
    {
        var current =
            new DirectoryInfo(AppContext.BaseDirectory);

        while (current is not null)
        {
            if (File.Exists(
                Path.Combine(current.FullName, "Miastro.sln")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new InvalidOperationException(
            "No se encontró la raíz del repositorio Miastro.");
    }
}
EOF

dotnet restore Miastro.sln

dotnet build Miastro.sln \
  --configuration Release \
  --no-restore

dotnet test tests/Miastro.Tests/Miastro.Tests.csproj \
  --configuration Release \
  --no-build \
  --logger "console;verbosity=normal"

echo
echo "=== FASE 1 — TESTS ==="
echo "Build: OK"
echo "Tests unitarios/técnicos: OK"
echo "Arquitectura: OK"
echo "XDG personalizado: OK"
echo "Configuración: OK"
echo "SQLite: OK"
echo "DI: OK"
