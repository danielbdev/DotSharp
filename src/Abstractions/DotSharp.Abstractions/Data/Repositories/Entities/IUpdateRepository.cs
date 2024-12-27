using DotSharp.Primitives.Entities;

namespace DotSharp.Abstractions.Data.Repositories.Entities;

/// <summary>
/// Defines methods for updating entities in the database.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public interface IUpdateRepository<TEntity> where TEntity : Entity
{
    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <returns>The updated entity.</returns>
    TEntity Update(TEntity entity);

    /// <summary>
    /// Updates or inserts a range of entities.
    /// </summary>
    /// <param name="entities">The entities to update or insert.</param>
    /// <param name="cancellationToken">The cancellation token for asynchronous operations.</param>
    /// <returns>A collection of updated entities.</returns>
    Task<IEnumerable<TEntity>> UpsertRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);
}