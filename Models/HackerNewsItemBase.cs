
namespace HackerNews.Models;

public record HackerNewsItemBase
{
    public required long Id { get; init; }

    public int Score { get; init; }
    public string Type { get; init; }
    public string Url { get; init; }
}