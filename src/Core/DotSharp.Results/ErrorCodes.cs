namespace DotSharp.Results;

/// <summary>
/// Defines stable error codes used across Domain/Application/Hosting.
/// Keep these values stable because they are mapped to HTTP status codes and may be relied upon by clients.
/// </summary>
public static class ErrorCodes
{
    /// <summary>
    /// Indicates request validation errors.
    /// </summary>
    public const string Validation = "validation_error";

    /// <summary>
    /// Indicates that a resource was not found.
    /// </summary>
    public const string NotFound = "not_found";

    /// <summary>
    /// Indicates a conflict with the current state.
    /// </summary>
    public const string Conflict = "conflict";

    /// <summary>
    /// Indicates that the user is authenticated but not allowed to perform the action.
    /// </summary>
    public const string Forbidden = "forbidden";

    /// <summary>
    /// Indicates that authentication is required or invalid.
    /// </summary>
    public const string Unauthorized = "unauthorized";

    /// <summary>
    /// Indicates an unexpected/unhandled error.
    /// </summary>
    public const string Unexpected = "unexpected_error";
}
