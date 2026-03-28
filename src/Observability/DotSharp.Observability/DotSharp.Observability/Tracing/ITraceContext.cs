namespace DotSharp.Observability.Tracing;

/// <summary>
/// Provides access to the current trace and span identifiers.
/// </summary>
public interface ITraceContext
{
    /// <summary>
    /// The current trace identifier.
    /// </summary>
    string TraceId { get; }

    /// <summary>
    /// The current span identifier.
    /// </summary>
    string SpanId { get; }
}
