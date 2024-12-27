using DotSharp.Primitives.Entities;

namespace DotSharp.Abstractions.Data.Repositories.Entities;

/// <summary>
/// Defines methods for creating entities in the database.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public interface ICreateRepository<TEntity> where TEntity : Entity
{
    /// <summary>
    /// Adds a new entity to the database.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">The cancellation token for asynchronous operations.</param>
    /// <returns>The added entity.</returns>
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a range of entities to the database.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    /// <param name="cancellationToken">The cancellation token for asynchronous operations.</param>
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
}