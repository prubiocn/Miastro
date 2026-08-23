using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Miastro.Infrastructure.Persistence.Entities;

namespace Miastro.Infrastructure.Persistence.Configurations;

public sealed class NatalHouseCuspEntityConfiguration
    : IEntityTypeConfiguration<NatalHouseCuspEntity>
{
    public void Configure(
        EntityTypeBuilder<NatalHouseCuspEntity> builder)
    {
        builder.ToTable("NatalHouseCusps");

        builder.HasKey(x => new
        {
            x.ChartId,
            x.HouseNumber
        });

        builder.Property(x => x.LongitudeDegrees)
            .IsRequired();
    }
}
