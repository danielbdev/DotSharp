using Microsoft.Extensions.DependencyInjection;

namespace DotSharp.Caching.Abstractions;

/// <summary>
/// Extension methods for registering DotSharp caching abstraction services.
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Registers the DotSharp caching abstractions.
    /// The generic <see cref="ICacheService{T}"/> is registered as an open generic
    /// placeholder — concrete implementations are registered by the Memory or Redis packages.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static IServiceCollection AddDotSharpCachingAbstractions(this IServiceCollection services)
    {
        // Abstractions do not register ICacheService<> directly — that's delegated
        // to concrete implementations (Memory, Redis). This method exists for
        // discoverability and serves as an extension point for future abstractions.
        return services;
    }
}
