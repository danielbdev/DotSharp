using DotSharp.Persistence.Abstractions.Specifications;
using DotSharp.Persistence.EFCore.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DotSharp.Persistence.EFCore.Specifications;

/// <summary>
/// EF Core implementation of <see cref="ISpecificationEvaluator"/>.
/// Applies specification criteria, includes, ordering and paging to a queryable source.
/// </summary>
public sealed class SpecificationEvaluator : ISpecificationEvaluator
{
    /// <inheritdoc />
    public IQueryable<T> Apply<T>(
        IQueryable<T> query,
        ISpecification<T>? specification,
        bool applyPaging = true,
        bool applyIncludes = true,
        bool applyOrdering = true) where T : class
    {
        if (specification is null)
            return query;

        if (specification.IsTracking)
            query = query.AsTracking();

        else if (specification.IsNoTracking)
            query = query.AsNoTracking();

        else if (specification.IsNoTrackingWithIdentityResolution)
            query = query.AsNoTrackingWithIdentityResolution();

        if (specification.IsSplitQuery)
            query = query.AsSplitQuery();

        query = query.Where(specification.Criteria);

        if (applyIncludes)
        {
            if (specification.Includes.Count > 0)
                query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));

            if (specification.IncludeStrings.Count > 0)
                query = specification.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));
        }

        if (applyOrdering && specification.OrderExpressions.Count > 0)
        {
            IOrderedQueryable<T>? ordered = null;

            foreach (OrderExpression<T> order in specification.OrderExpressions)
            {
                if (!order.IsThenBy)
                {
                    ordered = order.Direction == OrderDirection.Desc
                        ? query.OrderByDescending(order.Expression)
                        : query.OrderBy(order.Expression);
                }
                else
                {
                    ordered = order.Direction == OrderDirection.Desc
                        ? ordered!.ThenByDescending(order.Expression)
                        : ordered!.ThenBy(order.Expression);
                }
            }

            if (ordered is not null)
                query = ordered;
        }

        if (applyPaging && specification.Paging is not null)
            query = query.Paginate(specification.Paging.Page, specification.Paging.Size);

        return query;
    }

    /// <inheritdoc />
    public IQueryable<TResult> Apply<T, TResult>(
        IQueryable<T> query,
        ISpecification<T, TResult> specification,
        bool applyPaging = true,
        bool applyIncludes = true,
        bool applyOrdering = true) where T : class
    {
        IQueryable<T> baseQuery = Apply(query, (ISpecification<T>)specification, applyPaging, applyIncludes, applyOrdering);
        return baseQuery.Select(specification.Selector);
    }
}
