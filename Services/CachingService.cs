
using System.Diagnostics.CodeAnalysis;

using HackerNews.Services.Interfaces;

namespace HackerNews.Services;

public class CachingService() : ICachingService
{
    private static readonly Dictionary<long, string> RawItemsCache = new();

    private static readonly object SetValueLock = new();


    public bool ContainsKey(long id)
    {
        return RawItemsCache.ContainsKey(id);
    }

    public string GetValue(long id)
    {
        return RawItemsCache[id];
    }

    public bool TryGetValue(long id, [MaybeNullWhen(false)] out string content)
    {
        return RawItemsCache.TryGetValue(id, out content);
    }

    public bool SetValue(long id, string content)
    {
        lock (SetValueLock)
        {
            return RawItemsCache.TryAdd(id, content);
        }
    }
}