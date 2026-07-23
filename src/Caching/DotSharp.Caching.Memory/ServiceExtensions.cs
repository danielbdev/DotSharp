using DotSharp.Caching.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace DotSharp.Caching.Memory;

/// <summary>
/// Extension methods for registering DotSharp in-memory caching services.
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Registers <see cref="MemoryCacheService{T}"/> as the implementation of <see cref="ICacheService{T}"/>
    /// and ensures <see cref="IMemoryCache"/> is available via the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static IServiceCollection AddDotSharpCachingMemory(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddScoped(typeof(ICacheService<>), typeof(MemoryCacheService<>));
        return services;
    }
}
