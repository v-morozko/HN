
using HackerNews.Services.Interfaces;

namespace HackerNews.Services;

public class HackerNewsService(ICommunicationService communicationService) : IHackerNewsService
{
    public async Task<IEnumerable<string>?> GetBestStoriesAsync(int count, CancellationToken cancellationToken = default)
    {
        var ids = await communicationService.GetBestStoriesIdsAsync(cancellationToken);
        if (ids is null)
        { return null; }

        var items = await communicationService.GetItemsAsync(ids, cancellationToken);
        return items.Where(
                item => !string.IsNullOrWhiteSpace(item.Url)
                        && string.Equals(item.Type, "story", StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(i => i.Score)
            .Take(count)
            .Select(storyItem => communicationService.GetItemContent(storyItem.Id));
    }
}