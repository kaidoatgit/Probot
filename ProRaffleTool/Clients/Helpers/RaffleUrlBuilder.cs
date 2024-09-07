
using Microsoft.AspNetCore.WebUtilities;
using Probot.ProRaffleTool.Models.Enums;

namespace Probot.ProRaffleTool.Clients.Helpers;

public static class RaffleUrlBuilder
{
    private static readonly Dictionary<RaffleType, string> RaffleUrls = new()
    {
        { RaffleType.TWITTER_RAFFLES, BuildTwitterRafflesUrl() },
        { RaffleType.COMMUNITY_RAFFLES, BuildCommunityRafflesUrl() }
    };

    public static string GetUrl(RaffleType raffleType)
    {
        if (RaffleUrls.TryGetValue(raffleType, out var url))
        {
            return url;
        }
        throw new ArgumentException("Invalid raffle type", nameof(raffleType));
    }

    private static string BuildTwitterRafflesUrl()
    {
        var baseParams = new Dictionary<string, string?>
        {
            { "scope", "all" },
            { "sort", "ending" },
            { "sortDir", "1" },
            { "filter", "unregistered" },
            { "status", "active" },
            { "pageSize", "45" }
        };
        string path = "raffles?req=f&req=l&req=t";
        return QueryHelpers.AddQueryString(path, baseParams);
    }

    private static string BuildCommunityRafflesUrl()
    {
        var baseParams = new Dictionary<string, string?>
        {
            { "scope", "community" },
            { "sort", "ending" },
            { "sortDir", "1" },
            { "filter", "unregistered" },
            { "status", "active" },
            { "pageSize", "45" }
        };
        string path = "raffles";
        return QueryHelpers.AddQueryString(path, baseParams);
    }
}
