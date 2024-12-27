namespace DotSharp.Data.Caching.MemoryCache;

/// <summary>
/// Interface for memory cache service operations.
/// Provides methods for getting, creating, clearing, and managing cache entries.
/// </summary>
public interface IMemoryCacheService
{
    /// <summary>
    /// Retrieves an entity from the cache or creates it using the provided function if it does not exist.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to cache.</typeparam>
    /// <param name="cacheKey">The unique key for the cached entity.</param>
    /// <param name="createFunc">A function to create the entity if it is not found in the cache.</param>
    /// <param name="expiration">The duration for which the entity should remain in the cache.</param>
    /// <returns>The cached or newly created entity.</returns>
    Task<TEntity> GetOrCreateAsync<TEntity>(string cacheKey, Func<Task<TEntity>> createFunc, TimeSpan expiration);

    /// <summary>
    /// Generates a unique cache key based on entity type, request properties, and additional parameters.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity associated with the cache key.</typeparam>
    /// <param name="request">The request object containing properties to include in the key.</param>
    /// <param name="parameters">Additional parameters to append to the key.</param>
    /// <returns>A unique cache key.</returns>
    string GenerateCacheKey<TEntity>(object? request, params string[] parameters);

    /// <summary>
    /// Clears all cache entries.
    /// </summary>
    void ClearCache();

    /// <summary>
    /// Clears all cache entries associated with a specific entity type.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity whose cache entries should be cleared.</typeparam>
    void ClearCacheByEntity<TEntity>();
}