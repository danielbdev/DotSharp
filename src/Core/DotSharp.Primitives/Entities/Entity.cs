using System.ComponentModel.DataAnnotations.Schema;
using DotSharp.Primitives.Events;

namespace DotSharp.Primitives.Entities;

/// <summary>
/// Base entity type that provides domain event support.
/// Intended for entities without a single primary key, such as join entities in many-to-many relationships.
/// For entities with a primary key, use <see cref="Entity{TKey}"/> instead.
/// </summary>
public abstract class Entity : IEntity, IHasDomainEvents
{
    /// <summary>
    /// A list that stores domain events for the entity.
    /// </summary>
    private readonly List<DomainEvent> _domainEvents = [];

    /// <summary>
    /// Exposes domain events as read-only.
    /// </summary>
    [NotMapped]
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents;

    /// <summary>
    /// Adds a domain event to the entity's domain event collection.
    /// </summary>
    /// <param name="domainEvent">The domain event to add.</param>
    public void AddDomainEvent(DomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Removes a domain event from the entity's domain event collection.
    /// </summary>
    /// <param name="domainEvent">The domain event to remove.</param>
    public void RemoveDomainEvent(DomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        _domainEvents.Remove(domainEvent);
    }

    /// <summary>
    /// Clears all domain events from the entity's domain event collection.
    /// </summary>
    public void ClearDomainEvents() => _domainEvents.Clear();
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
    protected Entity() { }

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
    public bool IsTransient() => EqualityComparer<TKey>.Default.Equals(Id, default);

    /// <summary>
    /// Compares the current entity with another entity based on their IDs.
    /// </summary>
    /// <param name="obj">The other entity to compare with.</param>
    /// <returns>True if the entities are equal, otherwise false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is null) return false;

        if (ReferenceEquals(this, obj)) return true;

        if (obj is not Entity<TKey> other) return false;

        if (other.GetType() != GetType()) return false;

        // If either is transient, fall back to reference equality to avoid false positives.
        if (IsTransient() || other.IsTransient()) return false;

        return EqualityComparer<TKey>.Default.Equals(Id!, other.Id!);
    }

    /// <summary>
    /// Equality operator for comparing entities based on their IDs.
    /// </summary>
    /// <param name="left">The left entity.</param>
    /// <param name="right">The right entity.</param>
    /// <returns>True if the entities are equal, otherwise false.</returns>
    public static bool operator ==(Entity<TKey>? left, Entity<TKey>? right) => Equals(left, right);

    /// <summary>
    /// Inequality operator for comparing entities based on their IDs.
    /// </summary>
    /// <param name="left">The left entity.</param>
    /// <param name="right">The right entity.</param>
    /// <returns>True if the entities are not equal, otherwise false.</returns>
    public static bool operator !=(Entity<TKey>? left, Entity<TKey>? right) => !Equals(left, right);

    /// <summary>
    /// Returns the hash code of the entity based on its ID.
    /// </summary>
    /// <returns>The hash code of the entity.</returns>
    public override int GetHashCode()
    {
        if (IsTransient())
        {
            return HashCode.Combine(GetType(), base.GetHashCode());
        }

        return HashCode.Combine(GetType(), Id);
    }
}