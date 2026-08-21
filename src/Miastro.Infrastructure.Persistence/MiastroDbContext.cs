using Microsoft.EntityFrameworkCore;
using Miastro.Infrastructure.Persistence.Entities;

namespace Miastro.Infrastructure.Persistence;

public sealed class MiastroDbContext : DbContext
{
    public MiastroDbContext(
        DbContextOptions<MiastroDbContext> options)
        : base(options)
    {
    }

    public DbSet<PersonEntity> People => Set<PersonEntity>();
    public DbSet<BirthDataEntity> BirthData => Set<BirthDataEntity>();
    public DbSet<CurrentResidenceEntity> CurrentResidences
        => Set<CurrentResidenceEntity>();
    public DbSet<PersonHistoryEntity> PersonHistory
        => Set<PersonHistoryEntity>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(MiastroDbContext).Assembly);
    }
}
