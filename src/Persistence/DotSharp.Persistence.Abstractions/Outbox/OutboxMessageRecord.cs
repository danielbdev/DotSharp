namespace DotSharp.Persistence.Abstractions.Outbox;

/// <summary>
/// Provider-agnostic representation of an outbox message returned by the store.
/// </summary>
/// <param name="Id">The unique identifier of the message.</param>
/// <param name="OccurredOnUtc">The timestamp when the event occurred.</param>
/// <param name="EventName">The name of the event type.</param>
/// <param name="EventVersion">The version of the event type.</param>
/// <param name="Payload">The serialized event payload.</param>
/// <param name="CorrelationId">The optional correlation identifier.</param>
/// <param name="AttemptCount">The number of processing attempts.</param>
public sealed record OutboxMessageRecord(
    Guid Id,
    DateTimeOffset OccurredOnUtc,
    string EventName,
    int EventVersion,
    string Payload,
    string? CorrelationId,
    int AttemptCount);
