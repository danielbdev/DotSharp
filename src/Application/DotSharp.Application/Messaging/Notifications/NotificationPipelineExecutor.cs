using DotSharp.Application.Abstractions.Behaviors;
using DotSharp.Application.Abstractions.Messaging;

namespace DotSharp.Application.Messaging.Notifications;

/// <summary>
/// Executes the notification pipeline by chaining behaviors around a single handler.
/// </summary>
internal sealed class NotificationPipelineExecutor<TNotification>(
    IEnumerable<INotificationPipelineBehavior<TNotification>> behaviors,
    Func<Task> handler,
    TNotification notification)
    where TNotification : INotification
{
    private readonly IReadOnlyList<INotificationPipelineBehavior<TNotification>> _behaviors = [.. behaviors.Reverse()];
    private readonly TNotification _notification = notification;

    /// <summary>
    /// Executes the pipeline.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task Execute(CancellationToken cancellationToken)
    {
        Func<Task> next = handler;

        foreach (INotificationPipelineBehavior<TNotification> behavior in _behaviors)
        {
            Func<Task> current = next;
            TNotification notification = _notification;
            next = () => behavior.Handle(notification, current, cancellationToken);
        }

        return next();
    }
}
