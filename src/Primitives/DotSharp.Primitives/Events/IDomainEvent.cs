namespace DotSharp.Primitives.Events;

/// <summary>
/// Interface for entities that can raise and track domain events.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Retrieves all domain events associated with the entity.
    /// </summary>
    /// <returns>A read-only collection of domain events.</returns>
    IReadOnlyCollection<DomainEvent> GetDomainEvents();

    /// <summary>
    /// Adds a new domain event to the entity.
    /// </summary>
    /// <param name="domainEvent">The domain event to be raised.</param>
    void RaiseDomainEvent(DomainEvent domainEvent);

    /// <summary>
    /// Removes a specific domain event from the entity.
    /// </summary>
    /// <param name="domainEvent">The domain event to be removed.</param>
    void RemoveDomainEvent(DomainEvent domainEvent);

    /// <summary>
    /// Clears all domain events from the entity.
    /// </summary>
    void ClearDomainEvents();
}