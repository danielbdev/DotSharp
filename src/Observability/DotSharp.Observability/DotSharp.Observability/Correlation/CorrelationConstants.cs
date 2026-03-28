namespace DotSharp.Observability.Correlation;

/// <summary>
/// Constants for correlation header and log property names.
/// </summary>
public static class CorrelationConstants
{
    /// <summary>
    /// The HTTP header name used to propagate the correlation identifier.
    /// </summary>
    public const string HeaderName = "X-Correlation-Id";

    /// <summary>
    /// The log property key used to enrich log entries with the correlation identifier.
    /// </summary>
    public const string LogProperty = "CorrelationId";
}
