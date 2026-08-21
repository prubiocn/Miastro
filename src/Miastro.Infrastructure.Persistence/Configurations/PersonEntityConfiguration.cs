using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Miastro.Infrastructure.Persistence.Entities;

namespace Miastro.Infrastructure.Persistence.Configurations;

public sealed class PersonEntityConfiguration
    : IEntityTypeConfiguration<PersonEntity>
{
    public void Configure(
        EntityTypeBuilder<PersonEntity> builder)
    {
        builder.ToTable("People");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(x => x.NormalizedName)
            .IsRequired()
            .HasMaxLength(260);

        builder.Property(x => x.Phone)
            .HasMaxLength(64);

        builder.Property(x => x.Email)
            .HasMaxLength(254);

        builder.Property(x => x.PrivateNote)
            .HasMaxLength(10000);

        builder.HasIndex(x => x.FirstName);
        builder.HasIndex(x => x.LastName);
        builder.HasIndex(x => x.NormalizedName);
        builder.HasIndex(x => x.IsFavorite);
        builder.HasIndex(x => x.LastConsultationAtUtc);

        builder.HasOne(x => x.BirthData)
            .WithOne(x => x.Person)
            .HasForeignKey<BirthDataEntity>(x => x.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.CurrentResidence)
            .WithOne(x => x.Person)
            .HasForeignKey<CurrentResidenceEntity>(x => x.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.History)
            .WithOne(x => x.Person)
            .HasForeignKey(x => x.PersonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
