# AGENTS.md — Instructions for AI Agents

## Project

DotSharp is a collection of reusable .NET 10 infrastructure packages for Clean Architecture & DDD. Open source, MIT license.

## Tech Stack

- .NET 10.0, C# 13, ImplicitUsings, Nullable enabled
- xUnit v3 (MTP), FluentAssertions
- Entity Framework Core, FluentValidation, OpenTelemetry, Scrutor

## Repository Structure

```
src/
├── Core/           → DotSharp.Primitives, DotSharp.Results
├── Application/    → DotSharp.Application, DotSharp.Application.Observability
├── Web/            → DotSharp.Web
├── Caching/        → DotSharp.Caching.Abstractions, .Memory, .Redis
├── Persistence/    → DotSharp.Persistence.Abstractions, .EFCore
├── Observability/  → DotSharp.Observability, .OpenTelemetry
tests/              → Mirrors src/ structure
```

## Conventions

### Build
- `dotnet build` — `TreatWarningsAsErrors=true`, `GenerateDocumentationFile=true`
- Package versions in `Directory.Packages.props` (CPM) — never hardcode in csproj
- csproj inherits from `Directory.Build.props` — do not duplicate TargetFramework, ImplicitUsings, Nullable

### Code
- Namespace: `DotSharp.{Layer}.{Subnamespace}` (e.g., `DotSharp.Caching.Abstractions`)
- All public types and methods require XML doc comments
- `readonly struct` for value types, `sealed record` for DTOs/config, `sealed class` for services
- Extension method classes are `static`, registration methods in `ServiceExtensions`
- File-scoped namespaces

### Tests
- xUnit v3 MTP pattern: csproj needs `OutputType=Exe`, `UseAppHost=true`, `GenerateTestingPlatformEntryPoint=false`, `TestingPlatformDotnetTestSupport=true`
- Do NOT include `Microsoft.NET.Test.Sdk` — conflicts with MTP
- Test class: `sealed`, file-scoped namespace, `[Fact]` for tests
- Assertions: FluentAssertions

### README
Every package must have a README.md covering: description, installation, setup, usage examples for all public types, design decisions, dependencies.

## Package Pattern

Each domain follows a consistent split:

```
src/{Domain}/DotSharp.{Domain}.Abstractions/  → interfaces, records, extensions
src/{Domain}/DotSharp.{Domain}.{Impl}/        → concrete implementation
tests/{Domain}/DotSharp.{Domain}.Abstractions.Tests/
tests/{Domain}/DotSharp.{Domain}.{Impl}.Tests/
```

Registration: `AddDotSharp{Name}()` extension on `IServiceCollection`. README in each package.

## Key Types Reference

| Type | Package | Role |
|---|---|---|
| `Result<T>`, `Error`, `ErrorCodes` | Results | Exception-less error handling |
| `Entity<TKey>`, `AggregateRoot<TKey>` | Primitives | DDD base classes |
| `ISender`, `IPublisher`, `IMessageBus` | Application | Custom message bus |
| `IReadRepository<T,TKey>`, `ISpecification<T>` | Persistence.Abstractions | Data access contracts |
| `IClock` | Primitives | Testable time abstraction |
| `ICacheService<T>`, `CacheKey` | Caching.Abstractions | Cache-aside pattern |
| `ResultToHttpFilter`, `IErrorHttpMapper` | Web | Automatic Result→HTTP mapping |
| `ICorrelationContext`, `ITraceContext` | Observability | Cross-cutting telemetry |

## .gitignore Policy

- `.atl/` and `openspec/` are gitignored (internal tooling, not for open source consumers)

## Branch Strategy

- `main` is protected — all changes via PRs
- Feature branches: `feat/{description}` or SDD chain branches
- Conventional commits: `feat(scope):`, `fix(scope):`, `docs(scope):`, `refactor(scope):`