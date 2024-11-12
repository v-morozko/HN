
using System.Diagnostics;

using HackerNews.Models;
using HackerNews.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace HackerNews.Services;

public class RateLimiterService(TimeProvider timeProvider, IOptions<ApiOptions> apiOptions) : IRateLimiterService
{
    private readonly ApiOptions _ApiOptions = apiOptions.Value;

    private static long _RequestMadeLastTime = -1;

    private static readonly SemaphoreSlim SemaphoreSlim = new (1, 1);


    public async Task EnsureRateLimit(CancellationToken cancellationToken = default)
    {
        if (_ApiOptions.RequestsPerSecond <= 0)
        {
            return;
        }

        await SemaphoreSlim.WaitAsync(cancellationToken);
        try
        {
            if (_RequestMadeLastTime == -1)
            {
#pragma warning disable S2696 //intentional usage for multiple threads "synchronization"
                _RequestMadeLastTime = timeProvider.GetTimestamp();
#pragma warning restore S2696
                return;
            }

            while (!cancellationToken.IsCancellationRequested
                   && timeProvider.GetElapsedTime(_RequestMadeLastTime).TotalMilliseconds < 1000 / (double)_ApiOptions.RequestsPerSecond)
            {
                await Task.Delay(1, cancellationToken);
            }

            Debug.WriteLine($"Elapsed: {timeProvider.GetElapsedTime(_RequestMadeLastTime).TotalMilliseconds}");

            _RequestMadeLastTime = timeProvider.GetTimestamp();
        }
        finally
        {
            SemaphoreSlim.Release();
        }
    }
}