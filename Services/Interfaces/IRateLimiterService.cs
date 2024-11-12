namespace HackerNews.Services.Interfaces;

public interface IRateLimiterService
{
    Task EnsureRateLimit(CancellationToken cancellationToken = default);
}