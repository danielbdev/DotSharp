using DotSharp.Caching.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

namespace DotSharp.Caching.Redis;

/// <summary>
/// Extension methods for registering DotSharp Redis-backed caching services.
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Registers <see cref="RedisCacheService{T}"/> as the implementation of <see cref="ICacheService{T}"/>
    /// and <see cref="SystemTextJsonSerializer"/> as the default <see cref="ISerializer"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The caller is responsible for registering the underlying <see cref="IDistributedCache"/>
    /// implementation (e.g., via <c>AddStackExchangeRedisCache</c>). This method only registers
    /// the DotSharp caching layer on top of it.
    /// </para>
    /// <para>
    /// <see cref="ISerializer"/> is registered as a Singleton (stateless).
    /// <see cref="ICacheService{T}"/> is registered as Scoped.
    /// </para>
    /// </remarks>
    /// <param name="services">The service collection.</param>
    public static IServiceCollection AddDotSharpCachingRedis(this IServiceCollection services)
    {
        services.AddSingleton<ISerializer, SystemTextJsonSerializer>();
        services.AddScoped(typeof(ICacheService<>), typeof(RedisCacheService<>));
        return services;
    }
}
