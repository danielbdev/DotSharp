using DotSharp.Primitives.Entities;
using System.Linq.Expressions;

namespace DotSharp.Abstractions.Data.Repositories.Entities;

/// <summary>
/// Defines methods for deleting entities from the database.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TKey">The type of the entity's key.</typeparam>
public interface IDeleteRepository<TEntity, TKey> where TEntity : Entity<TKey>
{
    /// <summary>
    /// Deletes a single entity.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <returns>The key of the deleted entity.</returns>
    TKey Delete(TEntity entity);

    /// <summary>
    /// Deletes entities that match the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to filter entities to delete.</param>
    /// <returns>A collection of keys of the deleted entities.</returns>
    IEnumerable<TKey> DeleteWhere(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Deletes a range of entities.
    /// </summary>
    /// <param name="entities">The entities to delete.</param>
    void DeleteRange(IEnumerable<TEntity> entities);
}