using System.Text.Json;

using HackerNews.Models;
using HackerNews.Services.Interfaces;

namespace HackerNews.Services;

public class CommunicationService(
    ICachingService cachingService,
    HttpClient client,
    IRateLimiterService rateLimiterService)
    : ICommunicationService
{

    private readonly JsonSerializerOptions _JsonSerializerOptions = new() 
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<HashSet<long>?> GetBestStoriesIdsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var resultStream = await client.GetStreamAsync("beststories.json", cancellationToken);
            var ids = await JsonSerializer.DeserializeAsync<List<long>>(resultStream, options: null, cancellationToken);
            return ids?.ToHashSet();
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public string GetItemContent(long id)
    {
        return cachingService.GetValue(id);
    }

    private async Task<string?> GetItemByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        try
        {
            await rateLimiterService.EnsureRateLimit(cancellationToken);

            return await client.GetStringAsync($"item/{id}.json", cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            return null;
        }
    }

    public async Task<List<HackerNewsItemBase>> GetItemsAsync(HashSet<long> ids, CancellationToken cancellationToken = default)
    {
        var stories = new List<HackerNewsItemBase>();
        foreach (var id in ids)
        {
            if (!cachingService.TryGetValue(id, out var itemStr))
            {
                itemStr = await GetItemByIdAsync(id, cancellationToken);
                if (itemStr == null)
                { continue; }
            }

            var item = JsonSerializer.Deserialize<HackerNewsItemBase>(itemStr, _JsonSerializerOptions);
            if (item == null || item.Id != id)
            { continue; }

            stories.Add(item);

            if (cachingService.ContainsKey(id))
            { continue; }

            cachingService.SetValue(id, itemStr);
        }

        return stories;
    }
}