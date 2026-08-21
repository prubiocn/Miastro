using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Miastro.Infrastructure.Persistence.Entities;

namespace Miastro.Infrastructure.Persistence.Configurations;

public sealed class CurrentResidenceEntityConfiguration
    : IEntityTypeConfiguration<CurrentResidenceEntity>
{
    public void Configure(
        EntityTypeBuilder<CurrentResidenceEntity> builder)
    {
        builder.ToTable("CurrentResidences");
        builder.HasKey(x => x.PersonId);

        builder.Property(x => x.Locality)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Region)
            .IsRequired()
            .HasMaxLength(160);

        builder.Property(x => x.Country)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(x => x.IanaTimeZoneId)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.GeoNameId);
    }
}
