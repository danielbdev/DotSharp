namespace DotSharp.Abstractions.Data.Providers;

/// <summary>
/// Provides a method to get an instance of a database context.
/// </summary>
/// <typeparam name="TContext">The type of the database context.</typeparam>
public interface IDbContextProvider<TContext> where TContext : IDbContext
{
    /// <summary>
    /// Gets the database context.
    /// </summary>
    /// <returns>The database context of type <typeparamref name="TContext"/>.</returns>
    TContext GetDbContext();
}