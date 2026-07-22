using DotSharp.Results;

namespace DotSharp.Caching.Abstractions;

/// <summary>
/// Extension methods that bridge <see cref="ICacheService{T}"/> to the <see cref="Result{T}"/> pattern.
/// </summary>
public static class CacheResultExtensions
{
    /// <summary>
    /// Calls <see cref="ICacheService{T}.GetOrSetAsync"/> and wraps the result.
    /// If the factory returns <c>null</c> (cache miss with no value produced), a
    /// <see cref="Result{T}.Failure"/> with <see cref="Errors.NotFound"/> is returned.
    /// Otherwise, <see cref="Result{T}.Success"/> is returned with the cached/produced value.
    /// </summary>
    /// <typeparam name="T">The type of cached values.</typeparam>
    /// <param name="cache">The cache service.</param>
    /// <param name="key">The cache key.</param>
    /// <param name="factory">The factory to invoke on cache miss.</param>
    /// <param name="options">Optional cache entry options.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A <see cref="Result{T}"/> representing success or not-found failure.</returns>
    public static async ValueTask<Result<T>> GetOrSetResultAsync<T>(
        this ICacheService<T> cache,
        string key,
        Func<CancellationToken, ValueTask<T?>> factory,
        CacheOptions? options = null,
        CancellationToken ct = default)
    {
        var value = await cache.GetOrSetAsync(key, factory, options, ct);
        return value is not null
            ? Result<T>.Success(value)
            : Result<T>.Failure(Errors.NotFound($"Cache miss for key '{key}'."));
    }
}
