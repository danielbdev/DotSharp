# DotSharp.Caching.Redis

Distributed cache implementation for DotSharp — wraps `IDistributedCache` (StackExchange.Redis) behind the `ICacheService<T>` abstraction with pluggable serialization.

## Installation
```bash
dotnet add package DotSharp.Caching.Redis
```

## Setup

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Register the underlying Redis connection
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "dotsharp:";
});

// Register DotSharp caching layer on top
builder.Services.AddDotSharpCachingRedis();

var app = builder.Build();
// ...
```

`AddDotSharpCachingRedis()`:
- Registers `SystemTextJsonSerializer` as `ISerializer` (Singleton) — stateless, thread-safe.
- Registers `RedisCacheService<>` as `ICacheService<>` (Scoped) via open-generic DI.

**Important**: `AddDotSharpCachingRedis()` only registers the DotSharp layer. You must also register the underlying `IDistributedCache` implementation — typically via `AddStackExchangeRedisCache()`.

## What's included

### RedisCacheService\<T\>

Full implementation of `ICacheService<T>` backed by `IDistributedCache`. Values are serialized to byte arrays via `ISerializer` before storage and deserialized on retrieval.

```csharp
public sealed class ProductService(ICacheService<Product> cache)
{
    public async ValueTask<Product?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        // Distributed get — hits Redis, deserializes on hit
        return await cache.GetAsync($"product:{id}", ct);
    }

    public async ValueTask<Product?> GetOrFetchAsync(Guid id, CancellationToken ct)
    {
        // Get-or-set with distributed read-through
        return await cache.GetOrSetAsync(
            $"product:{id}",
            async token => await _db.Products.FindAsync([id], token),
            new CacheOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) },
            ct);
    }

    public async ValueTask InvalidateAsync(Guid id, CancellationToken ct)
    {
        // Remove from Redis — all nodes see the invalidation
        await cache.RemoveAsync($"product:{id}", ct);
        await _bus.PublishAsync(new ProductCacheInvalidated(id));
    }
}
```

### ISerializer — pluggable serialization

The serialization contract is an injectable interface, not a hardcoded implementation. The default is `SystemTextJsonSerializer`, but you can replace it with MessagePack, Protobuf, or any binary format.

```csharp
// Default: System.Text.Json (UTF-8)
builder.Services.AddDotSharpCachingRedis();

// Custom: MessagePack for smaller payloads
builder.Services.AddSingleton<ISerializer, MessagePackSerializer>();
builder.Services.AddDotSharpCachingRedis(); // ISerializer already registered — no-op
```

```csharp
public sealed class MessagePackSerializer : ISerializer
{
    public byte[] Serialize<T>(T value)
        => MessagePackSerializer.Serialize(value, MessagePackSerializerOptions.Standard);

    public T? Deserialize<T>(byte[]? bytes)
        => bytes is null ? default : MessagePackSerializer.Deserialize<T>(bytes, MessagePackSerializerOptions.Standard);
}
```

### CacheOptions mapping

`RedisCacheService<T>` maps the provider-agnostic `CacheOptions` to `DistributedCacheEntryOptions`:

| `CacheOptions` property | `DistributedCacheEntryOptions` mapping |
|---|---|
| `AbsoluteExpiration` | `DistributedCacheEntryOptions.AbsoluteExpiration` |
| `AbsoluteExpirationRelativeToNow` | `DistributedCacheEntryOptions.AbsoluteExpirationRelativeToNow` |
| `SlidingExpiration` | `DistributedCacheEntryOptions.SlidingExpiration` (emulated by the library) |

Unlike `MemoryCacheService`, `CacheItemPriority` is **not mapped** — Redis does not have an eviction priority concept at the API level.

### Distributed nature — no built-in stampede protection

Unlike `MemoryCacheService`, `RedisCacheService` does **not** provide per-key locking or stampede protection. Concurrent cache misses for the same key across multiple distributed nodes **may** result in multiple factory invocations.

```
Node A ──→ GetOrSetAsync("product:42")
  ├─ Redis GET → MISS
  ├─ Execute factory (DB query)
  └─ Redis SET → populate cache

Node B ──→ GetOrSetAsync("product:42")  ← simultaneous
  ├─ Redis GET → MISS (Node A hasn't set yet)
  ├─ Execute factory (DB query)  ← duplicate call
  └─ Redis SET → overwrite (last write wins)
```

**Why**: Distributed locking requires consensus (Redlock, etc.) which adds complexity, latency, and failure modes that belong in the consumer's architecture, not in a caching library. If your workload requires stampede protection at the distributed level, implement it externally — e.g., a distributed lock in your application service before calling `GetOrSetAsync`.

For single-node scenarios or workloads where duplicate factory calls are acceptable (idempotent reads), this is a non-issue.

## Design decisions

- **`ISerializer` in the Redis package (not Abstractions)** — serialization is strictly a distributed concern. In-memory caches store objects directly. Placing it in Abstractions would leak an implementation detail into the contract.
- **`SystemTextJsonSerializer` as default** — zero-dependency, allocation-efficient (`SerializeToUtf8Bytes`), thread-safe. Covers 90% of use cases without requiring MessagePack or Protobuf.
- **`ISerializer` as Singleton** — stateless contract. Serializer instances are configuration, not request-scoped resources.
- **`RedisCacheService<T>` as Scoped** — follows the same pattern as `MemoryCacheService<T>` and EFCore's `DbContext`. Consistent DI expectations across all `ICacheService<T>` implementations.
- **No value envelope** — `T` is serialized directly to byte array. Expiration is key metadata (SETEX), not payload. Cleaner migration between cache providers.
- **`RemoveAsync` uses `IDistributedCache.RemoveAsync`** — actually removes the entry from Redis. `RefreshAsync` only resets sliding expiration and does not invalidate.
- **`SlidingExpiration` is emulated** — Redis has no native sliding expiration. The library emulates it by resetting the TTL on each access. Documented in XML docs on `RedisCacheService<T>`.

## Dependencies

### NuGet packages
- `Microsoft.Extensions.Caching.StackExchangeRedis`

### DotSharp packages
- `DotSharp.Caching.Abstractions` — `ICacheService<T>`, `CacheKey`, `CacheOptions`
