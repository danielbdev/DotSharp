using System.Diagnostics;

namespace DotSharp.Observability.Tracing;

/// <summary>
/// Provides access to the current trace and span identifiers from <see cref="Activity.Current"/>.
/// </summary>
public sealed class TraceContext : ITraceContext
{
    /// <inheritdoc />
    public string TraceId =>
        Activity.Current?.TraceId.ToString() ?? string.Empty;

    /// <inheritdoc />
    public string SpanId =>
        Activity.Current?.SpanId.ToString() ?? string.Empty;
}
