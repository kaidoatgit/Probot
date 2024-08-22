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
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductOption> ProductOptions { get; set; } = null!;
        public virtual DbSet<LogEntry> LogEntries { get; set; } = null!;
        public virtual DbSet<ProductKey> ProductKeys { get; set; } = null!;
        public virtual DbSet<UserSetting> UserSettings { get; set; } = null!;
        public virtual DbSet<ProRaffle> ProRaffles { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Product>().HasData(
                new Product
                { 
                    Id = 1, 
                    Name = ProductName.ProRaffle, 
                    Description = "A tool which automates the process of registring in alphabot raffles"
                }
            );

            modelBuilder.Entity<ProductOption>().HasData(
                new ProductOption
                {
                    Id = 1, ProductId = 1, Price = 12m, Period = 1, PeriodDescription = "1 Month"
                },
                new ProductOption
                {
                    Id = 2, ProductId = 1, Price = 20m, Period = 2, PeriodDescription = "2 Months" 
                },
                new ProductOption
                {
                    Id = 3, ProductId = 1, Price = 30m, Period = 3, PeriodDescription = "3 Months" 
                }
            );

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

            // Configure unique index for Subscriptions
            // A unique index ensures that the combination of UserId and ProductId values must be unique across all rows in the Subscription table. 
            // In other words, no two Subscription entities can have the same combination of UserId and ProductId.
            // modelBuilder.Entity<Subscription>()
            //  .HasIndex(s => new { s.UserId, s.ProductId })
            //  .IsUnique();

            //modelBuilder.Entity<Order>()
            //   .HasOne(s => s.Subscription)
            //   .WithOne(o => o.Order)
            //   .HasForeignKey<Subscription>(s => s.OrderId)
            //   .IsRequired(false)
            //   .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Product>()
            .Property(p => p.Name)
            .HasConversion(
                pt => pt.ToString(),
                pt => (ProductName)Enum.Parse(typeof(ProductName), pt));

            modelBuilder.Entity<Order>()
            .Property(o => o.Status)
            .HasConversion(
                os => os.ToString(),
                os => (OrderStatus)Enum.Parse(typeof(OrderStatus), os));

            // modelBuilder.Entity<Invoice>()
            // .Property(p => p.ProductName)
            // .HasConversion(
            //     pt => pt.ToString(),
            //     pt => (ProductName)Enum.Parse(typeof(ProductName), pt));

            modelBuilder.Entity<Invoice>()
            .Property(o => o.OrderStatus)
            .HasConversion(
                os => os.ToString(),
                os => (OrderStatus)Enum.Parse(typeof(OrderStatus), os));

             modelBuilder.Entity<InvoiceItem>()
            .Property(p => p.ProductName)
            .HasConversion(
                pt => pt.ToString(),
                pt => (ProductName)Enum.Parse(typeof(ProductName), pt));

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

            
            modelBuilder.Entity<ProductKey>()
                .Property(pk => pk.Version)
                .IsConcurrencyToken();
                
            // Configure TPH Inheritance for UserProductConfiguration
            // modelBuilder.Entity<UserSetting>()
            //     .HasOne(ps => ps.Subscription)
            //     .WithOne(s => s.UserSetting)
            //     .HasForeignKey<UserSetting>(ps => ps.SubscriptionId)
            //     .OnDelete(DeleteBehavior.Cascade);

            // Configure the TPH Discriminator Column
            // modelBuilder.Entity<UserSetting>()
            //     .HasDiscriminator<string>("ConfigurationType")
            //     .HasValue<ProRaffle>("ProRaffle")
            //     .HasValue<ProBidSetting>("ProBidSetting");

            // Configure TPT Inheritance for UserProductConfiguration
            modelBuilder.Entity<UserSetting>()
                .ToTable("UserSettings");

            modelBuilder.Entity<ProRaffle>()
                .ToTable("ProRaffles")
                .HasIndex(prs => prs.Key)
                .IsUnique();
        }
    }
}
