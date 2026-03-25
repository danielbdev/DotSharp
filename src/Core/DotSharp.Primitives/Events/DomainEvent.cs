namespace DotSharp.Primitives.Events;

/// <summary>
/// Base domain event with identifiers and timestamps.
/// Keep dependency-free; if using a mediator, derive events and implement the mediator's notification interface there.
/// </summary>
public abstract class DomainEvent(string? correlationId = null) : IDomainEvent
{

    /// <summary>
    /// Unique event id (useful for idempotency).
    /// </summary>
    public Guid EventId { get; } = Guid.NewGuid();

    /// <summary>
    /// Occurrence timestamp in UTC.
    /// </summary>
    public DateTimeOffset OccurredOnUtc { get; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Optional correlation id for tracing.
    /// </summary>
    public string? CorrelationId { get; } = correlationId;
}