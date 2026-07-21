using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace DotSharp.Web.Results;

/// <summary>
/// Catches unhandled exceptions and returns RFC 9457 <see cref="Microsoft.AspNetCore.Mvc.ProblemDetails"/> responses.
/// Full implementation will be provided in a subsequent work unit.
/// </summary>
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    /// <inheritdoc />
    public ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Placeholder — full implementation in follow-up work unit.
        return ValueTask.FromResult(false);
    }
}
