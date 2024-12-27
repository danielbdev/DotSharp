using DotSharp.Primitives.Events;

namespace DotSharp.Primitives.Entities;

/// <summary>
/// Interface for entities that are capable of raising domain events.
/// </summary>
public interface IEntity : IDomainEvent
{
}

/// <summary>
/// Interface for entities with a generic identifier <typeparamref name="TKey"/>.
/// </summary>
public interface IEntity<TKey> : IEntity
{
    /// <summary>
    /// The unique identifier of the entity.
    /// </summary>
    TKey? Id { get; }
}