using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Probot.Data.Entities;

namespace Probot.Data.Configurations;

public class ProductSettingConfiguration : IEntityTypeConfiguration<ProductSetting>
{
    public void Configure(EntityTypeBuilder<ProductSetting> builder)
    {
        // Configure TPT Inheritance for ProductSetting
        builder.ToTable("ProductSettings")
            .HasIndex(pr => pr.Username)
            .IsUnique();
    }
}
