using DotSharp.Results;
using Microsoft.AspNetCore.Mvc;

namespace DotSharp.Web.Results;

/// <summary>
/// Maps <see cref="Error"/> (domain/application) into HTTP representation.
/// This layer is the only one that knows about HTTP.
/// </summary>
public interface IErrorHttpMapper
{
    /// <summary>
    /// Converts a domain/application error into an HTTP status code.
    /// </summary>
    int MapStatusCode(Error error);

    /// <summary>
    /// Converts a domain/application error into a <see cref="ProblemDetails"/> payload.
    /// </summary>
    ProblemDetails MapProblemDetails(Error error, int statusCode);
}
