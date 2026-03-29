using DotSharp.Primitives.Aggregates;

namespace DotSharp.Persistence.Abstractions.Repositories;

/// <summary>
/// Defines write operations for an aggregate root of type <typeparamref name="TAggregate"/>.
/// </summary>
/// <typeparam name="TAggregate">The aggregate root type.</typeparam>
/// <typeparam name="TKey">The aggregate root identifier type.</typeparam>
public interface IRepository<TAggregate, TKey> : IReadRepository<TAggregate, TKey>
    where TAggregate : AggregateRoot<TKey>
{
    /// <summary>
    /// Adds a new aggregate to the store.
    /// </summary>
    void Add(TAggregate aggregate);

    /// <summary>
    /// Adds a range of aggregates to the store.
    /// </summary>
    void AddRange(IEnumerable<TAggregate> aggregates);

    /// <summary>
    /// Updates an existing aggregate in the store.
    /// </summary>
    void Update(TAggregate aggregate);

    /// <summary>
    /// Removes an aggregate from the store.
    /// </summary>
    void Remove(TAggregate aggregate);

    /// <summary>
    /// Removes a range of aggregates from the store.
    /// </summary>
    void RemoveRange(IEnumerable<TAggregate> aggregates);
}
