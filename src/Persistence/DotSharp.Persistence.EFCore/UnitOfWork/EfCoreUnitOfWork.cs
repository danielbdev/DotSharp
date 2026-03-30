using DotSharp.Persistence.Abstractions.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace DotSharp.Persistence.EFCore.UnitOfWork;

/// <summary>
/// EF Core implementation of <see cref="IUnitOfWork"/>.
/// Coordinates persistence operations over a <see cref="DbContext"/>.
/// </summary>
public class EfCoreUnitOfWork(DbContext dbContext) : IUnitOfWork
{
    /// <inheritdoc />
    public int SaveChanges()
        => dbContext.SaveChanges();

    /// <inheritdoc />
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => dbContext.SaveChangesAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default)
    {
        await BeginTransactionAsync(cancellationToken);

        try
        {
            T result = await operation();
            await CommitTransactionAsync(cancellationToken);
            return result;
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        => await dbContext.Database.BeginTransactionAsync(cancellationToken);

    /// <inheritdoc />
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        => await dbContext.Database.CommitTransactionAsync(cancellationToken);

    /// <inheritdoc />
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        => await dbContext.Database.RollbackTransactionAsync(cancellationToken);
}
