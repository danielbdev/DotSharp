using DotSharp.Caching.Abstractions;
using Microsoft.Extensions.Caching.Distributed;

namespace DotSharp.Caching.Redis;

/// <summary>
/// A distributed cache service implementing <see cref="ICacheService{T}"/> using <see cref="IDistributedCache"/>.
/// Values are serialized via <see cref="ISerializer"/> before storage and deserialized on retrieval.
/// </summary>
/// <remarks>
/// <para><b>Distributed nature — no built-in stampede protection.</b></para>
/// <para>
/// Unlike <c>MemoryCacheService</c>, this implementation does NOT provide per-key locking or stampede protection.
/// Concurrent misses for the same key across multiple distributed nodes MAY result in multiple factory invocations.
/// Consumers requiring stampede protection for distributed scenarios MUST implement external distributed locking.
/// </para>
/// <para>
/// <c>SlidingExpiration</c> is not natively supported by Redis but is mapped to
/// <see cref="DistributedCacheEntryOptions.SlidingExpiration"/> and emulated by the library.
/// </para>
/// </remarks>
/// <typeparam name="T">The type of cached values.</typeparam>
public sealed class RedisCacheService<T> : ICacheService<T>
{
    private readonly IDistributedCache _cache;
    private readonly ISerializer _serializer;

    /// <summary>
    /// Initializes a new instance of <see cref="RedisCacheService{T}"/>.
    /// </summary>
    /// <param name="cache">The underlying distributed cache (e.g., Redis).</param>
    /// <param name="serializer">The serializer for converting values to/from byte arrays.</param>
    public RedisCacheService(IDistributedCache cache, ISerializer serializer)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
    }

    /// <inheritdoc />
    public async ValueTask<T?> GetAsync(string key, CancellationToken ct = default)
    {
        var bytes = await _cache.GetAsync(key, ct);
        return _serializer.Deserialize<T>(bytes);
    }

    /// <inheritdoc />
    public async ValueTask<T?> GetOrSetAsync(
        string key,
        Func<CancellationToken, ValueTask<T?>> factory,
        CacheOptions? options = null,
        CancellationToken ct = default)
    {
        var bytes = await _cache.GetAsync(key, ct);

        if (bytes is not null)
        {
            return _serializer.Deserialize<T>(bytes);
        }

        var value = await factory(ct);

        if (value is not null)
        {
            await SetAsync(key, value, options, ct);
        }

        return value;
    }

    /// <inheritdoc />
    public async ValueTask SetAsync(string key, T value, CacheOptions? options = null, CancellationToken ct = default)
    {
        var bytes = _serializer.Serialize(value);
        var entryOptions = BuildDistributedOptions(options);
        await _cache.SetAsync(key, bytes, entryOptions, ct);
    }

    /// <inheritdoc />
    public async ValueTask RemoveAsync(string key, CancellationToken ct = default)
    {
        await _cache.RemoveAsync(key, ct);
    }

    /// <inheritdoc />
    public async ValueTask<bool> ExistsAsync(string key, CancellationToken ct = default)
    {
        var value = await GetAsync(key, ct);
        return value is not null;
    }

    private static DistributedCacheEntryOptions BuildDistributedOptions(CacheOptions? options)
    {
        var entryOptions = new DistributedCacheEntryOptions();

        if (options is null)
        {
            return entryOptions;
        }

        if (options.AbsoluteExpiration.HasValue)
        {
            entryOptions.AbsoluteExpiration = options.AbsoluteExpiration.Value;
        }

        if (options.AbsoluteExpirationRelativeToNow.HasValue)
        {
            entryOptions.AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow.Value;
        }

        if (options.SlidingExpiration.HasValue)
        {
            entryOptions.SlidingExpiration = options.SlidingExpiration.Value;
        }

        return entryOptions;
    }
}
