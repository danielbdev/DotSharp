using Microsoft.AspNetCore.Http;

namespace DotSharp.Web.Correlation;

/// <summary>
/// Reads or generates a correlation identifier per request, stores it in
/// <see cref="HttpContext.Items"/>, and sets the response header before the
/// pipeline continues.
/// </summary>
/// <remarks>
/// Implements <see cref="IMiddleware"/> for constructor-injection compatibility.
/// Activated per-request from the DI container and testable in isolation.
/// </remarks>
public sealed class CorrelationMiddleware : IMiddleware
{
    private const string HeaderName = "X-Correlation-Id";
    private const string ItemKey = "X-Correlation-Id";

    /// <inheritdoc />
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var correlationId = context.Request.Headers[HeaderName].FirstOrDefault()
                            ?? Guid.NewGuid().ToString("N");

        context.Items[ItemKey] = correlationId;
        context.Response.Headers[HeaderName] = correlationId;

        await next(context);
    }
}
