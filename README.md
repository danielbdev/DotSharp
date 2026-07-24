# DotSharp

Reusable .NET infrastructure packages for Clean Architecture & Domain-Driven Design.

## Why DotSharp?

Every Clean Architecture project rewrites the same plumbing: result types, repository patterns, message buses, caching wrappers, HTTP result mapping. DotSharp gives you all of that — **battle-tested, zero-dependency core, testable by default** — so you focus on domain logic instead.

No framework lock-in. No magic. Pick the packages you need, wire them up with one-liner extensions, and write your domain.

## At a Glance

```csharp
// --- Domain: command + query ---
public sealed record CreateOrderCommand(string CustomerName, decimal Total)
    : IRequest<Result<Guid>>;

public sealed record GetOrderQuery(Guid Id) : IRequest<Result<Order>>;

public sealed class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.CustomerName).NotEmpty();
        RuleFor(x => x.Total).GreaterThan(0);
    }
}

// --- Application: handlers ---
public sealed class CreateOrderHandler(
    IRepository<Order, Guid> repo,
    IUnitOfWork uow,
    IClock clock) : IRequestHandler<CreateOrderCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateOrderCommand cmd, CancellationToken ct)
    {
        var order = new Order(cmd.CustomerName, cmd.Total, clock.UtcNow);
        repo.Add(order);
        await uow.SaveChangesAsync(ct);
        return order.Id;
    }
}

public sealed class GetOrderHandler(
    IReadRepository<Order, Guid> repo,
    ICacheService<Order> cache) : IRequestHandler<GetOrderQuery, Result<Order>>
{
    public async Task<Result<Order>> Handle(GetOrderQuery query, CancellationToken ct)
    {
        var order = await cache.GetOrSetAsync(
            CacheKey.For<Order>().WithSegment(query.Id.ToString()),
            ct => repo.GetByIdAsync(query.Id, ct),
            ct: ct);

        return order is not null ? order : Result<Order>.Failure(Errors.NotFound());
    }
}

// --- Web: zero-effort controller ---
[Route("orders")]
public sealed class OrdersController : DotSharpController
{
    [HttpPost]
    public Task<Result<Guid>> Create(CreateOrderCommand command, CancellationToken ct)
        => MessageBus.Send(command, ct);

    [HttpGet("{id:guid}")]
    public Task<Result<Order>> GetById(Guid id, CancellationToken ct)
        => MessageBus.Send(new GetOrderQuery(id), ct);
}

// --- Program.cs: one-liner setup ---
builder.Services.AddDotSharpWeb();
builder.Services.AddDotSharpCachingMemory();
builder.Services.AddDotSharpPersistenceEFCore<AppDbContext>();
builder.Services.AddDotSharpApplication(typeof(CreateOrderHandler).Assembly);

app.UseDotSharpCorrelation();
app.UseDotSharpExceptionHandling();
app.MapControllers();
```

What's happening here:

- `CreateOrderHandler` uses `IRepository`, `IUnitOfWork`, and `IClock` — all injectable, testable
- `GetOrderHandler` uses `IReadRepository` + `ICacheService` — `GetOrSetAsync` with `CacheKey` builder avoids hand-crafted string keys
- `CreateOrderValidator` runs automatically via `ValidationBehavior` — registered by `AddDotSharpApplication`
- `OrdersController` returns `Task<Result<T>>` directly — `ResultToHttpFilter` maps success/failure to HTTP responses, no manual `.ToIResult()`
- `DotSharpController` gives lazy `IMessageBus` — no constructor injection boilerplate
- `UseDotSharpCorrelation()` propagates `X-Correlation-Id` across every request
- Three extensions in `Program.cs` wire up Web, Caching, and Persistence

## Packages

| Layer | Package | Description |
|---|---|---|
| **Core** | `DotSharp.Primitives` | DDD building blocks: Entity, AggregateRoot, Domain Events, Auditing, Guard, PaginationResult, IClock |
| | `DotSharp.Results` | Exception-less error handling: `Result`, `Result<T>`, `Error`, `ErrorCodes`, `ValidationError` |
| **Application** | `DotSharp.Application` | Custom message bus (Request/Notification) with pipeline behaviors and FluentValidation |
| | `DotSharp.Application.Observability` | Tracing and log enrichment behaviors for the application pipeline |
| **Web** | `DotSharp.Web` | Automatic Result→HTTP mapping, correlation middleware, exception handling, validation filters, pagination headers, `DotSharpController` |
| **Caching** | `DotSharp.Caching.Abstractions` | `ICacheService<T>`, `CacheKey` builder, `CacheOptions`, Result bridge, `ISpecification.ToCacheKey()` |
| | `DotSharp.Caching.Memory` | `IMemoryCache` wrapper with per-key double-checked lock stampede protection |
| | `DotSharp.Caching.Redis` | `IDistributedCache` wrapper with pluggable `ISerializer` |
| **Persistence** | `DotSharp.Persistence.Abstractions` | Repository, Specification, Unit of Work, Outbox, Inbox |
| | `DotSharp.Persistence.EFCore` | EF Core implementation: repositories, specification evaluator, audit logging, soft delete, outbox interceptor |
| **Observability** | `DotSharp.Observability` | Correlation context, trace context, `DotSharpActivitySource`, log enrichment |
| | `DotSharp.Observability.OpenTelemetry` | OpenTelemetry integration bridge |

## Quick Start

```bash
dotnet build   # 0 warnings, 0 errors
dotnet test    # all tests pass
```

## Design Principles

- **Zero-dependency Core** — `DotSharp.Primitives` and `DotSharp.Results` have no external dependencies
- **Exception-less errors** — `Result`/`Result<T>` for expected failures, exceptions for bugs only
- **Explicit, not magic** — abstractions are interfaces, implementations are opt-in, nothing happens behind your back
- **Separation of concerns** — Abstractions define contracts, Implementation packages provide concrete types
- **Testable by default** — `IClock` for time, interfaces everywhere, DI-friendly constructors

## Tech Stack

.NET 10.0, C# 13 · Entity Framework Core · FluentValidation · OpenTelemetry · Scrutor · xUnit v3 · FluentAssertions

## License

MIT