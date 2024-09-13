using System.Collections.Concurrent;
using Probot.ProRaffleTool.Models;
using Probot.ProRaffleTool.Services.Services.Abstractions;

namespace Probot.ProRaffleTool.Services.Services;

public class RateLimiterService : IRateLimiterService
{
    private ConcurrentDictionary<string, RateLimiter> _rateLimiters { get; } = new();

    public RateLimiter GetOrAdd(string username)
    {
        return _rateLimiters.GetOrAdd(username, _ => new RateLimiter());
    }
}
