using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DotSharp.Persistence.EFCore.Extensions;

/// <summary>
/// Extension methods for EF Core queryable operations.
/// </summary>
public static class EntityFrameworkCoreExtensions
{
    /// <summary>
    /// Applies multiple include expressions to a queryable source.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="query">The queryable source.</param>
    /// <param name="includes">The include expressions to apply.</param>
    public static IQueryable<T> IncludeList<T>(
        this IQueryable<T> query,
        params Expression<Func<T, object>>[] includes) where T : class
    {
        foreach (Expression<Func<T, object>> include in includes)
            query = query.Include(include);

        return query;
    }

    /// <summary>
    /// Applies pagination to a queryable source.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="query">The queryable source.</param>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    public static IQueryable<T> Paginate<T>(
        this IQueryable<T> query,
        int pageNumber,
        int pageSize)
        => query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
}
