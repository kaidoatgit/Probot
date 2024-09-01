using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Probot.Data.Entities;

namespace Probot.Data.Configurations;

public class UserSettingConfiguration : IEntityTypeConfiguration<UserSetting>
{
    public void Configure(EntityTypeBuilder<UserSetting> builder)
    {
        // Configure TPT Inheritance for UserSetting
        builder.ToTable("UserSettings");
            // .Property(us => us.Version)
            // .IsConcurrencyToken();
    }
}
