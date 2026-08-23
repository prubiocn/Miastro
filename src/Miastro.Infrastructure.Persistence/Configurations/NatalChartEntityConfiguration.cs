using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Miastro.Infrastructure.Persistence.Entities;

namespace Miastro.Infrastructure.Persistence.Configurations;

public sealed class NatalChartEntityConfiguration
    : IEntityTypeConfiguration<NatalChartEntity>
{
    public void Configure(
        EntityTypeBuilder<NatalChartEntity> builder)
    {
        builder.ToTable("NatalCharts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.InputHash)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(x => x.BirthDataHash)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(x => x.AmbiguousSelection)
            .HasMaxLength(40);

        builder.Property(x => x.Locality)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.IanaTimeZoneId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.TzdbVersion)
            .IsRequired()
            .HasMaxLength(80);

        builder.Property(x => x.CalculationProfileId)
            .IsRequired()
            .HasMaxLength(80);

        builder.Property(x => x.MiastroVersion)
            .IsRequired()
            .HasMaxLength(80);

        builder.Property(x => x.Engine)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(x => x.EngineVersion)
            .IsRequired()
            .HasMaxLength(80);

        builder.Property(x => x.AdapterVersion)
            .IsRequired()
            .HasMaxLength(80);

        builder.Property(x => x.EphemerisVersion)
            .IsRequired()
            .HasMaxLength(160);

        builder.HasIndex(x => new
        {
            x.PersonId,
            x.InputHash
        })
        .IsUnique();

        builder.HasIndex(x => new
        {
            x.PersonId,
            x.Status
        });

        builder.HasIndex(x => x.CalculatedAtUtc);

        builder.HasOne<PersonEntity>()
            .WithMany()
            .HasForeignKey(x => x.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Placements)
            .WithOne(x => x.Chart)
            .HasForeignKey(x => x.ChartId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.HouseCusps)
            .WithOne(x => x.Chart)
            .HasForeignKey(x => x.ChartId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Aspects)
            .WithOne(x => x.Chart)
            .HasForeignKey(x => x.ChartId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
