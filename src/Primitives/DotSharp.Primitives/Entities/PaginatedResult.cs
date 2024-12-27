namespace DotSharp.Primitives.Entities;

/// <summary>
/// Represents a paginated result set for entities, useful for implementing pagination in APIs.
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
public class PaginatedResult<TEntity> where TEntity : Entity
{
    /// <summary>
    /// The collection of entities for the current page.
    /// </summary>
    public IEnumerable<TEntity> Results { get; protected set; } = null!;

    /// <summary>
    /// The total number of records across all pages.
    /// </summary>
    public int TotalRecords { get; protected set; }

    /// <summary>
    /// The current page number.
    /// </summary>
    public int PageNumber { get; protected set; }

    /// <summary>
    /// The number of records per page.
    /// </summary>
    public int PageSize { get; protected set; }

    /// <summary>
    /// Protected constructor to initialize the paginated result set.
    /// </summary>
    /// <param name="results">The entities for the current page.</param>
    /// <param name="totalRecords">The total number of records.</param>
    /// <param name="pageNumber">The current page number.</param>
    /// <param name="pageSize">The number of records per page.</param>
    protected PaginatedResult(IEnumerable<TEntity> results, int totalRecords, int pageNumber, int pageSize)
    {
        Results = results;
        TotalRecords = totalRecords;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    /// <summary>
    /// Static method to create a paginated result set.
    /// </summary>
    /// <param name="results">The entities for the current page.</param>
    /// <param name="totalRecords">The total number of records.</param>
    /// <param name="pageNumber">The current page number.</param>
    /// <param name="pageSize">The number of records per page.</param>
    /// <returns>A paginated result set.</returns>
    public static PaginatedResult<TEntity> Create(IEnumerable<TEntity> results, int totalRecords, int pageNumber, int pageSize)
    {
        return new(results, totalRecords, pageNumber, pageSize);
    }
}