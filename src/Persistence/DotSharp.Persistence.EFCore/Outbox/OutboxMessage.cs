namespace DotSharp.Persistence.EFCore.Outbox;

/// <summary>
/// Represents a persisted outbox message waiting to be processed.
/// </summary>
public sealed class OutboxMessage
{
    /// <summary>
    /// The unique identifier of the message.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The UTC timestamp when the event occurred.
    /// </summary>
    public DateTimeOffset OccurredOnUtc { get; set; }

    /// <summary>
    /// The optional correlation identifier.
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// The stable name of the event type.
    /// </summary>
    public string EventName { get; set; } = default!;

    /// <summary>
    /// The version of the event type.
    /// </summary>
    public int EventVersion { get; set; }

    /// <summary>
    /// The serialized event payload.
    /// </summary>
    public string Payload { get; set; } = default!;

    /// <summary>
    /// The UTC timestamp when the message was successfully processed.
    /// </summary>
    public DateTimeOffset? ProcessedOnUtc { get; set; }

    /// <summary>
    /// The number of processing attempts.
    /// </summary>
    public int AttemptCount { get; set; }

    /// <summary>
    /// The last error message if processing failed.
    /// </summary>
    public string? LastError { get; set; }

    /// <summary>
    /// The identifier of the lock owner during processing.
    /// </summary>
    public string? LockId { get; set; }

    /// <summary>
    /// The UTC timestamp until which the message is locked.
    /// </summary>
    public DateTimeOffset? LockedUntilUtc { get; set; }
}
