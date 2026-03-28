using Microsoft.Extensions.Logging;
using DotSharp.Observability.Correlation;
using DotSharp.Observability.Tracing;

namespace DotSharp.Observability.Logging;

/// <summary>
/// Extension methods for enriching log scopes with observability context.
/// </summary>
public static class LoggerExtensions
{
    /// <summary>
    /// Begins a log scope enriched with correlation, trace and span identifiers.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="correlation">The correlation context.</param>
    /// <param name="trace">The trace context.</param>
    /// <returns>A disposable scope that removes the enrichment when disposed.</returns>
    public static IDisposable? BeginDotSharpScope(
        this ILogger logger,
        ICorrelationContext correlation,
        ITraceContext trace)
    {
        return logger.BeginScope(new Dictionary<string, object>
        {
            [CorrelationConstants.LogProperty] = correlation.CorrelationId,
            ["TraceId"] = trace.TraceId,
            ["SpanId"] = trace.SpanId
        });
    }
}
