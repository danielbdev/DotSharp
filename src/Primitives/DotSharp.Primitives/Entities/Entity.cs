using DotSharp.Primitives.Events;

namespace DotSharp.Primitives.Entities;

/// <summary>
/// Abstract class for entities that need to track domain events.
/// </summary>
public abstract class Entity : IEntity
{
    /// <summary>
    /// A list that stores domain events for the entity.
    /// </summary>
    private readonly IList<DomainEvent> _domainEvents = [];

    /// <summary>
    /// Retrieves all domain events for the entity.
    /// </summary>
    /// <returns>A read-only collection of domain events.</returns>
    public IReadOnlyCollection<DomainEvent> GetDomainEvents() => [.. _domainEvents];

    /// <summary>
    /// Adds a new domain event to the entity.
    /// </summary>
    /// <param name="domainEvent">The domain event to be raised.</param>
    public void RaiseDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Removes a domain event from the entity.
    /// </summary>
    /// <param name="domainEvent">The domain event to be removed.</param>
    public void RemoveDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    /// <summary>
    /// Clears all domain events from the entity.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}

/// <summary>
/// Abstract class for entities with a generic identifier <typeparamref name="TKey"/>.
/// </summary>
public abstract class Entity<TKey> : Entity, IEntity<TKey>
{
    /// <summary>
    /// The unique identifier for the entity.
    /// </summary>
    public virtual TKey? Id { get; protected set; }

    /// <summary>
    /// Default constructor for the entity.
    /// </summary>
    protected Entity()
    { }

    /// <summary>
    /// Constructor that sets the identifier of the entity.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    protected Entity(TKey id)
    {
        Id = id;
    }

    /// <summary>
    /// Checks if the entity is transient (i.e., it doesn't have a valid ID yet).
    /// </summary>
    /// <returns>True if the entity is transient, otherwise false.</returns>
    public bool IsTransient()
    {
        return EqualityComparer<TKey>.Default.Equals(Id, default);
    }

    /// <summary>
    /// Compares the current entity with another entity based on their IDs.
    /// </summary>
    /// <param name="obj">The other entity to compare with.</param>
    /// <returns>True if the entities are equal, otherwise false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TKey> other)
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        return Id != null && Id.Equals(other.Id);
    }

    /// <summary>
    /// Equality operator for comparing entities based on their IDs.
    /// </summary>
    /// <param name="left">The left entity.</param>
    /// <param name="right">The right entity.</param>
    /// <returns>True if the entities are equal, otherwise false.</returns>
    public static bool operator ==(Entity<TKey> left, Entity<TKey> right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// Inequality operator for comparing entities based on their IDs.
    /// </summary>
    /// <param name="left">The left entity.</param>
    /// <param name="right">The right entity.</param>
    /// <returns>True if the entities are not equal, otherwise false.</returns>
    public static bool operator !=(Entity<TKey> left, Entity<TKey> right)
    {
        return !Equals(left, right);
    }

    /// <summary>
    /// Returns the hash code of the entity based on its ID.
    /// </summary>
    /// <returns>The hash code of the entity.</returns>
    public override int GetHashCode()
    {
        return Id?.GetHashCode() ?? 0;
    }
}