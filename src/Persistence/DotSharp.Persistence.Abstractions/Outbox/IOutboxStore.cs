namespace DotSharp.Persistence.Abstractions.Outbox;

/// <summary>
/// Defines the persistence operations for the outbox pattern.
/// </summary>
public interface IOutboxStore
{
    /// <summary>
    /// Dequeues a batch of unprocessed messages, locking them for processing.
    /// </summary>
    /// <param name="batchSize">The maximum number of messages to dequeue.</param>
    /// <param name="lockFor">The duration to lock the messages.</param>
    /// <param name="lockId">The identifier of the lock owner.</param>
    /// <param name="nowUtc">The current UTC timestamp.</param>
    /// <param name="maxAttempts">The maximum number of processing attempts allowed.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<IReadOnlyList<OutboxMessageRecord>> DequeueBatchAsync(
        int batchSize,
        TimeSpan lockFor,
        string lockId,
        DateTimeOffset nowUtc,
        int maxAttempts,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a message as successfully processed.
    /// </summary>
    /// <param name="id">The message identifier.</param>
    /// <param name="processedOnUtc">The UTC timestamp when the message was processed.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task MarkProcessedAsync(Guid id, DateTimeOffset processedOnUtc, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a message as failed.
    /// </summary>
    /// <param name="id">The message identifier.</param>
    /// <param name="attemptCount">The current attempt count.</param>
    /// <param name="error">The error message.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task MarkFailedAsync(Guid id, int attemptCount, string error, CancellationToken cancellationToken = default);
}
