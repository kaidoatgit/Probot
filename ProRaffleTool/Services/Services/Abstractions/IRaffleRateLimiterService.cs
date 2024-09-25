using Probot.ProRaffleTool.Models;

namespace Probot.ProRaffleTool.Services.Services.Abstractions;

public interface IRaffleRateLimiterService
{
    RaffleRateLimiter GetOrAdd(string username);
}
