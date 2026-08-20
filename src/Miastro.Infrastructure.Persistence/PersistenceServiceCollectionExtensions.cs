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
