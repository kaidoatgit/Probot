using Microsoft.EntityFrameworkCore;
using Probot.Data.Entities;

namespace Probot.Data;
public class ProbotContext : DbContext
{
    public ProbotContext(DbContextOptions<ProbotContext> options) : base(options) { }

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
    public virtual DbSet<ProductSetting> SubscriptionSettings { get; set; } = null!;
    public virtual DbSet<ProRaffleSetting> ProRaffleSettings { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProbotContext).Assembly);

        // Configure unique index for Subscriptions
        // A unique index ensures that the combination of UserId and ProductId values must be unique across all rows in the Subscription table. 
        // In other words, no two Subscription entities can have the same combination of UserId and ProductId.
        // modelBuilder.Entity<Subscription>()
        //  .HasIndex(s => new { s.UserId, s.ProductId })
        //  .IsUnique();    
    }

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     optionsBuilder
    //         .UseSqlite(
    //             "Data Source=subscription.db"
    //             //, providerOptions => { providerOptions }
    //             );
    // }
}