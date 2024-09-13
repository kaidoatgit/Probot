
using Microsoft.AspNetCore.WebUtilities;
using Probot.ProRaffleTool.Models.Enums;

namespace Probot.ProRaffleTool.Clients.Helpers;

public static class RaffleUrlBuilder
{
    private static readonly Dictionary<RaffleType, Func<int?, string>> RaffleUrlBuilders = new()
    {
        { RaffleType.TWITTER_RAFFLES, _ => BuildTwitterRafflesUrl() },
        { RaffleType.COMMUNITY_RAFFLES, pageNum => BuildCommunityRafflesUrl(pageNum) }
    };

    public static string GetUrl(RaffleType raffleType, int? pageNum = null)
    {
        if (RaffleUrlBuilders.TryGetValue(raffleType, out var buildUrl))
        {
            return buildUrl(pageNum);
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
            { "pageSize", "30" }
        };
        string path = "raffles?req=f&req=l&req=t";
        return QueryHelpers.AddQueryString(path, baseParams);
    }

    private static string BuildCommunityRafflesUrl(int? pageNum)
    {
        var baseParams = new Dictionary<string, string?>
        {
            { "scope", "community" },
            { "sort", "ending" },
            { "sortDir", "1" },
            { "filter", "unregistered" },
            { "status", "active" },
            { "pageSize", "40" }
        };

        if (pageNum.HasValue)
        {
            baseParams["pageNum"] = pageNum.Value.ToString();
        }

        string path = "raffles";
        return QueryHelpers.AddQueryString(path, baseParams);
    }
}
