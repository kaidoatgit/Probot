using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Probot.Data.Entities;
using Probot.Shared.Enums;

namespace Probot.Data.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.Property(t => t.Coin)
        .HasConversion(
            t => t.ToString(),
            t => (Coin)Enum.Parse(typeof(Coin), t));

    }
}
