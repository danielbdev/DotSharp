using DotSharp.Primitives.Entities;

namespace DotSharp.Primitives.Aggregates;

/// <summary>
/// Base aggregate root without a typed identifier.
/// Intended for aggregate roots without a single primary key, such as join entities in many-to-many relationships.
/// For aggregate roots with a primary key, use <see cref="AggregateRoot{TKey}"/> instead.
/// </summary>
public abstract class AggregateRoot : Entity, IAggregateRoot
{
}

/// <summary>
/// Base class for aggregate roots with a typed identifier.
/// </summary>
/// <typeparam name="TKey"></typeparam>
public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot<TKey>
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    protected AggregateRoot() { }

    /// <summary>
    /// Constructor that sets the identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the aggregate root.</param>
    protected AggregateRoot(TKey id) : base(id) { }
}