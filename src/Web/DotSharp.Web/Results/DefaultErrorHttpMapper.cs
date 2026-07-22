using DotSharp.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DotSharp.Web.Results;

/// <summary>
/// Default mapping from <see cref="Error.Code"/> to HTTP status code + ProblemDetails.
/// Enriches ProblemDetails with correlation and trace identifiers when available.
/// </summary>
public sealed class DefaultErrorHttpMapper : IErrorHttpMapper
{
    private readonly IHttpContextAccessor? _httpContextAccessor;

    /// <summary>
    /// Creates a new <see cref="DefaultErrorHttpMapper"/> with optional HTTP context accessor
    /// for correlation/trace enrichment.
    /// </summary>
    public DefaultErrorHttpMapper(IHttpContextAccessor? httpContextAccessor = null)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc />
    public int MapStatusCode(Error error)
        => error.Code switch
        {
            ErrorCodes.NotFound => StatusCodes.Status404NotFound,
            ErrorCodes.Validation => StatusCodes.Status400BadRequest,
            ErrorCodes.Conflict => StatusCodes.Status409Conflict,
            ErrorCodes.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorCodes.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError,
        };

    /// <inheritdoc />
    public ProblemDetails MapProblemDetails(Error error, int statusCode)
    {
        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = error.Code,
            Detail = error.Message,
        };

        if (error.Metadata is not null)
        {
            problem.Extensions["metadata"] = error.Metadata;
        }

        if (error.Details is not null)
        {
            problem.Extensions["details"] = error.Details;
        }

        // Enrich with correlationId and traceId when available via HttpContext
        if (_httpContextAccessor?.HttpContext is { } httpContext)
        {
            if (httpContext.Items["X-Correlation-Id"] is string correlationId)
            {
                problem.Extensions["correlationId"] = correlationId;
            }

            if (httpContext.Items["X-Trace-Id"] is string traceId)
            {
                problem.Extensions["traceId"] = traceId;
            }
        }

        return problem;
    }
}
