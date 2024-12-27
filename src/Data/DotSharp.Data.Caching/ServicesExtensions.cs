using DotSharp.Data.Caching.MemoryCache;
using Microsoft.Extensions.DependencyInjection;

namespace DotSharp.Data.Caching;

/// <summary>
/// Extension methods for configuring and registering memory cache services in a .NET application.
/// </summary>
public static class ServicesExtensions
{
    /// <summary>
    /// Registers the memory cache service and its dependencies.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> for method chaining.</returns>
    public static IServiceCollection AddMemoryCacheService(this IServiceCollection services)
    {
        services.AddMemoryCache();

        services.AddScoped<IMemoryCacheService, MemoryCacheService>();

        return services;
    }
}