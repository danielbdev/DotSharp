namespace DotSharp.Primitives.Entities;

/// <summary>
/// Marker interface for entities.
/// </summary>
public interface IEntity { }

/// <summary>
/// Interface for entities with a generic identifier <typeparamref name="TKey"/>.
/// </summary>
public interface IEntity<TKey> : IEntity
{
    /// <summary>
    /// The unique identifier of the entity.
    /// </summary>
    TKey? Id { get; }

    /// <summary>
    /// Indicates whether the entity is transient (Id not set).
    /// </summary>
    bool IsTransient();
}