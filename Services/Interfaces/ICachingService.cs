using System.Diagnostics.CodeAnalysis;

namespace HackerNews.Services.Interfaces;

public interface ICachingService
{
    bool ContainsKey(long id);

    string GetValue(long id);

    bool SetValue(long id, string content);

    bool TryGetValue(long id, [MaybeNullWhen(false)] out string content);
}