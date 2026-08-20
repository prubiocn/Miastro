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
