using DotSharp.Application.Abstractions.Messaging;

namespace DotSharp.Application.Messaging;

/// <summary>
/// Default implementation of <see cref="IMessageBus"/> that combines <see cref="ISender"/> and <see cref="IPublisher"/>.
/// </summary>
/// <param name="sender">The sender used to dispatch requests.</param>
/// <param name="publisher">The publisher used to dispatch notifications.</param>
public sealed class MessageBus(ISender sender, IPublisher publisher) : IMessageBus
{
    /// <inheritdoc />
    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        => sender.Send(request, cancellationToken);

    /// <inheritdoc />
    public Task Publish(INotification notification, CancellationToken cancellationToken = default)
        => publisher.Publish(notification, cancellationToken);

    /// <inheritdoc />
    public Task PublishMany(IEnumerable<INotification> notifications, CancellationToken cancellationToken = default)
        => publisher.PublishMany(notifications, cancellationToken);
}
