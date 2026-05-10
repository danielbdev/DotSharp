# DotSharp.Persistence.EFCore

Entity Framework Core implementation of DotSharp persistence abstractions.

## Installation
```bash
dotnet add package DotSharp.Persistence.EFCore
```

## What's included

### Registration
```csharp
services.AddDotSharpPersistenceEFCore<ApplicationDbContext>();
```

### Repositories
Generic repositories for basic CRUD and complex specification-based queries.
```csharp
// Read-only operations
IReadRepository<Order, Guid> readRepo;
var order = await readRepo.GetByIdAsync(orderId);
var activeOrders = await readRepo.ListAsync(new ActiveOrdersSpec());

// Write operations
IRepository<Order, Guid> repo;
repo.Add(newOrder);
repo.Update(existingOrder);
repo.Remove(toDelete);
```

### Unit of Work
Coordinates transactions and `SaveChangesAsync` calls.
```csharp
public class CreateOrderHandler(IUnitOfWork unitOfWork, IRepository<Order, Guid> repo)
{
    public async Task Handle(CreateOrder command)
    {
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            repo.Add(new Order());
            await unitOfWork.SaveChangesAsync();
            return Result.Success();
        });
    }
}
```

### Interceptors

| Interceptor | Purpose |
|-------------|---------|
| `AuditSaveChangesInterceptor` | Automatically populates `CreatedAt`, `CreatedBy`, etc. Handles Soft Delete. |
| `AuditLogSaveChangesInterceptor` | Generates detailed JSON change history for entities with `[TrackChanges]`. |
| `OutboxSaveChangesInterceptor` | Converts `IDomainEvent` to `OutboxMessage` during SaveChanges. |

### Query Service
Base class for complex read-only operations that require direct `IQueryable` access.
```csharp
public class OrderQueryService(DotSharpDbContext context) : QueryServiceBase(context)
{
    public async Task<PaginationResult<OrderDto>> GetPagedOrders(Paging paging)
    {
        var query = Query<Order>().Where(x => !x.IsDeleted);
        return await PaginateAsync(query, paging);
    }
}
```

### Specifications
The `SpecificationEvaluator` applies criteria, includes (string and expression based), ordering (including `ThenBy`), and paging to any `IQueryable`.

## Design decisions

- `DotSharpDbContext` includes required `DbSet` for `AuditLogEntries`, `OutboxMessages`, and `InboxConsumers` to ensure infrastructure patterns work out-of-the-box.
- `AuditSaveChangesInterceptor` transparently converts physical `Delete` operations into `Update` for entities implementing `IFullAuditable` (Soft Delete).
- `OutboxSaveChangesInterceptor` uses `ConditionalWeakTable` to track entities between `Saving` and `Saved` events, ensuring domain events are cleared only after successful persistence.
- `QueryServiceBase` enforces `AsNoTracking()` by default to promote better performance in read-heavy operations.
- `ExecuteInTransactionAsync` in `EfCoreUnitOfWork` provides a safe wrapper that handles `Begin`, `Commit`, and `Rollback` automatically.
- Multi-property inclusion is simplified via the `IncludeList` extension method.
