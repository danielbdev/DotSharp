using DotSharp.Persistence.Abstractions.Pagination;
using DotSharp.Persistence.Abstractions.Queries;
using DotSharp.Persistence.EFCore.Contexts;
using DotSharp.Persistence.EFCore.Extensions;
using DotSharp.Primitives.Pagination;
using Microsoft.EntityFrameworkCore;

namespace DotSharp.Persistence.EFCore.Queries;

/// <summary>
/// Base class for query services that need direct access to the database for complex read operations.
/// Implements <see cref="IQueryService"/> and provides helpers for pagination and single results.
/// </summary>
public abstract class QueryServiceBase(DotSharpDbContext context) : IQueryService
{
    /// <inheritdoc />
    public IQueryable<T> Query<T>() where T : class
        => context.Set<T>().AsNoTracking();

    /// <summary>
    /// Returns the first matching result or null.
    /// </summary>
    protected static async Task<TDto?> SingleAsync<TDto>(
        IQueryable<TDto> query,
        CancellationToken cancellationToken = default)
        => await query.FirstOrDefaultAsync(cancellationToken);

    /// <summary>
    /// Returns a paginated result from a queryable source.
    /// </summary>
    protected static async Task<PaginationResult<TDto>> PaginateAsync<TDto>(
        IQueryable<TDto> query,
        Paging paging,
        CancellationToken cancellationToken = default)
    {
        int total = await query.CountAsync(cancellationToken);

        List<TDto> items = await query
            .Paginate(paging.Page, paging.Size)
            .ToListAsync(cancellationToken);

        return PaginationResult<TDto>.Create(items, total, paging.Page, paging.Size);
    }

    /// <summary>
    /// Returns a paginated result from a queryable source with in-memory mapping.
    /// Useful for raw intermediate type pattern where mapping cannot be done in SQL.
    /// </summary>
    protected static async Task<PaginationResult<TDto>> PaginateAsync<TRaw, TDto>(
        IQueryable<TRaw> query,
        Paging paging,
        Func<List<TRaw>, List<TDto>> mapper,
        CancellationToken cancellationToken = default)
    {
        int total = await query.CountAsync(cancellationToken);

        List<TRaw> items = await query
            .Paginate(paging.Page, paging.Size)
            .ToListAsync(cancellationToken);

        return PaginationResult<TDto>.Create(mapper(items), total, paging.Page, paging.Size);
    }
}
