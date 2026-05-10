# DotSharp

Reusable .NET infrastructure packages for Clean Architecture & Domain-Driven Design (DDD).

## Project Overview

DotSharp is a collection of libraries designed to provide a solid technical foundation for .NET applications. It promotes best practices like exception-less error handling (Result pattern), explicit domain modeling (DDD base classes), and decoupled communication (Request/Notification bus).

### Key Technologies

- **Framework:** .NET 10.0
- **Language:** C# 13 (Implicit Usings, Nullable enabled)
- **Persistence:** Entity Framework Core
- **Validation:** FluentValidation
- **Observability:** OpenTelemetry
- **Testing:** xUnit v3, FluentAssertions
- **DI/Scanning:** Microsoft.Extensions.DependencyInjection, Scrutor

### Architecture & Packages

The project is structured into functional layers following Clean Architecture principles:

- **Core Layer:**
  - `DotSharp.Primitives`: Domain-Driven Design building blocks (Entities, AggregateRoots, Domain Events, Auditing).
  - `DotSharp.Results`: Implementation of the Result pattern for functional error handling.
- **Application Layer:**
  - `DotSharp.Application`: Custom message bus implementation (Request/Notification) with pipeline behaviors.
  - `DotSharp.Application.Observability`: Telemetry behaviors for the application bus.
- **Persistence Layer:**
  - `DotSharp.Persistence.Abstractions`: Repository, Unit of Work, and Specification patterns.
  - `DotSharp.Persistence.EFCore`: EF Core implementation of persistence abstractions, including Outbox/Inbox support and Audit Logging.
- **Observability Layer:**
  - `DotSharp.Observability`: Core tracing and correlation abstractions.
  - `DotSharp.Observability.OpenTelemetry`: OpenTelemetry exporter and instrumentation extensions.

## Building and Running

Since this project consists of NuGet packages, "running" typically refers to executing the test suite or consuming the packages in an application.

- **Build solution:**
  ```bash
  dotnet build
  ```
- **Run all tests:**
  ```bash
  dotnet test
  ```
- **Restore packages:**
  ```bash
  dotnet restore
  ```

## Development Conventions

### Coding Standards
- **Implicit Usings & Nullable:** Both are enabled globally via `Directory.Build.props`.
- **Warnings as Errors:** The solution is configured to treat all warnings as errors (`<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`).
- **XML Documentation:** Publicly visible members should be documented. Documentation generation is enabled, but warning 1591 is suppressed for internal flexibility.
- **Result Pattern:** Avoid throwing exceptions for business logic failures. Use `Result` or `Result<T>` from `DotSharp.Results`.

### Domain-Driven Design (DDD)
- **Entities:** Inherit from `Entity<TKey>` for identified objects or `Entity` for join entities.
- **Aggregate Roots:** Inherit from `AggregateRoot<TKey>`.
- **Domain Events:** Use `IDomainEvent` and `AddDomainEvent()`. Use `[OutboxEvent]` for persistence.
- **Auditing:** Use auditable variants (e.g., `FullAuditableEntity<TKey>`) to track creation, modification, and deletion metadata.

### Messaging & Behaviors
- Use `ISender` for requests (1:1) and `IPublisher` for notifications (1:N).
- Cross-cutting concerns like validation and observability are implemented as `IRequestPipelineBehavior`.

### Persistence
- **Repository Pattern:** Use `IReadRepository<TEntity, TKey>` and `IRepository<TEntity, TKey>`.
- **Specifications:** Complex queries should be encapsulated in `Specification<TEntity>` classes.
- **Unit of Work:** Use `IUnitOfWork` to commit changes atomically.

### Testing
- **xUnit v3:** The project uses the latest xUnit version.
- **FluentAssertions:** Used for expressive test assertions.
- **Naming:** Follow the `ProjectName.Tests` structure mirroring the `src` directory.
