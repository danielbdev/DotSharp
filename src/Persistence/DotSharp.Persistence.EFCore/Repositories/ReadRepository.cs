using DotSharp.Persistence.Abstractions.Repositories;
using DotSharp.Persistence.Abstractions.Specifications;
using DotSharp.Persistence.EFCore.Contexts;
using DotSharp.Primitives.Aggregates;
using DotSharp.Primitives.Guards;
using DotSharp.Primitives.Pagination;
using Microsoft.EntityFrameworkCore;

namespace DotSharp.Persistence.EFCore.Repositories;

/// <summary>
/// EF Core read-only repository for aggregate roots.
/// </summary>
public class ReadRepository<TAggregate, TKey>(
    DotSharpDbContext dbContext,
    ISpecificationEvaluator evaluator) : IReadRepository<TAggregate, TKey>
    where TAggregate : AggregateRoot<TKey>
{
    protected readonly DotSharpDbContext DbContext = dbContext;

    private IQueryable<TAggregate> BaseQuery(ISpecification<TAggregate> specification)
    {
        IQueryable<TAggregate> query = DbContext.Set<TAggregate>().AsQueryable();

        if (!specification.IsTracking && !specification.IsNoTracking && !specification.IsNoTrackingWithIdentityResolution)
            query = query.AsNoTracking();

        return query;
    }

    protected IQueryable<TAggregate> Query(ISpecification<TAggregate>? specification = null)
    {
        if (specification is null)
            return DbContext.Set<TAggregate>().AsNoTracking();

        return evaluator.Apply(BaseQuery(specification), specification);
    }

    /// <inheritdoc />
    public async Task<TAggregate?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
        => await DbContext.Set<TAggregate>()
            .FindAsync([id], cancellationToken);

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default)
        => await DbContext.Set<TAggregate>()
            .AnyAsync(e => e.Id!.Equals(id), cancellationToken);

    /// <inheritdoc />
    public async Task<bool> AnyAsync(ISpecification<TAggregate> specification, CancellationToken cancellationToken = default)
        => await evaluator
            .Apply(BaseQuery(specification), specification, applyPaging: false, applyIncludes: false, applyOrdering: false)
            .AnyAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<int> CountAsync(ISpecification<TAggregate> specification, CancellationToken cancellationToken = default)
        => await evaluator
            .Apply(BaseQuery(specification), specification, applyPaging: false, applyIncludes: false, applyOrdering: false)
            .CountAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<TAggregate?> GetSingleAsync(ISpecification<TAggregate> specification, CancellationToken cancellationToken = default)
        => await Query(specification)
            .FirstOrDefaultAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<TResult?> GetSingleAsync<TResult>(ISpecification<TAggregate, TResult> specification, CancellationToken cancellationToken = default)
        => await evaluator
            .Apply(BaseQuery(specification), specification)
            .FirstOrDefaultAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<TAggregate>> ListAsync(ISpecification<TAggregate> specification, CancellationToken cancellationToken = default)
        => await Query(specification)
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<TResult>> ListAsync<TResult>(ISpecification<TAggregate, TResult> specification, CancellationToken cancellationToken = default)
        => await evaluator
            .Apply(BaseQuery(specification), specification)
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<PaginationResult<TAggregate>> PageAsync(ISpecification<TAggregate> specification, CancellationToken cancellationToken = default)
    {
        Guard.NotNull(specification.Paging, nameof(specification.Paging));

        int total = await evaluator
            .Apply(BaseQuery(specification), specification, applyPaging: false, applyIncludes: false, applyOrdering: false)
            .CountAsync(cancellationToken);

        List<TAggregate> items = await evaluator
            .Apply(BaseQuery(specification), specification, applyPaging: true, applyIncludes: true, applyOrdering: true)
            .ToListAsync(cancellationToken);

        return PaginationResult<TAggregate>.Create(items, total, specification.Paging!.Page, specification.Paging.Size);
    }

    /// <inheritdoc />
    public async Task<PaginationResult<TResult>> PageAsync<TResult>(ISpecification<TAggregate, TResult> specification, CancellationToken cancellationToken = default)
    {
        Guard.NotNull(specification.Paging, nameof(specification.Paging));

        int total = await evaluator
            .Apply(BaseQuery(specification), specification, applyPaging: false, applyIncludes: false, applyOrdering: false)
            .CountAsync(cancellationToken);

        List<TResult> items = await evaluator
            .Apply(BaseQuery(specification), specification, applyPaging: true, applyIncludes: true, applyOrdering: true)
            .ToListAsync(cancellationToken);

        return PaginationResult<TResult>.Create(items, total, specification.Paging!.Page, specification.Paging.Size);
    }
}
