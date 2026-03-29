using DotSharp.Persistence.Abstractions.Pagination;
using System.Linq.Expressions;

namespace DotSharp.Persistence.Abstractions.Specifications;

/// <summary>
/// Defines a specification for querying entities of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
public interface ISpecification<T> where T : class
{
    /// <summary>
    /// Whether the query should track changes.
    /// </summary>
    bool IsTracking { get; }

    /// <summary>
    /// Whether the query should not track changes.
    /// </summary>
    bool IsNoTracking { get; }

    /// <summary>
    /// Whether the query should not track changes but resolve identity.
    /// </summary>
    bool IsNoTrackingWithIdentityResolution { get; }

    /// <summary>
    /// Whether the query should use split queries.
    /// </summary>
    bool IsSplitQuery { get; }

    /// <summary>
    /// The list of include expressions.
    /// </summary>
    List<Expression<Func<T, object>>> Includes { get; }

    /// <summary>
    /// The list of include strings for nested includes.
    /// </summary>
    List<string> IncludeStrings { get; }

    /// <summary>
    /// The filter criteria expression.
    /// </summary>
    Expression<Func<T, bool>> Criteria { get; }

    /// <summary>
    /// The pagination parameters.
    /// </summary>
    Paging? Paging { get; }

    /// <summary>
    /// The list of ordering expressions.
    /// </summary>
    IReadOnlyList<OrderExpression<T>> OrderExpressions { get; }
}

/// <summary>
/// Defines a specification for querying entities of type <typeparamref name="T"/> and projecting to <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
/// <typeparam name="TResult">The projection type.</typeparam>
public interface ISpecification<T, TResult> : ISpecification<T> where T : class
{
    /// <summary>
    /// The selector expression for projecting entities to <typeparamref name="TResult"/>.
    /// </summary>
    Expression<Func<T, TResult>> Selector { get; }
}
