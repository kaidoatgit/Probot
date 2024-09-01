using System.ComponentModel.DataAnnotations.Schema;

namespace Probot.Data.Entities;
public class LogEntry
{
    [Column(Order = 0)]
    public ulong Id { get; set; }
    [Column(Order = 1)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Column(Order = 2)]
    public string Action { get; set; } = string.Empty;
    [Column(Order = 3)]
    public string Description { get; set; } = string.Empty;
    [Column(Order = 4)]
    public string UserId { get; set; } = string.Empty;
}

