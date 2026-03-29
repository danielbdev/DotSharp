using DotSharp.Application.Abstractions.Messaging;

namespace DotSharp.Application.Abstractions.Handlers;

/// <summary>
/// Handles a notification of type <typeparamref name="TNotification"/>.
/// </summary>
/// <typeparam name="TNotification">The type of the notification.</typeparam>
public interface INotificationHandler<in TNotification> where TNotification : INotification
{
    /// <summary>
    /// Handles the notification.
    /// </summary>
    /// <param name="notification">The notification to handle.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task Handle(TNotification notification, CancellationToken cancellationToken);
}
