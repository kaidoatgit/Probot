using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Probot.Data.Entities;

namespace Probot.Data.Configurations;

public class ProRaffleConfiguration : IEntityTypeConfiguration<ProRaffle>
{
    public void Configure(EntityTypeBuilder<ProRaffle> builder)
    {
        builder.ToTable("ProRaffles")
            .HasBaseType<UserSetting>()
            .Property(pr => pr.Version)  // This assumes "Version" is inherited from UserSetting
            .IsConcurrencyToken();      // Mark the Version property as the concurrency token

        builder.HasIndex(pr => pr.Key)  // Create a unique index on the Key property
            .IsUnique();

    }
}
