namespace DotSharp.Persistence.EFCore.Inbox;

/// <summary>
/// Represents a processed inbox message for a specific consumer.
/// Used to ensure idempotent processing of incoming messages.
/// </summary>
public sealed class InboxConsumer
{
    /// <summary>
    /// The unique identifier of the message.
    /// </summary>
    public Guid MessageId { get; set; }

    /// <summary>
    /// The consumer identifier.
    /// </summary>
    public string Consumer { get; set; } = string.Empty;

    /// <summary>
    /// The UTC timestamp when the message was processed.
    /// </summary>
    public DateTimeOffset ProcessedOnUtc { get; set; }
}
