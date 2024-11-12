using HackerNews.Models;

namespace HackerNews.Services.Interfaces;

public interface ICommunicationService
{
    Task<HashSet<long>?> GetBestStoriesIdsAsync(CancellationToken cancellationToken = default);

    string GetItemContent(long id);

    Task<List<HackerNewsItemBase>> GetItemsAsync(HashSet<long> ids, CancellationToken cancellationToken = default);
}