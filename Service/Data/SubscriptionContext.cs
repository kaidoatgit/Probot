using Microsoft.EntityFrameworkCore;
using ProPayments.Service.Data.Entities;
using ProPayments.Service.Data.Entities.Enums;

namespace ProPayments.Service.Data
{
    public class SubscriptionContext : DbContext
    {
        public SubscriptionContext(DbContextOptions<SubscriptionContext> options) : base(options) { }

        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderItem> OrderItems { get; set; } = null!;
        public virtual DbSet<Transaction> Transactions { get; set; } = null!;
        public virtual DbSet<Invoice> Invoices { get; set; } = null!;
        public virtual DbSet<InvoiceItem> InvoiceItems { get; set; } = null!;
        public virtual DbSet<Subscription> Subscriptions { get; set; } = null!;
        public virtual DbSet<Plan> Plans { get; set; } = null!;
        public virtual DbSet<PlanOption> PlanOptions { get; set; } = null!;
        public virtual DbSet<LogEntry> LogEntries { get; set; } = null!;
        public virtual DbSet<AccessCode> AccessCodes { get; set; } = null!;
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var freePlanDescription = "Experience the essential features of our service with our Free Plan. Ideal for users who want to explore the basic functionalities at no cost. Enjoy a limited duration access and get a glimpse of the premium benefits without any commitment.";

            var basicPlanDescription = "Discover the perfect balance with our Basic Plan. Tailored for users who need more than the essentials but aren’t ready for all the premium extras. Enjoy extended access to a range of key features, enhanced support, and additional resources. Ideal for those seeking reliable performance and value.";

            var premiumPlanDescription = "Unlock the full potential of our service with our Premium Plan. Perfect for users seeking enhanced features and exclusive benefits. Enjoy uninterrupted access to premium content, priority support, and advanced functionalities. Choose from flexible duration options that suit your needs and elevate your experience.";

            modelBuilder.Entity<Plan>().HasData(
                new Plan
                { 
                    Id = 1, Type = PlanType.Free, Description = freePlanDescription
                },
                new Plan
                {
                    Id = 2, Type = PlanType.Basic, Description = basicPlanDescription
                }
            );

            modelBuilder.Entity<PlanOption>().HasData(
                new PlanOption
                { 
                    Id = 1, PlanId = 1, Price = 0m, Period = 3, PeriodDescription = "3 Days" 
                },
                new PlanOption
                {
                    Id = 2, PlanId = 2, Price = 12m, Period = 1, PeriodDescription = "1 Month"
                },
                new PlanOption
                {
                    Id = 3, PlanId = 2, Price = 20m, Period = 2, PeriodDescription = "2 Months" 
                },
                new PlanOption
                {
                    Id = 4, PlanId = 2, Price = 30m, Period = 3, PeriodDescription = "3 Months" 
                }
            );

            // Configure one-to-one relationship between Order and Invoice
            // By setting DeleteBehavior.SetNull, when deleting a order, the orderId in the related invoice will be set to null
            modelBuilder.Entity<Order>()
                .HasOne(i => i.Invoice)
                .WithOne(o => o.Order)
                .HasForeignKey<Invoice>(i => i.OrderId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure one-to-one relationship between Order and Transaction
            // Deleting a order, will delete the respective transaction
            modelBuilder.Entity<Order>()
                .HasOne(s => s.Transaction)
                .WithOne(t => t.Order)
                .HasForeignKey<Transaction>(t => t.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure unique index for Subscriptions
            // A unique index ensures that the combination of UserId and PlanId values must be unique across all rows in the Subscription table. 
            // In other words, no two Subscription entities can have the same combination of UserId and PlanId.
            // modelBuilder.Entity<Subscription>()
            //  .HasIndex(s => new { s.UserId, s.PlanId })
            //  .IsUnique();

            //modelBuilder.Entity<Order>()
            //   .HasOne(s => s.Subscription)
            //   .WithOne(o => o.Order)
            //   .HasForeignKey<Subscription>(s => s.OrderId)
            //   .IsRequired(false)
            //   .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Plan>()
            .Property(p => p.Type)
            .HasConversion(
                pt => pt.ToString(),
                pt => (PlanType)Enum.Parse(typeof(PlanType), pt));

            modelBuilder.Entity<Order>()
            .Property(o => o.Status)
            .HasConversion(
                os => os.ToString(),
                os => (OrderStatus)Enum.Parse(typeof(OrderStatus), os));

            // modelBuilder.Entity<Invoice>()
            // .Property(p => p.PlanType)
            // .HasConversion(
            //     pt => pt.ToString(),
            //     pt => (PlanType)Enum.Parse(typeof(PlanType), pt));

            modelBuilder.Entity<Invoice>()
            .Property(o => o.OrderStatus)
            .HasConversion(
                os => os.ToString(),
                os => (OrderStatus)Enum.Parse(typeof(OrderStatus), os));

             modelBuilder.Entity<InvoiceItem>()
            .Property(p => p.PlanType)
            .HasConversion(
                pt => pt.ToString(),
                pt => (PlanType)Enum.Parse(typeof(PlanType), pt));

            modelBuilder.Entity<Invoice>()
            .Property(t => t.Token)
            .HasConversion(
                t => t.ToString(),
                t => (Token)Enum.Parse(typeof(Token), t));

            modelBuilder.Entity<Transaction>()
            .Property(t => t.Token)
            .HasConversion(
                t => t.ToString(),
                t => (Token)Enum.Parse(typeof(Token), t));

        }
    }
}
