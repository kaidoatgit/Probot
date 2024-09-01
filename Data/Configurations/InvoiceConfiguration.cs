using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Probot.Data.Entities;
using Probot.Shared.Enums;

namespace Probot.Data.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.Property(i => i.OrderStatus)
            .HasConversion(
                os => os.ToString(),
                os => (OrderStatus)Enum.Parse(typeof(OrderStatus), os));

        builder.Property(i => i.Token)
            .HasConversion(
                t => t.ToString(),
                t => (Token)Enum.Parse(typeof(Token), t));

    }
}
