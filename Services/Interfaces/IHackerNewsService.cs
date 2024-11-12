namespace HackerNews.Services.Interfaces;

public interface IHackerNewsService
{
    Task<IEnumerable<string>?> GetBestStoriesAsync(int count, CancellationToken cancellationToken = default);
}