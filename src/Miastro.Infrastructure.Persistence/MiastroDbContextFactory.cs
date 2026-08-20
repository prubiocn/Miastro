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
