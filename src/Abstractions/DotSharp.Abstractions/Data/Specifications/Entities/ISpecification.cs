using DotSharp.Primitives.Constants;
using DotSharp.Primitives.Entities;
using System.Linq.Expressions;

namespace DotSharp.Abstractions.Data.Specifications.Entities;

/// <summary>
/// Interface for specifying query criteria and options for entities.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public interface ISpecification<TEntity> where TEntity : Entity
{
    /// <summary>
    /// Gets a value indicating whether tracking is enabled.
    /// </summary>
    bool IsTracking { get; }

    /// <summary>
    /// Gets a value indicating whether no tracking is enabled.
    /// </summary>
    bool IsNoTracking { get; }

    /// <summary>
    /// Gets a value indicating whether no tracking with identity resolution is enabled.
    /// </summary>
    bool IsNoTrackingWithIdentityResolution { get; }

    /// <summary>
    /// Gets a value indicating whether split query is enabled.
    /// </summary>
    bool IsSplitQuery { get; }

    /// <summary>
    /// Gets the list of include expressions.
    /// </summary>
    List<Expression<Func<TEntity, object>>> Includes { get; }

    /// <summary>
    /// Gets the list of include strings.
    /// </summary>
    List<string> IncludeStrings { get; }

    /// <summary>
    /// Gets the criteria expression for filtering entities.
    /// </summary>
    Expression<Func<TEntity, bool>> Criteria { get; }

    /// <summary>
    /// Gets a value indicating whether paging is enabled.
    /// </summary>
    bool IsPagingEnabled { get; }

    /// <summary>
    /// Gets the number of items to skip.
    /// </summary>
    int Skip { get; }

    /// <summary>
    /// Gets the number of items to take.
    /// </summary>
    int Take { get; }

    /// <summary>
    /// Gets the total number of records.
    /// </summary>
    int TotalRecords { get; }

    /// <summary>
    /// Gets the order direction for sorting.
    /// </summary>
    OrderDirection OrderDirection { get; }

    /// <summary>
    /// Gets the expression for ordering entities.
    /// </summary>
    Expression<Func<TEntity, object>> OrderBy { get; }

    /// <summary>
    /// Gets a value indicating whether caching is enabled.
    /// </summary>
    bool IsCacheEnabled { get; }

    /// <summary>
    /// Gets the cache object.
    /// </summary>
    object? CacheObject { get; }

    /// <summary>
    /// Gets the cache expiration time in minutes.
    /// </summary>
    int CacheExpirationTime { get; }

    #region Methods

    /// <summary>
    /// Sets the total number of records.
    /// </summary>
    /// <param name="queryCount">The total number of records.</param>
    public void SetTotalRecords(int queryCount);

    #endregion Methods
}