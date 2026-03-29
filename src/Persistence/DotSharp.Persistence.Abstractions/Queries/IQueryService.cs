namespace DotSharp.Persistence.Abstractions.Queries;

/// <summary>
/// Provides access to a queryable source for read-only queries.
/// Intended for complex read operations that go beyond the repository pattern.
/// </summary>
public interface IQueryService
{
    /// <summary>
    /// Returns a queryable source for the specified entity type.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    IQueryable<T> Query<T>() where T : class;
}
