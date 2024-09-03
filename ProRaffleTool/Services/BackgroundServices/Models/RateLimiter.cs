using System;

namespace Probot.ProRaffleTool.Services.BackgroundServices.Models;

public class RateLimiter
{
    private readonly SemaphoreSlim _semaphore = new(2, 2);
    private readonly TimeSpan _delay = TimeSpan.FromSeconds(10);

    public async Task WaitAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            await Task.Delay(_delay);
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
