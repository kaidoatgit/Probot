using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Probot.Data.Entities;
using Probot.Shared.Enums;

namespace Probot.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.Property(o => o.Status)
        .HasConversion(
            os => os.ToString(),
            os => (OrderStatus)Enum.Parse(typeof(OrderStatus), os));

        // Configure one-to-one relationship between Order and Invoice
        // By setting DeleteBehavior.SetNull, when deleting a order, the orderId in the related invoice will be set to null
        // modelBuilder.Entity<Order>()
        //     .HasOne(i => i.Invoice)
        //     .WithOne(o => o.Order)
        //     .HasForeignKey<Invoice>(i => i.OrderId)
        //     .OnDelete(DeleteBehavior.SetNull);

        // Configure one-to-one relationship between Order and Transaction
        // Deleting a order, will delete the respective transaction
        // modelBuilder.Entity<Order>()
        //     .HasOne(s => s.Transaction)
        //     .WithOne(t => t.Order)
        //     .HasForeignKey<Transaction>(t => t.OrderId)
        //     .OnDelete(DeleteBehavior.Cascade);
    }
}
