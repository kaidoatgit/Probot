using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ProPayments.Service.Data.Entities;

public class AccessCode
{
    [Key]
    [Column(Order = 0)]
    public string Code { get; set; } = Guid.NewGuid().ToString();
    [Column(Order = 1)]
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    [Column(Order = 2)]
    public DateTime EndDate { get; set; }
    [Column(Order = 3)]
    public bool IsRedeemed { get; set; } = false;

    // // Optional: To tie back to a specific order or user
    // public ulong? OrderId { get; set; }
    // public Order? Order { get; set; } // Navigation purpose

    // public ulong? UserId { get; set; }
    // public User? User { get; set; } // Navigation purpose
}

