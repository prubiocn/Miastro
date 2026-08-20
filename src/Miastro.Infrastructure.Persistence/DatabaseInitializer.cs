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
