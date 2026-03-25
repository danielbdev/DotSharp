namespace DotSharp.Primitives.Pagination;

/// <summary>
/// Represents a paginated result containing a subset of items and pagination metadata.
/// </summary>
/// <typeparam name="T">The type of items in the result.</typeparam>
public sealed class PaginationResult<T>
{
    /// <summary>
    /// The items in the current page.
    /// </summary>
    public IReadOnlyList<T> Items { get; }

    /// <summary>
    /// The current page number (1-based).
    /// </summary>
    public int PageNumber { get; }

    /// <summary>
    /// The number of items per page.
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// The total number of items across all pages.
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    /// The total number of pages.
    /// </summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;

    /// <summary>
    /// Indicates whether there is a previous page.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Indicates whether there is a next page.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Creates a new paginated result.
    /// </summary>
    /// <param name="items">The items in the current page.</param>
    /// <param name="totalCount">The total number of items across all pages.</param>
    /// <param name="pageNumber">The current page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    private PaginationResult(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageNumber);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageSize);
        ArgumentOutOfRangeException.ThrowIfNegative(totalCount);

        Items = items?.ToList().AsReadOnly() ?? [];
        TotalCount = Math.Max(0, totalCount);
        PageNumber = Math.Max(1, pageNumber);
        PageSize = Math.Max(1, pageSize);
    }

    /// <summary>
    /// Creates a new <see cref="PaginationResult{T}"/> with the specified items and pagination parameters.
    /// </summary>
    /// <param name="items">The items in the current page.</param>
    /// <param name="totalCount">The total number of items across all pages.</param>
    /// <param name="pageNumber">The current page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A new <see cref="PaginationResult{T}"/> instance.</returns>
    public static PaginationResult<T> Create(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
    {
        return new PaginationResult<T>(items, totalCount, pageNumber, pageSize);
    }
}