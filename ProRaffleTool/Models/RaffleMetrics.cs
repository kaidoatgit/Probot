
namespace Probot.ProRaffleTool.Models;

public class RaffleMetrics
{
    public int TwitterRafflesCount { get; set; }
    public int CommunityRafflesCount { get; set; }
    public int TotalRafflesCount 
    { 
        get => TwitterRafflesCount + CommunityRafflesCount; 
    }
    public int TotalRegistered { get; set; }
}
