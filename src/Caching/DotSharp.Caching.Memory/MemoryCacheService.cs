using System.Collections.Concurrent;
using DotSharp.Caching.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using AbstractionsPriority = DotSharp.Caching.Abstractions.CacheItemPriority;
using MsCacheItemPriority = Microsoft.Extensions.Caching.Memory.CacheItemPriority;

namespace DotSharp.Caching.Memory;

/// <summary>
/// An in-process cache service implementing <see cref="ICacheService{T}"/> using <see cref="IMemoryCache"/>.
/// Provides stampede protection via per-key <see cref="SemaphoreSlim"/> with double-checked locking.
/// </summary>
/// <typeparam name="T">The type of cached values.</typeparam>
public sealed class MemoryCacheService<T> : ICacheService<T>
{
    private readonly IMemoryCache _cache;
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

    /// <summary>
    /// Initializes a new instance of <see cref="MemoryCacheService{T}"/>.
    /// </summary>
    /// <param name="cache">The underlying memory cache.</param>
    public MemoryCacheService(IMemoryCache cache)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <inheritdoc />
    public ValueTask<T?> GetAsync(string key, CancellationToken ct = default)
    {
        _cache.TryGetValue(key, out T? value);
        return ValueTask.FromResult(value);
    }

    /// <inheritdoc />
    public async ValueTask<T?> GetOrSetAsync(
        string key,
        Func<CancellationToken, ValueTask<T?>> factory,
        CacheOptions? options = null,
        CancellationToken ct = default)
    {
        // Fast path: already cached
        if (_cache.TryGetValue(key, out T? cached))
        {
            return cached;
        }

        var semaphore = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));

        await semaphore.WaitAsync(ct);

        try
        {
            // Double-check: another thread may have populated the cache
            if (_cache.TryGetValue(key, out cached))
            {
                return cached;
            }

            var value = await factory(ct);

            if (value is not null)
            {
                var entryOptions = BuildEntryOptions(options);
                _cache.Set(key, value, entryOptions);
            }

            return value;
        }
        finally
        {
            semaphore.Release();
            CleanupLock(key, semaphore);
        }
    }

    /// <inheritdoc />
    public ValueTask SetAsync(string key, T value, CacheOptions? options = null, CancellationToken ct = default)
    {
        var entryOptions = BuildEntryOptions(options);
        _cache.Set(key, value, entryOptions);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask RemoveAsync(string key, CancellationToken ct = default)
    {
        _cache.Remove(key);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask<bool> ExistsAsync(string key, CancellationToken ct = default)
    {
        return ValueTask.FromResult(_cache.TryGetValue(key, out _));
    }

    private static MemoryCacheEntryOptions BuildEntryOptions(CacheOptions? options)
    {
        var entryOptions = new MemoryCacheEntryOptions();

        if (options is null)
        {
            return entryOptions;
        }

        if (options.AbsoluteExpiration.HasValue)
        {
            entryOptions.SetAbsoluteExpiration(options.AbsoluteExpiration.Value);
        }

        if (options.AbsoluteExpirationRelativeToNow.HasValue)
        {
            entryOptions.SetAbsoluteExpiration(options.AbsoluteExpirationRelativeToNow.Value);
        }

        if (options.SlidingExpiration.HasValue)
        {
            entryOptions.SetSlidingExpiration(options.SlidingExpiration.Value);
        }

        entryOptions.Priority = options.Priority switch
        {
            AbstractionsPriority.Low => MsCacheItemPriority.Low,
            AbstractionsPriority.Normal => MsCacheItemPriority.Normal,
            AbstractionsPriority.High => MsCacheItemPriority.High,
            AbstractionsPriority.NeverRemove => MsCacheItemPriority.NeverRemove,
            _ => MsCacheItemPriority.Normal,
        };

        return entryOptions;
    }

    private void CleanupLock(string key, SemaphoreSlim semaphore)
    {
        // If no one else is waiting, remove the semaphore from the dictionary
        // to prevent unbounded growth of the lock dictionary.
        if (semaphore.CurrentCount > 0)
        {
            _locks.TryRemove(key, out _);
        }
    }
}
