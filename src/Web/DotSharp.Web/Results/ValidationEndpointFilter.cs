using Microsoft.AspNetCore.Http;

namespace DotSharp.Web.Results;

/// <summary>
/// Validates minimal API request parameters using FluentValidation.
/// Full implementation will be provided in a subsequent work unit.
/// </summary>
public sealed class ValidationEndpointFilter : IEndpointFilter
{
    /// <inheritdoc />
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        // Placeholder — full implementation in follow-up work unit.
        return await next(context);
    }
}
