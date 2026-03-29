using DotSharp.Persistence.Abstractions.Specifications;
using DotSharp.Primitives.Aggregates;
using DotSharp.Primitives.Pagination;

namespace DotSharp.Persistence.Abstractions.Repositories;

/// <summary>
/// Defines read operations for an aggregate root of type <typeparamref name="TAggregate"/>.
/// </summary>
/// <typeparam name="TAggregate">The aggregate root type.</typeparam>
/// <typeparam name="TKey">The aggregate root identifier type.</typeparam>
public interface IReadRepository<TAggregate, TKey> where TAggregate : AggregateRoot<TKey>
{
    /// <summary>
    /// Gets an aggregate by its identifier.
    /// </summary>
    Task<TAggregate?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns true if an aggregate with the given identifier exists.
    /// </summary>
    Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns true if any aggregate matches the specification.
    /// </summary>
    Task<bool> AnyAsync(ISpecification<TAggregate> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the count of aggregates matching the specification.
    /// </summary>
    Task<int> CountAsync(ISpecification<TAggregate> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a single aggregate matching the specification.
    /// </summary>
    Task<TAggregate?> GetSingleAsync(ISpecification<TAggregate> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a single projected result matching the specification.
    /// </summary>
    Task<TResult?> GetSingleAsync<TResult>(ISpecification<TAggregate, TResult> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all aggregates matching the specification.
    /// </summary>
    Task<IReadOnlyList<TAggregate>> ListAsync(ISpecification<TAggregate> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all projected results matching the specification.
    /// </summary>
    Task<IReadOnlyList<TResult>> ListAsync<TResult>(ISpecification<TAggregate, TResult> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a paginated result of aggregates matching the specification.
    /// </summary>
    Task<PaginationResult<TAggregate>> PageAsync(ISpecification<TAggregate> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a paginated result of projected results matching the specification.
    /// </summary>
    Task<PaginationResult<TResult>> PageAsync<TResult>(ISpecification<TAggregate, TResult> specification, CancellationToken cancellationToken = default);
}
