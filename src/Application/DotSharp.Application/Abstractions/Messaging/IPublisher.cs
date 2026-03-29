namespace DotSharp.Application.Abstractions.Messaging;

/// <summary>
/// Publishes notifications to one or more handlers through the pipeline.
/// </summary>
public interface IPublisher
{
    /// <summary>
    /// Publishes a notification to all registered handlers.
    /// </summary>
    /// <param name="notification">The notification to publish.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task Publish(INotification notification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes multiple notifications to all registered handlers.
    /// </summary>
    /// <param name="notifications">The notifications to publish.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task PublishMany(IEnumerable<INotification> notifications, CancellationToken cancellationToken = default);
}
