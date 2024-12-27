# DotSharp.Data.Caching

A library providing a robust memory caching service for .NET applications. This package simplifies the process of caching entities in memory and managing cache entries effectively.

## Compatibility
- .NET 8

## Dependencies
- No Dependencies

## Features

- **Entity-Based Caching**: Cache entities with unique keys generated based on type, request, and parameters.
- **Customizable Expiration**: Define expiration policies for cached entries.
- **Cache Management**: Clear all cached entries or target specific entity types.
- **Thread-Safe Operations**: Manage cache keys safely in concurrent environments.



## Installation

To use this package, simply install it via NuGet in your project:

```bash
Install-Package DotSharp.Data.Caching
```

## Usage

### Register the Service

```cs
builder.Services.AddMemoryCacheService();
```

### Caching an Entity

```cs
public class ProductService
{
    private readonly IMemoryCacheService _cacheService;

    public ProductService(IMemoryCacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task<Product> GetProductAsync(Request request)
    {
        string cacheKey = _cacheService.GenerateCacheKey<Product>(request);

        return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
        {
            return await FetchProductFromDatabaseAsync(productId);
        }, TimeSpan.FromMinutes(30));
    }
}
```

### Clearing the Cache

```cs
_cacheService.ClearCache();
```

### Clear Entries for a Specific Entity

```cs
_cacheService.ClearCacheByEntity<Product>();
```

### Generating Cache Keys

```cs
string cacheKey = _cacheService.GenerateCacheKey<Product>(request, "extraParam1", "extraParam2");
```

## Contributing
Feel free to submit issues, feature requests, or pull requests.

## License
This package is licensed under the MIT License. See LICENSE for more details.