using System;

namespace Probot.ProRaffleTool.Models;

public class NotificationMessage
{
    public string Description { get; set; } = string.Empty;
    public int Color { get; set; }
    public ulong UserId { get; set; }
    public bool IsMentionable { get; set; }
    public string Username { get; set; } = string.Empty;
}
