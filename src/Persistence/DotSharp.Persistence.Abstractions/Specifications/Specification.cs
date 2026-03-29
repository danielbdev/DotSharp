using System.Linq.Expressions;
using DotSharp.Persistence.Abstractions.Pagination;

namespace DotSharp.Persistence.Abstractions.Specifications;

/// <summary>
/// Base class for specifications that filter, order, include and paginate queries.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
public abstract class Specification<T> : ISpecification<T> where T : class
{
    private bool _criteriaSet = false;
    private readonly List<OrderExpression<T>> _orderExpressions = [];

    /// <inheritdoc />
    public bool IsTracking { get; private set; }

    /// <inheritdoc />
    public bool IsNoTracking { get; private set; }

    /// <inheritdoc />
    public bool IsNoTrackingWithIdentityResolution { get; private set; }

    /// <inheritdoc />
    public bool IsSplitQuery { get; private set; }

    /// <inheritdoc />
    public List<Expression<Func<T, object>>> Includes { get; } = [];

    /// <inheritdoc />
    public List<string> IncludeStrings { get; } = [];

    /// <inheritdoc />
    public Expression<Func<T, bool>> Criteria { get; private set; } = _ => true;

    /// <inheritdoc />
    public Paging? Paging { get; private set; }

    /// <inheritdoc />
    public IReadOnlyList<OrderExpression<T>> OrderExpressions => _orderExpressions;

    /// <summary>
    /// Enables change tracking for the query.
    /// </summary>
    protected void AsTracking()
    {
        IsTracking = true;
        IsNoTracking = false;
        IsNoTrackingWithIdentityResolution = false;
    }

    /// <summary>
    /// Disables change tracking for the query.
    /// </summary>
    protected void AsNoTracking()
    {
        IsTracking = false;
        IsNoTracking = true;
        IsNoTrackingWithIdentityResolution = false;
    }

    /// <summary>
    /// Disables change tracking but resolves identity for the query.
    /// </summary>
    protected void AsNoTrackingWithIdentityResolution()
    {
        IsTracking = false;
        IsNoTracking = false;
        IsNoTrackingWithIdentityResolution = true;
    }

    /// <summary>
    /// Enables split query execution.
    /// </summary>
    protected void AsSplitQuery()
    {
        IsSplitQuery = true;
    }

    /// <summary>
    /// Adds a navigation property include expression.
    /// </summary>
    /// <param name="includeExpression">The include expression.</param>
    protected void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    /// <summary>
    /// Adds a string-based include for nested navigation properties (e.g. "EntityA.EntityB").
    /// </summary>
    /// <param name="includeString">The include string.</param>
    protected void AddInclude(string includeString)
    {
        IncludeStrings.Add(includeString);
    }

    /// <summary>
    /// Sets the filter criteria expression.
    /// </summary>
    /// <param name="criteriaExpression">The criteria expression.</param>
    protected void SetCriteria(Expression<Func<T, bool>> criteriaExpression)
    {
        Criteria = criteriaExpression;
        _criteriaSet = true;
    }

    /// <summary>
    /// Combines the existing criteria with an additional condition using AND.
    /// </summary>
    /// <param name="additional">The additional criteria expression.</param>
    protected void AndCriteria(Expression<Func<T, bool>> additional)
    {
        if (!_criteriaSet)
        {
            Criteria = additional;
            _criteriaSet = true;
            return;
        }

        ParameterExpression parameter = Expression.Parameter(typeof(T));

        ReplaceParameterVisitor leftVisitor = new(Criteria.Parameters[0], parameter);
        Expression left = leftVisitor.Visit(Criteria.Body);

        ReplaceParameterVisitor rightVisitor = new(additional.Parameters[0], parameter);
        Expression right = rightVisitor.Visit(additional.Body);

        Criteria = Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left, right), parameter);
    }

    /// <summary>
    /// Applies pagination to the query.
    /// </summary>
    /// <param name="paging">The pagination parameters.</param>
    protected void ApplyPaging(Paging paging)
    {
        Paging = paging;
    }

    /// <summary>
    /// Applies primary ordering to the query.
    /// </summary>
    /// <param name="expr">The ordering expression.</param>
    /// <param name="direction">The ordering direction. Defaults to ascending.</param>
    protected void ApplyOrderBy(Expression<Func<T, object>> expr, OrderDirection direction = OrderDirection.Asc)
    {
        _orderExpressions.Add(new OrderExpression<T>(expr, direction, IsThenBy: false));
    }

    /// <summary>
    /// Applies secondary ordering to the query.
    /// </summary>
    /// <param name="expr">The ordering expression.</param>
    /// <param name="direction">The ordering direction. Defaults to ascending.</param>
    protected void ThenOrderBy(Expression<Func<T, object>> expr, OrderDirection direction = OrderDirection.Asc)
    {
        _orderExpressions.Add(new OrderExpression<T>(expr, direction, IsThenBy: true));
    }

    private sealed class ReplaceParameterVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
        : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node)
            => node == oldParameter ? newParameter : base.VisitParameter(node);
    }
}

/// <summary>
/// Base class for specifications that project query results to <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
/// <typeparam name="TResult">The projection type.</typeparam>
public abstract class Specification<T, TResult> : Specification<T>, ISpecification<T, TResult> where T : class
{
    /// <inheritdoc />
    public Expression<Func<T, TResult>> Selector { get; private set; } = default!;

    /// <summary>
    /// Applies a selector expression for projecting entities to <typeparamref name="TResult"/>.
    /// </summary>
    /// <param name="selector">The selector expression.</param>
    protected void ApplySelector(Expression<Func<T, TResult>> selector)
    {
        Selector = selector;
    }
}
