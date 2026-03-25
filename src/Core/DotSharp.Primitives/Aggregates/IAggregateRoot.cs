namespace DotSharp.Primitives.Aggregates;

/// <summary>
/// Marker interface for aggregate roots.
/// Aggregate roots are the entry points to aggregate boundaries in DDD.
/// </summary>
public interface IAggregateRoot { }

/// <summary>
/// Marker interface for aggregate roots with a typed identifier.
/// </summary>
/// <typeparam name="TKey">The type of the aggregate root identifier.</typeparam>
public interface IAggregateRoot<TKey> : IAggregateRoot
{
    /// <summary>
    /// The unique identifier of the aggregate root.
    /// </summary>
    TKey? Id { get; }
}