namespace DotSharp.Results;

/// <summary>
/// Domain-friendly factory methods for common errors.
/// These helpers keep codes/messages consistent across the solution.
/// </summary>
public static class Errors
{
    /// <summary>
    /// Creates a not-found error.
    /// </summary>
    public static Error NotFound(string message) => new(ErrorCodes.NotFound, message);

    /// <summary>
    /// Creates a validation error.
    /// </summary>
    public static Error Validation(string message) => new(ErrorCodes.Validation, message);

    /// <summary>
    /// Creates a conflict error.
    /// </summary>
    public static Error Conflict(string message) => new(ErrorCodes.Conflict, message);

    /// <summary>
    /// Creates a forbidden error.
    /// </summary>
    public static Error Forbidden(string message) => new(ErrorCodes.Forbidden, message);

    /// <summary>
    /// Creates an unauthorized error.
    /// </summary>
    public static Error Unauthorized(string message) => new(ErrorCodes.Unauthorized, message);
}
