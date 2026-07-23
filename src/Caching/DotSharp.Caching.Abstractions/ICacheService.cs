namespace DotSharp.Caching.Abstractions;

/// <summary>
/// Defines a generic cache service providing atomic get-or-set semantics.
/// Cache misses return <c>null</c> rather than throwing or returning a <c>Result</c>.
/// Use <c>CacheResultExtensions.GetOrSetResultAsync</c> for a Result-based bridge.
/// </summary>
/// <typeparam name="T">The type of cached values.</typeparam>
public interface ICacheService<T>
{
    /// <summary>
    /// Retrieves a cached value by key. Returns <c>null</c> if the key does not exist.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The cached value, or <c>null</c> on cache miss.</returns>
    ValueTask<T?> GetAsync(string key, CancellationToken ct = default);

    /// <summary>
    /// Retrieves a cached value by key, invoking the <paramref name="factory"/> to produce
    /// and store a value on cache miss. Returns the cached or newly produced value.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <param name="factory">The factory to invoke on a cache miss.</param>
    /// <param name="options">Optional cache entry options (expiration, priority).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The cached or factory-produced value.</returns>
    ValueTask<T?> GetOrSetAsync(
        string key,
        Func<CancellationToken, ValueTask<T?>> factory,
        CacheOptions? options = null,
        CancellationToken ct = default);

    /// <summary>
    /// Stores a value in the cache under the given key.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <param name="value">The value to store.</param>
    /// <param name="options">Optional cache entry options (expiration, priority).</param>
    /// <param name="ct">Cancellation token.</param>
    ValueTask SetAsync(string key, T value, CacheOptions? options = null, CancellationToken ct = default);

    /// <summary>
    /// Removes the cache entry for the given key. No-op if the key does not exist.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <param name="ct">Cancellation token.</param>
    ValueTask RemoveAsync(string key, CancellationToken ct = default);

    /// <summary>
    /// Checks whether a cache entry exists for the given key.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns><c>true</c> if the key exists; otherwise <c>false</c>.</returns>
    ValueTask<bool> ExistsAsync(string key, CancellationToken ct = default);
}

/// <summary>
/// Non-generic cache service marker for dynamic scenarios where the value type is not known at compile time.
/// Inherits all methods from <see cref="ICacheService{Object}"/>.
/// </summary>
public interface ICacheService : ICacheService<object>
{
}
