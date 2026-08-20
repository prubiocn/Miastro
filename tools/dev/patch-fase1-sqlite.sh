#!/usr/bin/env bash
set -euo pipefail

ROOT="/home/pablo/Aplicaciones/Miastro"
cd "$ROOT"

mkdir -p \
  src/Miastro.Infrastructure.Persistence/Entities \
  src/Miastro.Infrastructure.Persistence/Migrations

cat > src/Miastro.Infrastructure.Persistence/Entities/TechnicalProbe.cs <<'EOF'
namespace Miastro.Infrastructure.Persistence.Entities;

internal sealed class TechnicalProbe
{
    public int Id { get; set; }

    public string Value { get; set; } = string.Empty;

    public DateTimeOffset CreatedUtc { get; set; }
}
EOF

cat > src/Miastro.Infrastructure.Persistence/MiastroDbContext.cs <<'EOF'
using Microsoft.EntityFrameworkCore;
using Miastro.Infrastructure.Persistence.Entities;

namespace Miastro.Infrastructure.Persistence;

public sealed class MiastroDbContext : DbContext
{
    public MiastroDbContext(DbContextOptions<MiastroDbContext> options)
        : base(options)
    {
    }

    internal DbSet<TechnicalProbe> TechnicalProbes => Set<TechnicalProbe>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var probe = modelBuilder.Entity<TechnicalProbe>();

        probe.ToTable("TechnicalProbes");
        probe.HasKey(x => x.Id);

        probe.Property(x => x.Value)
            .HasMaxLength(128)
            .IsRequired();

        probe.Property(x => x.CreatedUtc)
            .IsRequired();
    }
}
EOF

cat > src/Miastro.Infrastructure.Persistence/PersistenceServiceCollectionExtensions.cs <<'EOF'
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Miastro.Application.Platform;

namespace Miastro.Infrastructure.Persistence;

public static class PersistenceServiceCollectionExtensions
{
    public static IServiceCollection AddMiastroPersistence(
        this IServiceCollection services)
    {
        services.AddDbContext<MiastroDbContext>((provider, options) =>
        {
            var paths = provider.GetRequiredService<IApplicationPaths>();

            options.UseSqlite(
                $"Data Source={paths.DatabasePath}");
        });

        services.AddSingleton<DatabaseInitializer>();

        return services;
    }
}
EOF

cat > src/Miastro.Infrastructure.Persistence/DatabaseInitializer.cs <<'EOF'
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Miastro.Application.Platform;
using Miastro.Infrastructure.Persistence.Entities;

namespace Miastro.Infrastructure.Persistence;

public sealed class DatabaseInitializer
{
    private readonly IServiceProvider _services;
    private readonly IApplicationPaths _paths;

    public DatabaseInitializer(
        IServiceProvider services,
        IApplicationPaths paths)
    {
        _services = services;
        _paths = paths;
    }

    public async Task InitializeAsync(
        CancellationToken cancellationToken = default)
    {
        await using var scope = _services.CreateAsyncScope();

        var db = scope.ServiceProvider
            .GetRequiredService<MiastroDbContext>();

        await db.Database.MigrateAsync(cancellationToken);

        var exists = await db.TechnicalProbes
            .AnyAsync(cancellationToken);

        if (!exists)
        {
            db.TechnicalProbes.Add(
                new TechnicalProbe
                {
                    Value = "phase1",
                    CreatedUtc = DateTimeOffset.UtcNow
                });

            await db.SaveChangesAsync(cancellationToken);
        }

        if (OperatingSystem.IsLinux() &&
            File.Exists(_paths.DatabasePath))
        {
            File.SetUnixFileMode(
                _paths.DatabasePath,
                UnixFileMode.UserRead |
                UnixFileMode.UserWrite);
        }
    }
}
EOF

cat > src/Miastro.Infrastructure.Persistence/MiastroDbContextFactory.cs <<'EOF'
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Miastro.Infrastructure.Persistence;

public sealed class MiastroDbContextFactory
    : IDesignTimeDbContextFactory<MiastroDbContext>
{
    public MiastroDbContext CreateDbContext(string[] args)
    {
        var databasePath = Path.Combine(
            Path.GetTempPath(),
            "miastro-design-time.db");

        var options = new DbContextOptionsBuilder<MiastroDbContext>()
            .UseSqlite($"Data Source={databasePath}")
            .Options;

        return new MiastroDbContext(options);
    }
}
EOF

python3 - <<'PY'
from pathlib import Path

path = Path("/home/pablo/Aplicaciones/Miastro/src/Miastro.Bootstrap/MiastroBootstrap.cs")
text = path.read_text()

if "using Miastro.Infrastructure.Persistence;" not in text:
    text = text.replace(
        "using Miastro.Infrastructure.Platform.Linux.Xdg;",
        "using Miastro.Infrastructure.Platform.Linux.Xdg;\nusing Miastro.Infrastructure.Persistence;"
    )

if "services.AddMiastroPersistence();" not in text:
    text = text.replace(
        "services.AddSingleton(settings);",
        "services.AddSingleton(settings);\n\n        services.AddMiastroPersistence();"
    )

path.write_text(text)
PY

python3 - <<'PY'
from pathlib import Path

path = Path("/home/pablo/Aplicaciones/Miastro/src/Miastro.UI.Avalonia/Program.cs")
text = path.read_text()

if "using Miastro.Infrastructure.Persistence;" not in text:
    text = text.replace(
        "using Miastro.UI.Avalonia.ViewModels;",
        "using Miastro.UI.Avalonia.ViewModels;\nusing Miastro.Infrastructure.Persistence;"
    )

needle = """            var logger = App.Services
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger("Miastro.Startup");
"""

replacement = """            var initializer = App.Services
                .GetRequiredService<DatabaseInitializer>();

            initializer.InitializeAsync()
                .GetAwaiter()
                .GetResult();

            var logger = App.Services
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger("Miastro.Startup");
"""

if "GetRequiredService<DatabaseInitializer>()" not in text:
    text = text.replace(needle, replacement)

path.write_text(text)
PY

if [[ ! -f .config/dotnet-tools.json ]]; then
    dotnet new tool-manifest
fi

if ! dotnet tool list | grep -q '^dotnet-ef '; then
    dotnet tool install dotnet-ef --version 10.0.11
fi

rm -rf src/Miastro.Infrastructure.Persistence/Migrations

dotnet tool run dotnet-ef migrations add InitialTechnicalSchema \
  --project src/Miastro.Infrastructure.Persistence/Miastro.Infrastructure.Persistence.csproj \
  --startup-project src/Miastro.Infrastructure.Persistence/Miastro.Infrastructure.Persistence.csproj \
  --output-dir Migrations

dotnet restore Miastro.sln

dotnet build Miastro.sln \
  --configuration Release \
  --no-restore

echo
echo "=== MIGRACIONES ==="
dotnet tool run dotnet-ef migrations list \
  --project src/Miastro.Infrastructure.Persistence/Miastro.Infrastructure.Persistence.csproj \
  --startup-project src/Miastro.Infrastructure.Persistence/Miastro.Infrastructure.Persistence.csproj \
  --no-build

echo
echo "PATCH SQLITE COMPLETADO"
