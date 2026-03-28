using DotSharp.Observability.Tracing;
using OpenTelemetry.Trace;

namespace DotSharp.Observability.OpenTelemetry;

/// <summary>
/// Extension methods for integrating DotSharp instrumentation with OpenTelemetry.
/// </summary>
public static class TelemetryExtensions
{
    /// <summary>
    /// Adds DotSharp instrumentation to the <see cref="TracerProviderBuilder"/>.
    /// Registers the <see cref="DotSharpActivitySource"/> so that spans created by DotSharp
    /// components are captured by the OpenTelemetry SDK.
    /// </summary>
    /// <param name="builder">The tracer provider builder.</param>
    /// <returns>The same builder for chaining.</returns>
    public static TracerProviderBuilder AddDotSharpInstrumentation(this TracerProviderBuilder builder)
    {
        builder.AddSource(DotSharpActivitySource.Instance.Name);

        return builder;
    }
}
