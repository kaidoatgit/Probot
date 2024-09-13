using Probot.ProRaffleTool.Models;

namespace Probot.ProRaffleTool.Services.Services.Abstractions;

public interface IRateLimiterService
{
    RateLimiter GetOrAdd(string username);
}
