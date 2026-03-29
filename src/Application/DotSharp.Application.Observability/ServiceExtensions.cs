using DotSharp.Application.Abstractions.Behaviors;
using DotSharp.Application.Observability.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace DotSharp.Application.Observability;

/// <summary>
/// Extension methods for registering DotSharp application observability services.
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Registers observability behaviors for requests and notifications.
    /// </summary>
    public static IServiceCollection AddDotSharpApplicationObservability(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRequestPipelineBehavior<,>), typeof(RequestObservabilityBehavior<,>));
        services.AddScoped(typeof(INotificationPipelineBehavior<>), typeof(NotificationObservabilityBehavior<>));

        return services;
    }
}
