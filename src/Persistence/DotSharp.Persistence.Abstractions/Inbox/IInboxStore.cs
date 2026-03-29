namespace DotSharp.Persistence.Abstractions.Inbox;

/// <summary>
/// Defines the persistence operations for the inbox pattern.
/// Used to ensure idempotent processing of incoming messages.
/// </summary>
public interface IInboxStore
{
    /// <summary>
    /// Attempts to mark a message as processed for a given consumer.
    /// Returns true if the message was marked successfully (first time processing).
    /// Returns false if the message was already processed by this consumer.
    /// </summary>
    /// <param name="messageId">The unique identifier of the message.</param>
    /// <param name="consumer">The consumer identifier.</param>
    /// <param name="nowUtc">The current UTC timestamp.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<bool> TryMarkProcessedAsync(
        Guid messageId,
        string consumer,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default);
}
