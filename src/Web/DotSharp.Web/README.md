# DotSharp.Web

ASP.NET Core integration layer for DotSharp. Provides automatic `Result`/`Result<T>` → HTTP mapping via MVC filters, correlation context, global exception handling, validation filtering, pagination headers, and a base controller with lazy `IMessageBus`.

## Installation
```bash
dotnet add package DotSharp.Web
```

## Setup

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDotSharpWeb();

var app = builder.Build();

app.UseDotSharpExceptionHandling();   // Place early
app.UseDotSharpCorrelation();         // MUST be first in the pipeline

app.MapControllers();
app.Run();
```

## What's included

### Automatic Result → HTTP mapping

`ResultToHttpFilter` is an `IAsyncResultFilter` that automatically unwraps `Result` and `Result<T>` returned by controller actions. **No manual mapping needed** — just return the result directly.

```csharp
[Route("orders")]
public class OrdersController : DotSharpController
{
    [HttpPost]
    public Task<Result<Guid>> Create(CreateOrderCommand command, CancellationToken ct)
        => MessageBus.Send(command, ct);

    [HttpGet("{id:guid}")]
    public async Task<Result<Order>> GetById(Guid id)
    {
        Result<Order> result = await _service.GetByIdAsync(id);
        return result;
    }
}
```

**How it works:**

| Return type | Success | Failure |
|---|---|---|
| `Result` | `204 No Content` | `ProblemDetails` with mapped status |
| `Result<T>` | `200 OK` with `Value` | `ProblemDetails` with mapped status |

**Error code mapping:**

| `ErrorCode` | HTTP Status |
|---|---|
| `validation_error` | 400 Bad Request |
| `unauthorized` | 401 Unauthorized |
| `forbidden` | 403 Forbidden |
| `not_found` | 404 Not Found |
| `conflict` | 409 Conflict |
| `unexpected_error` | 500 Internal Server Error |

### Customizable error mapping

`IErrorHttpMapper` controls how `Error` is translated to HTTP. The default implementation maps `ErrorCode` to status codes as shown above. Replace it for custom behavior:

```csharp
public sealed class CustomErrorMapper : IErrorHttpMapper
{
    public int MapStatusCode(Error error) => error.Code switch
    {
        ErrorCodes.Validation => StatusCodes.Status422UnprocessableEntity,
        _ => StatusCodes.Status500InternalServerError
    };

    public ProblemDetails MapProblemDetails(Error error, int statusCode)
        => new()
        {
            Status = statusCode,
            Title = error.Code,
            Detail = error.Message
        };
}
```

```csharp
builder.Services.AddDotSharpWeb();
builder.Services.AddSingleton<IErrorHttpMapper, CustomErrorMapper>();
```

### Pagination headers

`PaginationHeaderFilter` automatically detects `PaginationResult<T>` responses and sets pagination headers while extracting the items array as the response body. Applied globally — no per-endpoint setup.

Response headers:

| Header | Example |
|---|---|
| `X-Pagination-PageNumber` | `3` |
| `X-Pagination-PageSize` | `20` |
| `X-Pagination-TotalCount` | `100` |
| `X-Pagination-TotalPages` | `5` |

### DotSharpController

Base controller with lazy `IMessageBus` access — resolves on first use from `HttpContext.RequestServices`:

```csharp
[Route("orders")]
public class OrdersController : DotSharpController
{
    [HttpPost]
    public Task<Result<Guid>> Create(CreateOrderCommand command, CancellationToken ct)
        => MessageBus.Send(command, ct);
}
```

### Correlation middleware

`CorrelationMiddleware` reads an existing `X-Correlation-Id` header or generates a new lowercase GUID, stores it in `HttpContext.Items`, and sets it on the response header.

```csharp
// Registered via AppExtensions
app.UseDotSharpCorrelation();
```

`ICorrelationContext` is available in any downstream service:

```csharp
public class OrdersAppService(IOrderRepository repo, ICorrelationContext correlation)
{
    public async Task<Result<Order>> GetByIdAsync(Guid id)
    {
        var correlationId = correlation.CorrelationId;
        // ...
    }
}
```

### Global exception handler

`GlobalExceptionHandler` catches unhandled exceptions and returns an RFC 9457 `ProblemDetails` response with HTTP 500, enriched with correlation and trace context.

```csharp
// Registered via AppExtensions — enabled by AddDotSharpWeb + UseDotSharpExceptionHandling
app.UseDotSharpExceptionHandling();
```

### Validation endpoint filter

`ValidationEndpointFilter` validates minimal API bound parameters using FluentValidation. Returns HTTP 400 with `ProblemDetails` on failure.

```csharp
builder.Services.AddScoped<IValidator<CreateOrderRequest>, CreateOrderRequestValidator>();

