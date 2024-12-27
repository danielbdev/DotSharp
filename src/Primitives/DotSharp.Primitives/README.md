# DotSharp.Primitives

This package provides an infrastructure for auditing entity changes (creation, update, and deletion) and managing domain events. It allows tracking changes to entities and supports event-driven architectures by enabling domain events

## Compatibility
- .NET 8

## Dependencies
- No Dependencies

## Features

- **Audit Tracking**: Track when an entity is created, updated, or deleted.
- **Domain Events**: Manage domain events associated with entities and enable event-driven architecture.
- **Audit Log**: Record detailed logs of changes, including the old and new values, action type, and the user who made the changes.
- **Paginated Results**: Handle paginated results for entities efficiently.

## Components

### Interfaces

- **ICreationAudit**: Tracks the creation details (timestamp and user).
- **IUpdateAudit**: Tracks the last modification details (timestamp and user).
- **IDeletionAudit**: Tracks the deletion details (timestamp, user, and deleted status).
- **IAuditableEntity**: Combines `ICreationAudit` and `IUpdateAudit` for entities requiring both creation and update tracking.
- **IFullAuditableEntity**: Extends `IAuditableEntity` to include deletion tracking (`IDeletionAudit`).

### Base Classes

- **AuditableEntity**: A base class implementing `IAuditableEntity` to handle creation and update audits.
- **FullAuditableEntity**: Extends `AuditableEntity` and implements `IFullAuditableEntity` to support deletion auditing.
- **Entity**: A base class for entities that supports domain events.
- **PaginatedResult<TEntity>**: Represents paginated results for entities, providing an easy way to return large datasets.

### Audit Log

The **AuditLog** class records the changes made to entities. It stores information like:
- Entity name
- Entity ID
- Action type (Create, Update, Delete)
- Old and new values
- User who made the change
- Timestamp of the change

### Domain Events

- **DomainEvent**: A base class for domain events, implementing `INotification` to integrate with event-driven architectures.
- **IDomainEvent**: Interface for entities that manage domain events, providing methods to raise, remove, and clear events.

## Installation

To use this package, simply install it via NuGet in your project:

```bash
Install-Package DotSharp.Primitives
```

## Usage

### Example: Auditing Entity Changes

```cs
public class Product : FullAuditableEntity
{
    public string Name { get; protected set; }
    public string Code { get; protected set; }

    protected Product(string name, string code)
    {
        Name = name;
        Code = code;
    }

    public static Product Create(string name, string code)
    {
        return new Product(name, code);
    }

    public void Update(string name, string code)
    {
        Name = name;
        Code = code;
    }
}

```

### Example: Domain Event Handling

```cs
public class ProductCreatedDomainEvent : DomainEvent
{
    public Product Product { get; }

    public UserCreatedEvent(Product product)
    {
        Product = product;
    }
}

public class Product : FullAuditableEntity
{
    public string Name { get; protected set; }
    public string Code { get; protected set; }

    protected Product(string name, string code)
    {
        Name = name;
        Code = code;
    }

    public static Product Create(string name, string code)
    {
        Product product = new Product(name, code);

        product.RaiseDomainEvent(new ProductCreatedDomainEvent(product));

        return product;
    }

    public void Update(string name, string code)
    {
        Name = name;
        Code = code;
    }
}

```

## Contributing
Feel free to submit issues, feature requests, or pull requests.

## License
This package is licensed under the MIT License. See LICENSE for more details.