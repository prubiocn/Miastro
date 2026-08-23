using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Miastro.Infrastructure.Persistence.Entities;

namespace Miastro.Infrastructure.Persistence.Configurations;

public sealed class NatalAspectEntityConfiguration
    : IEntityTypeConfiguration<NatalAspectEntity>
{
    public void Configure(
        EntityTypeBuilder<NatalAspectEntity> builder)
    {
        builder.ToTable("NatalAspects");

        builder.HasKey(x => new
        {
            x.ChartId,
            x.FirstObject,
            x.SecondObject
        });

        builder.HasIndex(x => x.Kind);
    }
}
