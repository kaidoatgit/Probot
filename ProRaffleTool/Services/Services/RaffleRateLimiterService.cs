using System.Collections.Concurrent;
using Probot.ProRaffleTool.Models;
using Probot.ProRaffleTool.Services.Services.Abstractions;

namespace Probot.ProRaffleTool.Services.Services;

public class RaffleRateLimiterService : IRaffleRateLimiterService
{
    private ConcurrentDictionary<string, RaffleRateLimiter> _rateLimiters { get; } = new();

    public RaffleRateLimiter GetOrAdd(string username)
    {
        return _rateLimiters.GetOrAdd(username, _ => new RaffleRateLimiter());
    }
}
