
namespace HackerNews.Models;

public record ApiOptions
{
    public required string BaseAddress { get; init; }

    public int RequestsPerSecond { get; init; } = -1;

    public int RetryAttempts { get; init; } = 5;
}