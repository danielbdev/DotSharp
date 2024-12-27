using Microsoft.Extensions.Caching.Memory;
using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text;

namespace DotSharp.Data.Caching.MemoryCache;

/// <summary>
/// Implementation of <see cref="IMemoryCacheService"/> using <see cref="IMemoryCache"/>.
/// Provides thread-safe management of cached entities.
/// </summary>
public class MemoryCacheService(IMemoryCache memoryCache) : IMemoryCacheService
{
    private readonly ConcurrentDictionary<string, HashSet<string>> _entityKeys = new();

    /// <inheritdoc />
    public async Task<TEntity> GetOrCreateAsync<TEntity>(string cacheKey, Func<Task<TEntity>> createFunc, TimeSpan expiration)
    {
        if (memoryCache.TryGetValue(cacheKey, out TEntity cachedValue))
        {
            return cachedValue;
        }

        TEntity createdValue = await createFunc();

        if (createdValue is not null)
        {
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(expiration);

            memoryCache.Set(cacheKey, createdValue, cacheEntryOptions);
        }

        return createdValue;
    }

    /// <inheritdoc />
    public string GenerateCacheKey<TEntity>(object? request, params string[] parameters)
    {
        StringBuilder keyBuilder = new();

        keyBuilder.Append(typeof(TEntity).Name).Append(':');

        if (parameters != null && parameters.Length > 0)
        {
            keyBuilder.Append(string.Join(":", parameters)).Append(':');
        }

        if (request is null)
        {
            return keyBuilder.ToString().TrimEnd(':');
        }

        Type type = request.GetType();

        PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (PropertyInfo property in properties)
        {
            string propertyName = property.Name;

            object? propertyValue = property.GetValue(request);

            keyBuilder.Append(propertyName).Append('=');

            if (propertyValue is IEnumerable enumerable)
            {
                keyBuilder.Append(string.Join(",", enumerable.Cast<object>()));
            }
            else
            {
                keyBuilder.Append(propertyValue ?? "null");
            }

            keyBuilder.Append(':');
        }

        string? cacheKey = keyBuilder.ToString().TrimEnd(':');

        AddCacheKey<TEntity>(cacheKey);

        return cacheKey;
    }

    /// <inheritdoc />
    public void ClearCache()
    {
        foreach (string key in _entityKeys.Values.SelectMany(k => k))
        {
            memoryCache.Remove(key);
        }
        _entityKeys.Clear();
    }

    /// <inheritdoc />
    public void ClearCacheByEntity<TEntity>()
    {
        string entityName = typeof(TEntity).Name;

        if (_entityKeys.TryRemove(entityName, out var keys))
        {
            foreach (var key in keys)
            {
                memoryCache.Remove(key);
            }
        }
    }

    /// <summary>
    /// Adds a cache key to the internal tracking dictionary for the specified entity type.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity associated with the cache key.</typeparam>
    /// <param name="cacheKey">The cache key to add.</param>
    private void AddCacheKey<TEntity>(string cacheKey)
    {
        string entityName = typeof(TEntity).Name;

        HashSet<string>? keys = _entityKeys.GetOrAdd(entityName, _ => []);
        lock (keys)
        {
            keys.Add(cacheKey);
        }
    }
}