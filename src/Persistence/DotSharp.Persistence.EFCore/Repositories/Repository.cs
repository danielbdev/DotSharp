using DotSharp.Persistence.Abstractions.Repositories;
using DotSharp.Persistence.Abstractions.Specifications;
using DotSharp.Persistence.EFCore.Contexts;
using DotSharp.Primitives.Aggregates;

namespace DotSharp.Persistence.EFCore.Repositories;

/// <summary>
/// EF Core repository for aggregate roots with write operations.
/// Changes are persisted explicitly via <see cref="Abstractions.UnitOfWork.IUnitOfWork"/>.
/// </summary>
public class Repository<TAggregate, TKey>(
    DotSharpDbContext dbContext,
    ISpecificationEvaluator evaluator)
    : ReadRepository<TAggregate, TKey>(dbContext, evaluator), IRepository<TAggregate, TKey>
    where TAggregate : AggregateRoot<TKey>
{
    /// <inheritdoc />
    public void Add(TAggregate aggregate)
        => DbContext.Set<TAggregate>().Add(aggregate);

    /// <inheritdoc />
    public void AddRange(IEnumerable<TAggregate> aggregates)
        => DbContext.Set<TAggregate>().AddRange(aggregates);

    /// <inheritdoc />
    public void Update(TAggregate aggregate)
        => DbContext.Set<TAggregate>().Update(aggregate);

    /// <inheritdoc />
    public void Remove(TAggregate aggregate)
        => DbContext.Set<TAggregate>().Remove(aggregate);

    /// <inheritdoc />
    public void RemoveRange(IEnumerable<TAggregate> aggregates)
        => DbContext.Set<TAggregate>().RemoveRange(aggregates);
}
