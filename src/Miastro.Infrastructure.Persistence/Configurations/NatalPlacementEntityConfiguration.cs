using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Miastro.Infrastructure.Persistence.Entities;

namespace Miastro.Infrastructure.Persistence.Configurations;

public sealed class NatalPlacementEntityConfiguration
    : IEntityTypeConfiguration<NatalPlacementEntity>
{
    public void Configure(
        EntityTypeBuilder<NatalPlacementEntity> builder)
    {
        builder.ToTable("NatalPlacements");

        builder.HasKey(x => new
        {
            x.ChartId,
            x.ObjectId
        });

        builder.HasIndex(x => x.ObjectId);

        builder.Property(x => x.LongitudeDegrees)
            .IsRequired();

        builder.Property(x => x.ZodiacSign)
            .IsRequired();

        builder.Property(x => x.DegreeInSign)
            .IsRequired();
    }
}
