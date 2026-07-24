# DotSharp

Reusable .NET infrastructure packages for Clean Architecture & Domain-Driven Design.

## Packages

| Layer | Package | Description |
|---|---|---|
| **Core** | `DotSharp.Primitives` | DDD building blocks: Entity, AggregateRoot, Domain Events, Auditing, Guard, PaginationResult, IClock |
| | `DotSharp.Results` | Result pattern: `Result`, `Result<T>`, `Error`, `ErrorCodes`, `ValidationError` |
| **Application** | `DotSharp.Application` | Custom message bus (Request/Notification) with pipeline behaviors and FluentValidation integration |
| | `DotSharp.Application.Observability` | Tracing and log enrichment behaviors for the application pipeline |
| **Web** | `DotSharp.Web` | Automatic Result→HTTP mapping, correlation middleware, exception handling, validation filters, pagination headers, `DotSharpController` |
| **Caching** | `DotSharp.Caching.Abstractions` | `ICacheService<T>`, `CacheKey` builder, `CacheOptions`, Result bridge, `ISpecification.ToCacheKey()` |
| | `DotSharp.Caching.Memory` | `IMemoryCache` wrapper with per-key double-checked lock stampede protection |
| | `DotSharp.Caching.Redis` | `IDistributedCache` wrapper with pluggable `ISerializer` (System.Text.Json default) |
| **Persistence** | `DotSharp.Persistence.Abstractions` | Repository, Specification, Unit of Work, Outbox, Inbox patterns |
| | `DotSharp.Persistence.EFCore` | EF Core implementation: repositories, specification evaluator, audit logging, soft delete, outbox interceptor |
| **Observability** | `DotSharp.Observability` | Correlation context, trace context, `DotSharpActivitySource`, log enrichment |
| | `DotSharp.Observability.OpenTelemetry` | OpenTelemetry integration bridge |

## Quick Start

```bash
# Build everything
dotnet build

# Run all tests
dotnet test
```

### Install a package

```bash
dotnet add package DotSharp.Results
dotnet add package DotSharp.Web
dotnet add package DotSharp.Caching.Memory
```

## Key Design Principles

- **Zero-dependency Core** — `DotSharp.Primitives` and `DotSharp.Results` have no external dependencies
- **Exception-less errors** — `Result`/`Result<T>` for expected failures, exceptions for bugs only
- **Explicit over magic** — abstractions are interfaces, implementations are opt-in
- **Separation of concerns** — Abstractions packages define contracts, Implementation packages provide concrete types
- **Testable by default** — `IClock` for time, interfaces for everything, DI-friendly

## Tech Stack

- **Framework:** .NET 10.0, C# 13
- **Persistence:** Entity Framework Core
- **Validation:** FluentValidation
- **Observability:** OpenTelemetry
- **DI:** Microsoft.Extensions.DependencyInjection, Scrutor
- **Testing:** xUnit v3, FluentAssertions

## License

MIT
