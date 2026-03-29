namespace DotSharp.Persistence.Abstractions.Specifications;

/// <summary>
/// Evaluates a specification and applies it to a queryable source.
/// </summary>
public interface ISpecificationEvaluator
{
    /// <summary>
    /// Applies a specification to a queryable source.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="query">The queryable source.</param>
    /// <param name="specification">The specification to apply.</param>
    /// <param name="applyPaging">Whether to apply pagination.</param>
    /// <param name="applyIncludes">Whether to apply includes.</param>
    /// <param name="applyOrdering">Whether to apply ordering.</param>
    IQueryable<T> Apply<T>(
        IQueryable<T> query,
        ISpecification<T>? specification,
        bool applyPaging = true,
        bool applyIncludes = true,
        bool applyOrdering = true) where T : class;

    /// <summary>
    /// Applies a specification with projection to a queryable source.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <typeparam name="TResult">The projection type.</typeparam>
    /// <param name="query">The queryable source.</param>
    /// <param name="specification">The specification to apply.</param>
    /// <param name="applyPaging">Whether to apply pagination.</param>
    /// <param name="applyIncludes">Whether to apply includes.</param>
    /// <param name="applyOrdering">Whether to apply ordering.</param>
    IQueryable<TResult> Apply<T, TResult>(
        IQueryable<T> query,
        ISpecification<T, TResult> specification,
        bool applyPaging = true,
        bool applyIncludes = true,
        bool applyOrdering = true) where T : class;
}
