namespace DotSharp.Primitives.Events;

/// <summary>
/// Contract for aggregates/entities that manage domain events.
/// </summary>
public interface IHasDomainEvents
{
    /// <summary>
    /// The domain events raised by this entity, exposed as a read-only collection.
    /// </summary>
    IReadOnlyCollection<DomainEvent> DomainEvents { get; }

    /// <summary>
    /// Adds a domain event to the entity's domain event collection.
    /// </summary>
    /// <param name="domainEvent">The domain event to add.</param>
    void AddDomainEvent(DomainEvent domainEvent);

    /// <summary>
    /// Removes a domain event from the entity's domain event collection.
    /// </summary>
    /// <param name="domainEvent">The domain event to remove.</param>
    void RemoveDomainEvent(DomainEvent domainEvent);

    /// <summary>
    /// Clears all domain events from the entity's domain event collection.
    /// </summary>
    void ClearDomainEvents();
}