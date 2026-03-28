using DotSharp.Observability.Tracing;
using Microsoft.Extensions.DependencyInjection;

namespace DotSharp.Observability;

/// <summary>
/// Extension methods for registering DotSharp observability services.
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Registers the core observability services.
    /// Registers <see cref="ITraceContext"/> as a singleton.
    /// For HTTP-based <see cref="Correlation.ICorrelationContext"/> registration, use DotSharp.Web.
    /// </summary>
    public static IServiceCollection AddDotSharpObservability(this IServiceCollection services)
    {
        services.AddSingleton<ITraceContext, TraceContext>();

        return services;
    }
}
