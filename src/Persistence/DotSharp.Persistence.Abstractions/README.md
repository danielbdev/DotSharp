# DotSharp.Persistence.Abstractions

Provider-agnostic persistence contracts for .NET — repositories, specifications, unit of work, query service, and outbox/inbox patterns.

## Installation
```bash
dotnet add package DotSharp.Persistence.Abstractions
```

## What's included

### Repositories
```csharp
public sealed class ConfirmOrderHandler(
    IRepository<Order, Guid> repository,
    IUnitOfWork unitOfWork)
{
    public async Task<Result> Handle(Guid id, CancellationToken ct)
    {
        Order? order = await repository.GetByIdAsync(id, ct);
        if (order is null)
            return Errors.NotFound("Order not found.");

        order.Confirm();
        repository.Update(order);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
```

### Specifications
```csharp
public sealed class ActiveOrdersByCustomerSpec : Specification<Order>
{
    public ActiveOrdersByCustomerSpec(Guid customerId, int page, int size)
    {
        SetCriteria(x => x.CustomerId == customerId);
        AndCriteria(x => !x.IsDeleted);
        ApplyOrderBy(x => x.CreatedAt, OrderDirection.Desc);
        ApplyPaging(new Paging(page, size));
        AsNoTracking();
    }
}
```

Use with a repository:
```csharp
PaginationResult<Order> result = await repository.PageAsync(
    new ActiveOrdersByCustomerSpec(customerId, page, size), ct);
```

### Projections
```csharp
public sealed class OrderSummarySpec : Specification<Order, OrderSummaryDto>
{
    public OrderSummarySpec(Guid customerId)
    {
        SetCriteria(x => x.CustomerId == customerId);
        ApplySelector(x => new OrderSummaryDto(x.Id, x.CreatedAt));
        AsNoTracking();
    }
}
```

### Query service

For complex read operations across multiple aggregates:
```csharp
public sealed class OrdersQueryService(IQueryService queryService)
{
    public async Task<List<OrderDto>> GetByCustomerAsync(Guid customerId, CancellationToken ct)
    {
        return await queryService.Query<Order>()
            .Where(x => x.CustomerId == customerId)
            .Select(x => new OrderDto(x.Id, x.CreatedAt))
            .ToListAsync(ct);
    }
}
```

### Unit of work
```csharp
await unitOfWork.ExecuteInTransactionAsync(async () =>
{
    repository.Add(order);
    await unitOfWork.SaveChangesAsync(ct);
    return order.Id;
}, ct);
```

## Design decisions

- `IRepository<TAggregate, TKey>` is constrained to `AggregateRoot<TKey>` — repositories are aggregate-oriented by design.
- `Specification<T>` uses `ExpressionVisitor` for `AndCriteria` — avoids `Expression.Invoke` which cannot be translated to SQL by EF Core.
- `OrderDirection` defaults to `Asc` — consistent with SQL standard.
- `IQueryService` exposes raw `IQueryable<T>` intentionally — complex read queries that span multiple aggregates should not be forced through the repository pattern.
- `IUnitOfWork` provides both automatic (`ExecuteInTransactionAsync`) and manual (`BeginTransactionAsync`/`CommitTransactionAsync`/`RollbackTransactionAsync`) transaction control.
