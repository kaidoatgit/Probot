using System;

namespace Probot.ProRaffleTool.Models;

public class RateLimiter
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly TimeSpan _defaultDelay  = TimeSpan.FromSeconds(15);
    private volatile bool _apiRequestWaiting = false;

    // public async Task WaitAsync(TimeSpan? delay = null)
    // {
    //     var delayToUse = delay ?? _defaultDelay;
    //     await _semaphore.WaitAsync();
    //     try
    //     {
    //         await Task.Delay(delayToUse);
    //     }
    //     finally
    //     {
    //         _semaphore.Release();
    //     }
    // }

    public async Task WaitAsync(TimeSpan? delay = null)
    {
        var delayToUse = delay ?? _defaultDelay;
        await _semaphore.WaitAsync(delayToUse);
        _semaphore.Release();
    }
}
