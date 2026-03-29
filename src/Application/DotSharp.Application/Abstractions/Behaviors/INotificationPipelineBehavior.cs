using DotSharp.Application.Abstractions.Messaging;

namespace DotSharp.Application.Abstractions.Behaviors;

/// <summary>
/// Defines a behavior in the notification pipeline.
/// Behaviors are executed in order around each notification handler.
/// </summary>
/// <typeparam name="TNotification">The type of the notification.</typeparam>
public interface INotificationPipelineBehavior<TNotification> where TNotification : INotification
{
    /// <summary>
    /// Handles the notification by executing the behavior and calling the next delegate.
    /// </summary>
    /// <param name="notification">The notification.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task Handle(TNotification notification, Func<Task> next, CancellationToken cancellationToken);
}
