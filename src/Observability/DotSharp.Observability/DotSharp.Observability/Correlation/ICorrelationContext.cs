namespace DotSharp.Observability.Correlation;

/// <summary>
/// Provides access to the correlation identifier for the current operation.
/// </summary>
public interface ICorrelationContext
{
    /// <summary>
    /// The correlation identifier for the current operation.
    /// </summary>
    string CorrelationId { get; }
}
