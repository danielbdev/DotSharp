# DotSharp.Observability

Core observability primitives for .NET — correlation, tracing, and structured logging enrichment.

## Installation
```bash
dotnet add package DotSharp.Observability
```

## What's included

### Correlation

Propagate a correlation identifier across operations for end-to-end traceability.

Use `CorrelationConstants` for consistent header and log property names:
```csharp
CorrelationConstants.HeaderName   // "X-Correlation-Id"
CorrelationConstants.LogProperty  // "CorrelationId"
```

Implement `ICorrelationContext` in your infrastructure layer:
```csharp
services.AddScoped<ICorrelationContext>(_ => new CorrelationContext(Guid.NewGuid().ToString()));
```

### Tracing

Access the current trace and span identifiers from `Activity.Current`:
```csharp
public sealed class MyService(ITraceContext trace)
{
    public void Execute()
    {
        Console.WriteLine(trace.TraceId);
        Console.WriteLine(trace.SpanId);
    }
}
```

Create spans using the shared `DotSharpActivitySource`:
```csharp
using Activity? activity = DotSharpActivitySource.Instance.StartActivity("MyOperation");
```

### Logging enrichment

Enrich all log entries within a scope with `CorrelationId`, `TraceId` and `SpanId`:
```csharp
public sealed class OrderService(
    ILogger<OrderService> logger,
    ICorrelationContext correlation,
    ITraceContext trace)
{
    public async Task CreateAsync(CreateOrderCommand command)
    {
        using (logger.BeginDotSharpScope(correlation, trace))
        {
            logger.LogInformation("Creating order for {CustomerName}", command.CustomerName);
            logger.LogInformation("Order created successfully");
        }
    }
}
```

### Registration
```csharp
services.AddDotSharpObservability();
```

Registers `ITraceContext` as a singleton. `ICorrelationContext` must be registered separately based on your infrastructure.

## Design decisions

- `ICorrelationContext` is intentionally left without a default implementation — the registration strategy depends on the consumer's infrastructure (HTTP, worker, gRPC, etc.).
- `TraceContext` reads from `Activity.Current` — outside an active span, `TraceId` and `SpanId` return empty strings.
- `DotSharpActivitySource` uses the name `"DotSharp"` — register it in your diagnostics pipeline to capture spans created by DotSharp components.
- `CorrelationConstants` centralizes header and log property names to avoid string literals scattered across the codebase.
