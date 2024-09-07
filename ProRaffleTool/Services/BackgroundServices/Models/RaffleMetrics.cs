using System;

namespace Probot.ProRaffleTool.Services.BackgroundServices.Models;

public class RaffleMetrics
{
    public int TwitterRafflesCount { get; private set; }
    public int CommunityRafflesCount { get; private set; }
    public int TotalRafflesCount 
    { 
        get => TwitterRafflesCount + CommunityRafflesCount; 
    }
    public int TotalRegistered { get; set; }

    public void UpdateCount(int twitterRafflesCount, int communityRafflesCount)
    {
        TotalRegistered = 0;
        TwitterRafflesCount = twitterRafflesCount;
        CommunityRafflesCount = communityRafflesCount;
    }
}
