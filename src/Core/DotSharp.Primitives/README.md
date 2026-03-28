# DotSharp.Primitives

Core building blocks for Domain-Driven Design in .NET.

## Installation
```bash
dotnet add package DotSharp.Primitives
```

## What's included

### Entities

Base classes for domain entities with optional primary key and domain event support.
```csharp
public class Order : Entity<Guid>
{
    public Order(Guid id) : base(id) { }
}

// Join entity (no primary key)
public class OrderProduct : Entity { }
```

### Aggregate roots

Entry points to aggregate boundaries. Extend `Entity` with aggregate semantics.
```csharp
public class Order : AggregateRoot<Guid>
{
    private Order() { }

    public static Order Create(Guid id)
    {
        var order = new Order(id);
        order.AddDomainEvent(new OrderCreatedEvent(id));
        return order;
    }
}
```

### Auditable variants

Automatically track who created, modified, or deleted an entity.

| Class | Tracks |
|-------|--------|
| `CreationAuditableEntity<TKey>` | `CreatedAt`, `CreatedBy` |
| `AuditableEntity<TKey>` | `CreatedAt`, `CreatedBy`, `LastModifiedAt`, `LastModifiedBy` |
| `FullAuditableEntity<TKey>` | All of the above + `IsDeleted`, `DeletedAt`, `DeletedBy` |

Same variants are available for aggregate roots (`CreationAuditableAggregateRoot`, etc.).

### Domain events
```csharp
public sealed record OrderCreatedEvent(Guid OrderId) : DomainEvent;
```

Use `OutboxEventAttribute` to mark events that should be persisted via the outbox pattern:
```csharp
[OutboxEvent]
public sealed record OrderCreatedEvent(Guid OrderId) : DomainEvent;
```

### Change tracking

Opt-in per-entity change history via `[TrackChanges]`:
```csharp
[TrackChanges]
public class Order : FullAuditableAggregateRoot<Guid> { }
```

Requires `IAuditUserProvider` and `IAuditLog` to be implemented and registered in your infrastructure layer.

### Guard

Argument validation utility:
```csharp
Guard.Against.Null(order, nameof(order));
Guard.Against.NullOrEmpty(name, nameof(name));
```

### Pagination
```csharp
PaginationResult<OrderDto> result = new(items, totalCount, pageNumber, pageSize);
```

## Design decisions

- `Entity` (keyless) exists for join entities in many-to-many relationships where identity is defined by the combination of foreign keys.
- Auditable variants are intentionally duplicated between `Entities/` and `Aggregates/` — C# single inheritance makes unification impossible without breaking the type chain.
- `IHasDomainEvents` lives under `Events/` since it belongs conceptually to the event model, not to entities.
- `[TrackChanges]` is separate from auditable base classes — tracking field values (audit log) is a different concern from tracking who/when (audit fields).
- `IAuditUserProvider` and `IAuditLog` are intentionally left without implementations in this package — infrastructure concerns belong in the consumer's project.
