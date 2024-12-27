using DotSharp.Abstractions.Data.Specifications.Entities;
using DotSharp.Primitives.Entities;

namespace DotSharp.Abstractions.Data.Repositories.Entities;

/// <summary>
/// Defines methods for reading entities from the database.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TKey">The type of the entity's key.</typeparam>
public interface IReadRepository<TEntity, TKey> where TEntity : Entity<TKey>
{
    /// <summary>
    /// Gets all entities matching the specified specification with pagination.
    /// </summary>
    /// <param name="specification">The specification to filter entities.</param>
    /// <param name="cancellationToken">The cancellation token for asynchronous operations.</param>
    /// <returns>A paginated result of entities.</returns>
    Task<PaginatedResult<TEntity>> GetAllAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a single entity matching the specified specification.
    /// </summary>
    /// <param name="specification">The specification to filter the entity.</param>
    /// <param name="cancellationToken">The cancellation token for asynchronous operations.</param>
    /// <returns>The entity, or null if no matching entity is found.</returns>
    Task<TEntity?> GetSingleAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
}