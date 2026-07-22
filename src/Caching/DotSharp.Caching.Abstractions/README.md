# DotSharp.Caching.Abstractions

Provider-agnostic caching contracts for .NET — generic cache service, structured key builder, Result pattern bridge, and specification-driven cache keys.

## Installation
```bash
dotnet add package DotSharp.Caching.Abstractions
```

## What's included

### ICacheService\<T\>

The core cache abstraction — five methods that every caching implementation must provide. Cache misses return `null` rather than throwing or wrapping in `Result`. Use the `GetOrSetResultAsync` extension for a Result-pattern bridge.

```csharp
public sealed class ProductService(ICacheService<Product> cache)
{
    public async ValueTask<Product?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        // Simple get — null on cache miss
        return await cache.GetAsync($"product:{id}", ct);
    }

    public async ValueTask<Product?> GetOrCreateAsync(Guid id, CancellationToken ct)
    {
        // Get-or-set with automatic population on miss
        return await cache.GetOrSetAsync(
            $"product:{id}",
            async token => await _db.Products.FindAsync([id], token),
            options: new CacheOptions { SlidingExpiration = TimeSpan.FromMinutes(10) },
            ct);
    }

    public async ValueTask InvalidateAsync(Guid id, CancellationToken ct)
        => await cache.RemoveAsync($"product:{id}", ct);

    public async ValueTask<bool> IsCachedAsync(Guid id, CancellationToken ct)
        => await cache.ExistsAsync($"product:{id}", ct);
}
```

The non-generic `ICacheService` (extending `ICacheService<object>`) is available for dynamic scenarios where the value type is not known at compile time.

### CacheKey — structured key builder

`CacheKey` is a `readonly struct` with implicit conversion from `string` and a fluent builder that produces deterministic keys in the format `prefix:TypeName:seg1:seg2`.

```csharp
// Implicit from string — ad-hoc keys
ICacheService<Product> cache = ...;
await cache.GetAsync("product:42", ct);

// Fluent builder — composable from DI settings + request data
CacheKey key = CacheKey.For<Product>()
    .WithPrefix("hot")          // optional prefix
    .WithSegment("42")          // entity id
    .WithSegment("active")      // filter state
    .Build();
// → "hot:Product:42:active"

// Shorthand with multiple segments
CacheKey key = CacheKey.For<Order>()
    .WithPrefix("region:us")
    .WithSegments("pending", "v3")
    .Build();
// → "region:us:Order:pending:v3"
```

The builder is composable — construct the key from configuration prefixes and runtime identifiers without string concatenation.

### CacheResultExtensions — Result bridge

Bridges `ICacheService<T>` to the `Result<T>` pattern used throughout DotSharp. The extension wraps null factory results as `Errors.NotFound`, making cache operations composable with Result-based command handlers.

```csharp
public sealed class GetOrderHandler(ICacheService<Order> cache)
{
    public async ValueTask<Result<Order>> Handle(Guid id, CancellationToken ct)
    {
        return await cache.GetOrSetResultAsync(
            $"order:{id}",
            async token => await _db.Orders.FindAsync([id], token),
            options: new CacheOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) },
            ct);
        // Cache hit → Result<Order>.Success(value)
        // Cache miss + factory returns null → Result<Order>.Failure(Errors.NotFound(...))
    }
}
```

### SpecificationCacheExtensions — auto-key from specs

Generates deterministic cache keys from `ISpecification<T>` instances by hashing the criteria expression tree via SHA256. Import the `DotSharp.Caching.Abstractions.Specifications` namespace to enable this feature — it is opt-in to avoid pulling `DotSharp.Persistence.Abstractions` into every caching consumer.

```csharp
using DotSharp.Caching.Abstractions.Specifications;

var spec = new ActiveProductsByCategorySpec("electronics", page: 1, size: 20);
var key = spec.ToCacheKey();
// → 16-char Base64 string (e.g., "XyZ8aB3cD4eF5gH6")

// Use as a cache-aside key
var products = await cache.GetOrSetAsync(
    spec.ToCacheKey(),
    async token => await repository.ListAsync(spec, token),
    options: new CacheOptions { SlidingExpiration = TimeSpan.FromMinutes(5) },
    ct);
```

The key is derived from `Criteria.ToString()` and all `OrderExpressions` — changing the criteria expression or ordering produces a different key, ensuring cache correctness automatically.

### CacheOptions — expiration and priority

A `sealed record` controlling entry lifetime and eviction priority. Provider-agnostic — implementations map this to their native options type.

```csharp
// Expire at a specific point in time
var options = new CacheOptions
{
    AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(1)
};

// Expire after a duration
var options = new CacheOptions
{
    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
};

// Sliding expiration — reset on each access
var options = new CacheOptions
{
    SlidingExpiration = TimeSpan.FromMinutes(5)
};

// High-priority entry — evicted after normal ones
var options = new CacheOptions
{
    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24),
    Priority = CacheItemPriority.High
};
```

`CacheItemPriority` enum values: `Low`, `Normal` (default), `High`, `NeverRemove`.

### Service registration

`AddDotSharpCachingAbstractions()` is an extension point for future abstraction-level registrations. Concrete `ICacheService<T>` implementations are registered by the Memory or Redis packages — see their READMEs for setup.

```csharp
services.AddDotSharpCachingAbstractions(); // optional — extension point
```

## Design decisions

- **Generic-on-interface (`ICacheService<T>`) over generic-on-method** — open-generic DI (`typeof(ICacheService<>)`) maps to one implementation, following the `IRepository<TAggregate, TKey>` pattern. No per-call type parameters needed.
- **`null` for cache miss over `Result`** — `null` is operational, not a domain error. The `GetOrSetResultAsync` extension bridges to `Result<T>` when needed, but the core API stays clean and allocation-free.
- **`CacheKey` as `readonly struct`** — value semantics, cheap copy, implicit from string. Follows `Result` and `Error` struct patterns. No heap allocation on the hot path.
- **Fluent builder over static factory** — composable from DI settings (prefix) and request data (segments) without string concatenation or format strings.
- **`SpecificationCacheExtensions` in a separate namespace** — opt-in dependency on `DotSharp.Persistence.Abstractions`. Caching consumers that don't use specifications never pull the persistence dependency.
- **`Expression.ToString()` for cache key hashing** — deterministic per expression tree. SHA256 avoids giant keys; Base64 truncation keeps keys compact. No reflection-based property dump that can't handle complex criteria.
- **`CacheOptions` as `sealed record` over class** — immutable-by-default with `init` properties. Records print cleanly in logs. No mutable state surprises across async boundaries.

## Dependencies

### NuGet packages
- `Microsoft.Extensions.DependencyInjection.Abstractions`

### DotSharp packages
- `DotSharp.Results` — `Result<T>`, `Errors`, `ErrorCodes`
- `DotSharp.Persistence.Abstractions` — `ISpecification<T>` (for `SpecificationCacheExtensions`)
