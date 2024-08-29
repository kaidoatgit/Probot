using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProPayments.Service.Data.Entities;

public class ProRaffle : UserSetting
{
    [Column(Order = 1)]
    public string Key { get; set; } = null!;
    [Column(Order = 2)]
    public bool IsPaused { get; set; } = false;
    [Column(Order = 3)]
    [ConcurrencyCheck]
    public Guid Version { get; set; }

    public ProRaffle() : base("ProRaffle")
    {
    }
}