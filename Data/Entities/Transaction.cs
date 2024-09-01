using Probot.Shared.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Probot.Data.Entities;
public class Transaction
{
    [Column(Order = 0)]
    public ulong Id { get; set; }
    [Column(Order = 1)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Column(Order = 2)]
    public string? Hash { get; set; }
    [Column(Order = 3)]
    public decimal TotalAmount { get; set; }
    [Column(Order = 4)]
    public Token Token { get; set; }
    [Column(Order = 5)]
    public DateTime? PaymentDate { get; set; }
    [Column(Order = 6)]
    public string? PaymentAddress { get; set; }
    [Column(Order = 7)]
    public string RecipientAddress { get; set; } = string.Empty;

    [Column(Order = 8)]
    public ulong OrderId { get; set; }
    public Order Order { get; set; } = null!; //Navigation purpose

    internal void Close(ProbotContext context, string hash)
    {
        Hash = hash;
        PaymentDate = DateTime.UtcNow;
        context.Entry(this).Property(t => t.Hash).IsModified = true;
        context.Entry(this).Property(t => t.PaymentDate).IsModified = true;
    }

    //internal async Task Close(SubscriptionContext context, string hash, CancellationToken stoppingToken)
    // {
    //     await context.Transactions
    //         .Where(t => t.Id == Id)
    //         .ExecuteUpdateAsync(u => u
    //             .SetProperty(t => t.Hash, hash)
    //         stoppingToken);
    // }
}
