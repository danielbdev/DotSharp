using System.Reflection;
using DotSharp.Application.Abstractions.Behaviors;
using DotSharp.Application.Abstractions.Handlers;
using DotSharp.Application.Abstractions.Messaging;
using DotSharp.Application.Behaviors;
using DotSharp.Application.Messaging;
using DotSharp.Application.Messaging.Notifications;
using DotSharp.Application.Messaging.Requests;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DotSharp.Application;

/// <summary>
/// Extension methods for registering DotSharp application services.
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Registers request/notification buses, pipeline behaviors and optionally scans assemblies for handlers and validators.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assembliesToScan">Assemblies to scan for handlers and validators.</param>
    public static IServiceCollection AddDotSharpApplication(
        this IServiceCollection services,
        params Assembly[] assembliesToScan)
    {
        services.AddScoped<ISender, RequestBus>();
        services.AddScoped<IPublisher, NotificationBus>();
        services.AddScoped<IMessageBus, MessageBus>();

        services.AddScoped(typeof(IRequestPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        if (assembliesToScan is { Length: > 0 })
        {
            services.AddValidatorsFromAssemblies(assembliesToScan, ServiceLifetime.Scoped);

            services.Scan(scan => scan
                .FromAssemblies(assembliesToScan)
                .AddClasses(c => c.AssignableTo(typeof(IRequestHandler<,>)), publicOnly: false)
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
                .AddClasses(c => c.AssignableTo(typeof(INotificationHandler<>)), publicOnly: false)
                    .AsImplementedInterfaces()
                    .WithScopedLifetime());
        }

        return services;
    }
}
