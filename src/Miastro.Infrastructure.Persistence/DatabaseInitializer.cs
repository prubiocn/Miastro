using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Miastro.Infrastructure.Persistence;

public sealed class DatabaseInitializer
{
    private readonly IServiceScopeFactory _scopeFactory;

    public DatabaseInitializer(
        IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory
            ?? throw new ArgumentNullException(nameof(scopeFactory));
    }

    public async Task InitializeAsync(
        CancellationToken cancellationToken = default)
    {
        await using var scope =
            _scopeFactory.CreateAsyncScope();

        var dbContext =
            scope.ServiceProvider
                .GetRequiredService<MiastroDbContext>();

        await dbContext.Database.MigrateAsync(
            cancellationToken);
    }
}
