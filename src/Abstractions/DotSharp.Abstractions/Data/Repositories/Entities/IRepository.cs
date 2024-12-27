using DotSharp.Primitives.Entities;

namespace DotSharp.Abstractions.Data.Repositories.Entities;

/// <summary>
/// Combines all repository operations (create, update, delete, read) for an entity type.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TKey">The type of the entity's key.</typeparam>
public interface IRepository<TEntity, TKey> : ICreateRepository<TEntity>, IUpdateRepository<TEntity>, IDeleteRepository<TEntity, TKey>, IReadRepository<TEntity, TKey> where TEntity : Entity<TKey>
{
}