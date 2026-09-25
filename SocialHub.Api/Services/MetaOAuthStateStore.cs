using System.Collections.Concurrent;
using System.Security.Cryptography;
using SocialHub.Api.Interfaces;

namespace SocialHub.Api.Services;

public sealed class MetaOAuthStateStore : IMetaOAuthStateStore
{
    private static readonly TimeSpan Lifetime = TimeSpan.FromMinutes(10);
    private readonly ConcurrentDictionary<string, Entry> _entries = new(StringComparer.Ordinal);

    public string Create(int workspaceId)
    {
        var now = DateTimeOffset.UtcNow;
        foreach (var expired in _entries.Where(pair => pair.Value.ExpiresAt <= now).Select(pair => pair.Key).ToArray())
            _entries.TryRemove(expired, out _);

        var state = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .TrimEnd('=').Replace('+', '-').Replace('/', '_');
        _entries[state] = new Entry(workspaceId, now.Add(Lifetime));
        return state;
    }

    public bool TryConsume(string state, out int workspaceId)
    {
        workspaceId = 0;
        if (string.IsNullOrWhiteSpace(state) || !_entries.TryRemove(state, out var entry) || entry.ExpiresAt <= DateTimeOffset.UtcNow)
            return false;

        workspaceId = entry.WorkspaceId;
        return true;
    }

    private sealed record Entry(int WorkspaceId, DateTimeOffset ExpiresAt);
}
