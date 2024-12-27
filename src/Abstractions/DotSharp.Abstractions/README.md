# DotSharp.Abstractions

This package provides core abstractions for data access and entity management, including support for repositories, query specifications, and user context services. It promotes clean, maintainable code by offering standardized patterns for CRUD operations, entity query filtering, and efficient data retrieval.

## Compatibility
- .NET 8

## Dependencies
- DotSharp.Primitives

## Features
- **CRUD Operations**: Standardized interfaces for creating, reading, updating, and deleting entities.
- **Query Specifications**: Define flexible query criteria and options for filtering, sorting, and paginating entities.
- **User Context**: Access to current user details and claims through the ICurrentUserService.
- **Entity Tracking**: Enable or disable entity tracking for performance optimization in queries.
- **Caching**: Built-in support for caching query results to improve performance.
- **Pagination**: Efficient handling of large datasets by supporting paging through query specifications.

## Components

### Interfaces
- **ICreateRepository<TEntity>**: Interface for adding entities to the repository.
- **IUpdateRepository<TEntity>**: Interface for updating or upserting entities.
- **IDeleteRepository<TEntity, TKey>**: Interface for deleting entities or bulk deletions based on criteria.
- **IReadRepository<TEntity, TKey>**: Interface for reading entities with filtering, ordering, and pagination.
- **IRepository<TEntity, TKey>**: Combines ICreateRepository, IUpdateRepository, IDeleteRepository, and IReadRepository for full CRUD support.
- **ISpecification<TEntity>**: Defines query criteria and options such as filtering, sorting, and caching. It includes properties like IsPagingEnabled, OrderBy, IsCacheEnabled, and more.
- **ICurrentUserService**: Interface for retrieving current user information, including username, email, claims, and token.

## Installation

To use this package, simply install it via NuGet in your project:

```bash
Install-Package DotSharp.Abtractions
```

## Contributing
Feel free to submit issues, feature requests, or pull requests.

## License
This package is licensed under the MIT License. See LICENSE for more details.