using DotSharp.Results;
using Microsoft.AspNetCore.Mvc;

namespace DotSharp.Web.Results;

/// <summary>
/// Creates RFC 9457 <see cref="ProblemDetails"/> responses from <see cref="Error"/>
/// and <see cref="Exception"/> instances.
/// </summary>
public static class ProblemDetailsFactory
{
    private static readonly HashSet<string> ReservedExtensionKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "errors",
        "correlationId",
        "traceId",
    };

    /// <summary>
    /// Creates a <see cref="ProblemDetails"/> from an <see cref="Error"/> with
    /// optional correlation and trace enrichment.
    /// </summary>
    /// <param name="error">The domain error.</param>
    /// <param name="correlationId">Optional correlation identifier for the request.</param>
    /// <param name="traceId">Optional trace identifier for the request.</param>
    /// <returns>A <see cref="ProblemDetails"/> with the error detail and enriched extensions.</returns>
    public static ProblemDetails Create(Error error, string? correlationId, string? traceId)
    {
        var problemDetails = new ProblemDetails
        {
            Detail = error.Message,
        };

        if (error.Code == ErrorCodes.Validation && error.Details is not null)
        {
            problemDetails.Extensions["errors"] = error.Details;
        }

        if (correlationId is not null)
        {
            problemDetails.Extensions["correlationId"] = correlationId;
        }

        if (traceId is not null)
        {
            problemDetails.Extensions["traceId"] = traceId;
        }

        if (error.Metadata is not null)
        {
            foreach (var (key, value) in error.Metadata)
            {
                if (!ReservedExtensionKeys.Contains(key))
                {
                    problemDetails.Extensions[key] = value;
                }
            }
        }

        return problemDetails;
    }

    /// <summary>
    /// Creates a <see cref="ProblemDetails"/> from an <see cref="Exception"/> with
    /// optional correlation and trace enrichment.
    /// </summary>
    /// <param name="exception">The caught exception.</param>
    /// <param name="correlationId">Optional correlation identifier for the request.</param>
    /// <param name="traceId">Optional trace identifier for the request.</param>
    /// <returns>A <see cref="ProblemDetails"/> with the exception message and enriched extensions.</returns>
    public static ProblemDetails Create(Exception exception, string? correlationId, string? traceId)
    {
        var problemDetails = new ProblemDetails
        {
            Detail = exception.Message,
        };

        if (correlationId is not null)
        {
            problemDetails.Extensions["correlationId"] = correlationId;
        }

        if (traceId is not null)
        {
            problemDetails.Extensions["traceId"] = traceId;
        }

        return problemDetails;
    }
}
