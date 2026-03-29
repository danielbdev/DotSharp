using System.Linq.Expressions;

namespace DotSharp.Persistence.Abstractions.Specifications;

/// <summary>
/// Represents an ordering expression applied to a query.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
/// <param name="Expression">The ordering expression.</param>
/// <param name="Direction">The ordering direction.</param>
/// <param name="IsThenBy">Whether this is a secondary ordering (ThenBy).</param>
public sealed record OrderExpression<T>(
    Expression<Func<T, object>> Expression,
    OrderDirection Direction,
    bool IsThenBy = false);