var orders = app.MapGroup("/api/orders")
    .AddEndpointFilter<ValidationEndpointFilter>();

orders.MapPost("/", (CreateOrderRequest request, IOrderService service) =>
    service.CreateAsync(request));
```

### Model binding error mapping

`ModelBindingErrorMapper.ToError()` converts `ModelStateDictionary` binding errors to a structured `Error` with `ValidationError` details.

```csharp
[ApiController]
[Route("api/orders")]
public class OrdersController(IOrderService service) : ControllerBase
{
    [HttpPost]
    public IActionResult Create([FromBody] CreateOrderRequest request)
    {
        if (!ModelState.IsValid)
        {
            Error error = ModelBindingErrorMapper.ToError(ModelState);
            return BadRequest(new DefaultErrorHttpMapper(httpContextAccessor)
                .MapProblemDetails(error, 400));
        }

        return Ok(service.Create(request));
    }
}
```

## Middleware order warning

`UseDotSharpCorrelation()` **MUST** be registered before `UseDotSharpExceptionHandling()`. The correlation ID must be available when the exception handler runs to include it in error responses.

```csharp
// ✅ Correct
app.UseDotSharpCorrelation();
app.UseDotSharpExceptionHandling();

// ❌ Incorrect
app.UseDotSharpExceptionHandling();
app.UseDotSharpCorrelation(); // too late — exception handler won't have correlation ID
```

## Design decisions

- **Automatic `IAsyncResultFilter` over explicit mapping** — `ResultToHttpFilter` unwraps results transparently. Controllers return `Task<Result<T>>` directly, no `.ToIResult()` boilerplate.
- **`IErrorHttpMapper` is injectable** — error-to-HTTP mapping is a DI-replaceable strategy, not a static utility. Teams customize it without touching filters.
- **`IMiddleware` for CorrelationMiddleware** — per-request DI activation, constructor injection, and isolated testability.
- **`IExceptionHandler` over middleware for exceptions** — uses the built-in ASP.NET Core exception handler pipeline, no custom try/catch wrapper.
- **`AppExtensions` for pipeline clarity** — `UseDotSharpCorrelation()` and `UseDotSharpExceptionHandling()` hide middleware internals from the consumer.
- **GUID `ToString("N")` format** — lowercase, compact, URL-safe correlation IDs without dashes.
- **Response header set BEFORE `await next()`** — if the pipeline throws, the correlation header is still present.
- **`ICorrelationContext` factory reads from `HttpContext.Items`** — per-request scoping via `IHttpContextAccessor`, no thread-static pitfalls.
- **Defensive `ITraceContext` guard** — `AddDotSharpWeb()` calls `AddDotSharpObservability()` only if `ITraceContext` is not already registered.
- **FluentValidation is a hard dependency** — DotSharp.Application already depends on it; explicit reference makes the validation filter discoverable.

## Dependencies

### NuGet packages
- `FluentValidation.DependencyInjectionExtensions`
- `Microsoft.Extensions.DependencyInjection.Abstractions`
- `Microsoft.AspNetCore.App` (framework reference)

### DotSharp packages
- `DotSharp.Results` — `Result`, `Result<T>`, `Error`, `Errors`, `ErrorCodes`
- `DotSharp.Observability` — `ICorrelationContext`, `ITraceContext`
- `DotSharp.Primitives` — `PaginationResult<T>`
- `DotSharp.Application` — `IMessageBus` (used by `DotSharpController`)
