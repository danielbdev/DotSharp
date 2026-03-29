namespace DotSharp.Persistence.Abstractions.Pagination;

/// <summary>
/// Represents the pagination input parameters for a query.
/// </summary>
/// <param name="Page">The current page number (1-based).</param>
/// <param name="Size">The number of items per page.</param>
public sealed record Paging(int Page, int Size);
