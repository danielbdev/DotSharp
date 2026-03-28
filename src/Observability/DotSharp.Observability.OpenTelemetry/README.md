# DotSharp.Observability.OpenTelemetry

OpenTelemetry integration for DotSharp — registers the DotSharp activity source with the OpenTelemetry SDK.

## Installation
```bash
dotnet add package DotSharp.Observability.OpenTelemetry
```

## What's included

### AddDotSharpInstrumentation

Registers the `DotSharpActivitySource` so that spans created by DotSharp components are captured by the OpenTelemetry SDK.
```csharp
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddDotSharpInstrumentation()
        .AddOtlpExporter());
```

## Design decisions

- This package exists to keep `DotSharp.Observability` free of OpenTelemetry dependencies — consumers who don't use OpenTelemetry don't need to install it.
- `AddDotSharpInstrumentation` only registers the `DotSharpActivitySource` — no additional configuration is imposed on the consumer's OpenTelemetry pipeline.
