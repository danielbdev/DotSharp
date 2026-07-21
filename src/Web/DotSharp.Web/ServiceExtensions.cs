using DotSharp.Observability;
using DotSharp.Observability.Correlation;
using DotSharp.Web.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DotSharp.Web;

/// <summary>
/// Extension methods for registering DotSharp web services.
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Registers DotSharp web layer services including correlation context,
    /// global exception handling, and validation endpoint filter.
    /// Defensively calls <see cref="Observability.ServiceExtensions.AddDotSharpObservability"/>
    /// if <see cref="DotSharp.Observability.Tracing.ITraceContext"/> is not already registered.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection for fluent chaining.</returns>
    public static IServiceCollection AddDotSharpWeb(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddScoped<ICorrelationContext>(sp =>
        {
            var accessor = sp.GetRequiredService<IHttpContextAccessor>();
            var id = accessor.HttpContext?.Items["X-Correlation-Id"] as string
                     ?? Guid.NewGuid().ToString("N");
            return new CorrelationContext(id);
        });

        services.AddScoped<GlobalExceptionHandler>();
        services.AddTransient<ValidationEndpointFilter>();

        if (services.All(sd => sd.ServiceType != typeof(DotSharp.Observability.Tracing.ITraceContext)))
        {
            services.AddDotSharpObservability();
        }

        return services;
    }
}
