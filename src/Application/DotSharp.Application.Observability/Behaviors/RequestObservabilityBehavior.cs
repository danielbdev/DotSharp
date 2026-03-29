using System.Diagnostics;
using DotSharp.Application.Abstractions.Behaviors;
using DotSharp.Observability.Correlation;
using DotSharp.Observability.Logging;
using DotSharp.Observability.Tracing;
using Microsoft.Extensions.Logging;

namespace DotSharp.Application.Observability.Behaviors;

/// <summary>
/// Pipeline behavior that wraps request handling with an activity span and enriched log scope.
/// </summary>
public sealed class RequestObservabilityBehavior<TRequest, TResponse>(
    ILogger<RequestObservabilityBehavior<TRequest, TResponse>> logger,
    ICorrelationContext correlation,
    ITraceContext trace) : IRequestPipelineBehavior<TRequest, TResponse>
{
    /// <inheritdoc />
    public async Task<TResponse> Handle(TRequest request, Func<Task<TResponse>> next, CancellationToken cancellationToken)
    {
        using Activity? activity = DotSharpActivitySource.Instance.StartActivity(typeof(TRequest).Name);

        using (logger.BeginDotSharpScope(correlation, trace))
        {
            logger.LogInformation("Handling {Request}", typeof(TRequest).Name);
            TResponse response = await next();
            logger.LogInformation("Handled {Request}", typeof(TRequest).Name);
            return response;
        }
    }
}
