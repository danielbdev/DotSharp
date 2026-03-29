using DotSharp.Application.Abstractions.Behaviors;
using DotSharp.Application.Abstractions.Messaging;
using DotSharp.Observability.Correlation;
using DotSharp.Observability.Logging;
using DotSharp.Observability.Tracing;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace DotSharp.Application.Observability.Behaviors;

/// <summary>
/// Pipeline behavior that wraps notification handling with an activity span and enriched log scope.
/// </summary>
public sealed class NotificationObservabilityBehavior<TNotification>(
    ILogger<NotificationObservabilityBehavior<TNotification>> logger,
    ICorrelationContext correlation,
    ITraceContext trace) : INotificationPipelineBehavior<TNotification>
    where TNotification : INotification
{
    /// <inheritdoc />
    public async Task Handle(TNotification notification, Func<Task> next, CancellationToken cancellationToken)
    {
        using Activity? activity = DotSharpActivitySource.Instance.StartActivity(typeof(TNotification).Name);

        using (logger.BeginDotSharpScope(correlation, trace))
        {
            logger.LogInformation("Publishing {Notification}", typeof(TNotification).Name);
            await next();
            logger.LogInformation("Published {Notification}", typeof(TNotification).Name);
        }
    }
}
