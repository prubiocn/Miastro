using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Miastro.Infrastructure.Persistence.Entities;

namespace Miastro.Infrastructure.Persistence.Configurations;

public sealed class PersonHistoryEntityConfiguration
    : IEntityTypeConfiguration<PersonHistoryEntity>
{
    public void Configure(
        EntityTypeBuilder<PersonHistoryEntity> builder)
    {
        builder.ToTable("PersonHistory");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Summary)
            .IsRequired()
            .HasMaxLength(240);

        builder.HasIndex(x => new
        {
            x.PersonId,
            x.OccurredAtUtc
        });
    }
}
