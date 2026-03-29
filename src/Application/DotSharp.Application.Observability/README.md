# DotSharp.Application.Observability

Observability behaviors for DotSharp.Application — wraps request and notification handling with activity spans and enriched log scopes.

## Installation
```bash
dotnet add package DotSharp.Application.Observability
```

## What's included

### RequestObservabilityBehavior

Wraps each request handler execution with an `Activity` span and a log scope enriched with `CorrelationId`, `TraceId` and `SpanId`.

### NotificationObservabilityBehavior

Wraps each notification handler execution with an `Activity` span and a log scope enriched with `CorrelationId`, `TraceId` and `SpanId`.

### Registration
```csharp
builder.Services.AddDotSharpObservability();
builder.Services.AddDotSharpApplication(typeof(CreateOrderCommandHandler).Assembly);
builder.Services.AddDotSharpApplicationObservability();
```

Requires `ICorrelationContext` and `ITraceContext` to be registered. `AddDotSharpObservability()` registers `ITraceContext` — `ICorrelationContext` must be registered separately based on your infrastructure.

## Design decisions

- Observability behaviors are intentionally separate from `DotSharp.Application` — consumers who don't need observability don't take the dependency.
- `ICorrelationContext` and `ITraceContext` must be registered separately based on your infrastructure.
- Each request and notification gets its own `Activity` span named after the request or notification type.
