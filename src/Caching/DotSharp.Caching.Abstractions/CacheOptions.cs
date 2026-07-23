namespace DotSharp.Caching.Abstractions;

/// <summary>
/// Controls the priority of a cache entry for eviction.
/// </summary>
public enum CacheItemPriority
{
    /// <summary>Low priority — evicted first under memory pressure.</summary>
    Low = 0,

    /// <summary>Normal priority (default).</summary>
    Normal = 1,

    /// <summary>High priority — evicted after normal entries.</summary>
    High = 2,

    /// <summary>Never removed automatically by the cache.</summary>
    NeverRemove = 3,
}

/// <summary>
/// Options controlling cache entry behavior including expiration and eviction priority.
/// </summary>
public sealed record CacheOptions
{
    /// <summary>
    /// An absolute expiration point in time.
    /// </summary>
    public DateTimeOffset? AbsoluteExpiration { get; init; }

    /// <summary>
    /// A relative expiration duration from the time the entry was stored.
    /// </summary>
    public TimeSpan? AbsoluteExpirationRelativeToNow { get; init; }

    /// <summary>
    /// A sliding expiration duration — the entry expires if not accessed within this window.
    /// </summary>
    public TimeSpan? SlidingExpiration { get; init; }

    /// <summary>
    /// The eviction priority. Defaults to <see cref="CacheItemPriority.Normal"/>.
    /// </summary>
    public CacheItemPriority Priority { get; init; } = CacheItemPriority.Normal;
}
