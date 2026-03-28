namespace DotSharp.Results;

/// <summary>
/// Represents a failure as data (no HTTP concerns).
/// This is the single error contract used by Domain/Application and later translated to HTTP in Hosting.
/// </summary>
/// <param name="Code">A stable, machine-readable error code.</param>
/// <param name="Message">A human-friendly message describing the error.</param>
/// <param name="Metadata">Optional metadata (e.g., ids, entity names) for diagnostics/client usage.</param>
/// <param name="Details">Optional validation details (primarily for <see cref="ErrorCodes.Validation"/>).</param>
public sealed record Error(
    string Code,
    string Message,
    IReadOnlyDictionary<string, object?>? Metadata = null,
    IReadOnlyList<ValidationError>? Details = null)
{
    /// <summary>
    /// Returns a new <see cref="Error"/> with an added/replaced metadata entry.
    /// </summary>
    /// <param name="key">Metadata key.</param>
    /// <param name="value">Metadata value.</param>
    /// <returns>A new error instance with updated metadata.</returns>
    public Error With(string key, object? value)
    {
        Dictionary<string, object?>? dict = Metadata is null
            ? []
            : new Dictionary<string, object?>(Metadata);

        dict[key] = value;
        return this with { Metadata = dict };
    }

    /// <summary>
    /// Returns a new <see cref="Error"/> with validation details.
    /// </summary>
    /// <param name="details">Validation details.</param>
    /// <returns>A new error instance with details.</returns>
    public Error WithDetails(IReadOnlyList<ValidationError> details)
        => this with { Details = details };
}
