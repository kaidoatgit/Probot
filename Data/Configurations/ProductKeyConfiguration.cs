using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Probot.Data.Entities;

namespace Probot.Data.Configurations;

public class ProductKeyConfiguration : IEntityTypeConfiguration<ProductKey>
{
    public void Configure(EntityTypeBuilder<ProductKey> builder)
    {
        builder.Property(pk => pk.Version)
            .IsConcurrencyToken();

    }
}
