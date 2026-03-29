using DotSharp.Application.Abstractions.Behaviors;
using DotSharp.Application.Abstractions.Handlers;
using DotSharp.Application.Abstractions.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DotSharp.Application.Messaging.Notifications;

/// <summary>
/// Default implementation of <see cref="IPublisher"/> that dispatches notifications through the pipeline.
/// </summary>
/// <param name="provider">The service provider used to resolve handlers and behaviors.</param>
public sealed class NotificationBus(IServiceProvider provider) : IPublisher
{
    /// <inheritdoc />
    public Task Publish(INotification notification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(notification);

        MethodInfo method = typeof(NotificationBus).GetMethod(nameof(PublishInternal), BindingFlags.NonPublic | BindingFlags.Instance)!;
        MethodInfo closed = method.MakeGenericMethod(notification.GetType());

        return (Task)closed.Invoke(this, [notification, cancellationToken])!;
    }

    /// <inheritdoc />
    public Task PublishMany(IEnumerable<INotification> notifications, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(notifications);

        return Task.WhenAll(notifications.Select(n => Publish(n, cancellationToken)));
    }

    private Task PublishInternal<TNotification>(TNotification notification, CancellationToken cancellationToken)
        where TNotification : INotification
    {
        INotificationHandler<TNotification>[] handlers = [.. provider.GetServices<INotificationHandler<TNotification>>()];

        if (handlers.Length == 0)
            return Task.CompletedTask;

        IEnumerable<INotificationPipelineBehavior<TNotification>> behaviors = provider.GetServices<INotificationPipelineBehavior<TNotification>>();

        IEnumerable<Task> tasks = handlers.Select(h =>
        {
            NotificationPipelineExecutor<TNotification> executor = new(
                behaviors,
                () => h.Handle(notification, cancellationToken),
                notification);

            return executor.Execute(cancellationToken);
        });

        return Task.WhenAll(tasks);
    }
}
