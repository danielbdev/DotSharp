namespace DotSharp.Observability.Correlation;

/// <summary>
/// Default implementation of <see cref="ICorrelationContext"/>.
/// </summary>
/// <param name="correlationId">The correlation identifier for the current operation.</param>
public sealed class CorrelationContext(string correlationId) : ICorrelationContext
{
    /// <inheritdoc />
    public string CorrelationId { get; } = correlationId;
}
