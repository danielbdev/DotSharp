using DotSharp.Observability.Correlation;
using DotSharp.Observability.Tracing;
using DotSharp.Web.Results;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc;

namespace DotSharp.Web.Results;

/// <summary>
/// Catches unhandled exceptions and returns RFC 9457 <see cref="ProblemDetails"/>
/// responses enriched with correlation and trace context.
/// </summary>
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private const string Rfc9110InternalServerError = "https://tools.ietf.org/html/rfc9110#section-15.6.1";
    private const string DefaultTitle = "An error occurred while processing your request.";

    private readonly ICorrelationContext _correlation;
    private readonly ITraceContext _trace;

    /// <summary>
    /// Creates a new <see cref="GlobalExceptionHandler"/>.
    /// </summary>
    /// <param name="correlation">Current correlation context.</param>
    /// <param name="trace">Current trace context.</param>
    public GlobalExceptionHandler(ICorrelationContext correlation, ITraceContext trace)
    {
        _correlation = correlation;
        _trace = trace;
    }

    /// <inheritdoc />
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var pd = new ProblemDetails
        {
            Status = 500,
            Type = Rfc9110InternalServerError,
            Title = DefaultTitle,
            Detail = exception.Message,
        };

        pd.Extensions["correlationId"] = _correlation.CorrelationId;
        pd.Extensions["traceId"] = _trace.TraceId;

        httpContext.Response.StatusCode = 500;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync<ProblemDetails>(pd, cancellationToken);

        return true;
    }
}
