# DotSharp.Caching.Memory

In-process cache implementation for DotSharp — wraps `Microsoft.Extensions.Caching.Memory.IMemoryCache` behind the `ICacheService<T>` abstraction with built-in stampede protection.

## Installation
```bash
dotnet add package DotSharp.Caching.Memory
```

## Setup

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDotSharpCachingMemory();

var app = builder.Build();
// ...
```

`AddDotSharpCachingMemory()`:
- Calls `AddMemoryCache()` — registers `IMemoryCache` with the default configuration.
- Registers `MemoryCacheService<>` as `Scoped` via open-generic DI — any resolution of `ICacheService<T>` returns the in-memory implementation.

## What's included

### MemoryCacheService\<T\>

Full implementation of `ICacheService<T>` backed by `IMemoryCache`. Supports all five cache operations plus per-key stampede protection and full `CacheOptions` mapping.

```csharp
public sealed class ProductService(ICacheService<Product> cache)
{
    public async ValueTask<Product?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        // Simple get — returns null on miss
        return await cache.GetAsync($"product:{id}", ct);
    }

    public async ValueTask<Product?> GetOrFetchAsync(Guid id, CancellationToken ct)
    {
        // Get-or-set with stampede protection
        return await cache.GetOrSetAsync(
            $"product:{id}",
            async token =>
            {
                // Expensive operation — only one thread executes this on concurrent miss
                await Task.Delay(100, token); // simulated I/O
                return await _db.Products.FindAsync([id], token);
            },
            new CacheOptions { SlidingExpiration = TimeSpan.FromMinutes(10) },
            ct);
    }

    public async ValueTask WarmAsync(Product product, CancellationToken ct)
    {
        // Set with expiration
        await cache.SetAsync(
            $"product:{product.Id}",
            product,
            new CacheOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) },
            ct);
    }

    public async ValueTask InvalidateAsync(Guid id, CancellationToken ct)
        => await cache.RemoveAsync($"product:{id}", ct);
}
```

### Stampede protection

When multiple concurrent requests miss the cache for the same key, the naive approach invokes the expensive factory N times — a cache stampede. `MemoryCacheService<T>` prevents this with a **double-checked locking** pattern:

```
Request 1 ──→ GetOrSetAsync("product:42")
  ├─ TryGetValue → MISS
  ├─ Acquire per-key SemaphoreSlim from ConcurrentDictionary
  ├─ Double-check: TryGetValue → still MISS
  ├─ Execute factory (only this thread)
  ├─ Set result in _cache
  └─ Release semaphore, return value

Request 2 ──→ GetOrSetAsync("product:42")
  ├─ TryGetValue → MISS (race with Request 1)
  ├─ Wait on per-key SemaphoreSlim (Request 1 holds it)
  ├─ Request 1 releases → Request 2 enters
  ├─ Double-check: TryGetValue → HIT! (set by Request 1)
  └─ Return cached value — factory never invoked
```

Key design points:
- **Per-key isolation** via `ConcurrentDictionary<string, SemaphoreSlim>` — keys don't block each other.
- **Double-checked lock** — after acquiring the semaphore, the cache is queried again. If another thread populated it while we waited, we return the cached value without invoking the factory.
- **Lock cleanup** — semaphores are removed from the dictionary when no waiters remain, preventing unbounded growth.
- **Cancellation awareness** — `SemaphoreSlim.WaitAsync` respects the cancellation token. If the caller cancels while waiting, the semaphore is correctly released.

This is the same approach used by FusionCache and LazyCache — established, battle-tested, and allocation-light.

### CacheOptions mapping

`MemoryCacheService<T>` maps the provider-agnostic `CacheOptions` to `MemoryCacheEntryOptions`:

| `CacheOptions` property | `MemoryCacheEntryOptions` call |
|---|---|
| `AbsoluteExpiration` | `SetAbsoluteExpiration(DateTimeOffset)` |
| `AbsoluteExpirationRelativeToNow` | `SetAbsoluteExpiration(TimeSpan)` |
| `SlidingExpiration` | `SetSlidingExpiration(TimeSpan)` |
| `Priority` | Mapped to `Microsoft.Extensions.Caching.Memory.CacheItemPriority` |

When `CacheOptions` is `null`, entries use `MemoryCacheEntryOptions` defaults — no automatic expiration.

## Design decisions

- **`ConcurrentDictionary<string, SemaphoreSlim>` over global lock** — per-key isolation prevents head-of-line blocking. A slow factory for key A never delays key B retrievals.
- **`ConcurrentDictionary` over `Lazy<Task<T>>`** — `Lazy<Task<T>>` caches the task result permanently in the dictionary, leaking memory. `SemaphoreSlim` is disposable and cleaned up after use.
- **`IMemoryCache.Set` for cache population (not `CreateEntry`)** — simpler API, same behavior. `CreateEntry` is needed only when you need the `post-eviction callback` registration, which we don't use.
- **Scoped lifetime** — follows EFCore's `DbContext` registration pattern. Per-request isolation matches typical web workloads. Singleton would require thread-safe factories that could leak across requests.
- **Explicit `CacheItemPriority` enum in Abstractions** — avoids coupling consumers to `Microsoft.Extensions.Caching.Memory`. The Memory implementation maps between the two enums internally.
- **No async void in `SetAsync`/`RemoveAsync`** — `IMemoryCache.Set` and `Remove` are synchronous. We wrap them in `ValueTask.CompletedTask` for API consistency, not to hide async work.

## Dependencies

### NuGet packages
- `Microsoft.Extensions.Caching.Memory`

### DotSharp packages
- `DotSharp.Caching.Abstractions` — `ICacheService<T>`, `CacheKey`, `CacheOptions`
