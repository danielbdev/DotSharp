# DotSharp.Results

Result pattern implementation for .NET — model success and failure as data, not exceptions.

## Installation
```bash
dotnet add package DotSharp.Results
```

## What's included

### Result and Result\<T\>
```csharp
// Operation with no return value
Result result = Result.Success();
Result result = Result.Failure(Errors.NotFound("Order not found."));

// Operation with a return value
Result<Guid> result = Result<Guid>.Success(orderId);
Result<Guid> result = Result<Guid>.Failure(Errors.NotFound("Order not found."));
```

Implicit operators allow returning values directly:
```csharp
public Result<Guid> CreateOrder(string customerName)
{
    if (string.IsNullOrEmpty(customerName))
        return Errors.Validation("Customer name is required.");

    return Guid.NewGuid();
}
```

### Error

Represents a failure as data with no HTTP concerns.
```csharp
var error = new Error(
    Code: ErrorCodes.NotFound,
    Message: "Order not found.",
    Metadata: new Dictionary<string, object?> { ["orderId"] = orderId }
);

// Add metadata fluently
error = error.With("orderId", orderId);

// Add validation details
error = error.WithDetails(validationErrors);
```

### Errors factory

Domain-friendly helper methods for common errors:
```csharp
Errors.NotFound("Order not found.");
Errors.Validation("Invalid request.");
Errors.Conflict("Order already exists.");
Errors.Forbidden("You are not allowed to perform this action.");
Errors.Unauthorized("Authentication is required.");
Errors.Unexpected("An unexpected error occurred.");
```

### ErrorCodes

Stable, machine-readable codes for mapping to HTTP status codes or client responses:

| Code | Constant |
|------|----------|
| `validation_error` | `ErrorCodes.Validation` |
| `not_found` | `ErrorCodes.NotFound` |
| `conflict` | `ErrorCodes.Conflict` |
| `forbidden` | `ErrorCodes.Forbidden` |
| `unauthorized` | `ErrorCodes.Unauthorized` |
| `unexpected_error` | `ErrorCodes.Unexpected` |

### ErrorMessages

Predefined human-readable messages for common validation scenarios:
```csharp
string message = string.Format(ErrorMessages.NotEmpty, "CustomerName");
// "The field 'CustomerName' is required."

string message = string.Format(ErrorMessages.MinimumLength, "CustomerName", 3);
// "The field 'CustomerName' must have a minimum length of '3' characters."
```

### ValidationError

Represents a single field-level validation error:
```csharp
var validationError = new ValidationError(
    Property: "CustomerName",
    Code: "NotEmpty",
    Message: "The field 'CustomerName' is required."
);
```

## Handling results
```csharp
Result<Order> result = await orderService.GetByIdAsync(orderId);

if (result.IsFailure)
{
    // Handle failure
    Console.WriteLine(result.Error?.Message);
    return;
}

// Use the value safely
Order order = result.Value;
```

## Design decisions

- `Result` and `Result<T>` are `readonly struct` to avoid heap allocations on hot paths.
- Implicit operators allow returning `Error` or `T` directly where `Result<T>` is expected, reducing boilerplate.
- `ErrorCodes` are kept stable and machine-readable — they are meant to be mapped to HTTP status codes or client error contracts in the hosting layer.
- `ErrorMessages` are intentionally separate from `ErrorCodes` to allow localization in the future.
- `Errors` factory centralizes error creation to keep codes and messages consistent across the solution.
- `Error` has no HTTP concerns — translation to `ProblemDetails` or similar happens in the hosting/presentation layer.
```
