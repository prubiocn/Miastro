using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Miastro.Infrastructure.Persistence.Entities;

namespace Miastro.Infrastructure.Persistence.Configurations;

public sealed class BirthDataEntityConfiguration
    : IEntityTypeConfiguration<BirthDataEntity>
{
    public void Configure(
        EntityTypeBuilder<BirthDataEntity> builder)
    {
        builder.ToTable("BirthData");
        builder.HasKey(x => x.PersonId);

        builder.Property(x => x.Locality)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Country)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(x => x.Region)
            .IsRequired()
            .HasMaxLength(160);

        builder.Property(x => x.Subregion)
            .HasMaxLength(160);

        builder.Property(x => x.IanaTimeZoneId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.TzdbVersion)
            .HasMaxLength(80);

        builder.HasIndex(x => x.GeoNameId);
        builder.HasIndex(x => x.IanaTimeZoneId);
    }
}
