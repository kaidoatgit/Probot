using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Probot.Data.Entities;

namespace Probot.Data.Configurations;

public class ProRaffleSettingConfiguration : IEntityTypeConfiguration<ProRaffleSetting>
{
    public void Configure(EntityTypeBuilder<ProRaffleSetting> builder)
    {
        builder.ToTable("ProRaffleSettings")
            .HasBaseType<ProductSetting>();
            
        builder.HasIndex(pr => pr.Key)
            .IsUnique();

        builder.Property(pr => pr.Version) 
            .IsConcurrencyToken();   
    }
}
