namespace DotSharp.Results;

/// <summary>
/// Represents a single validation error detail for a property/field.
/// </summary>
/// <param name="Property">The name/path of the invalid property.</param>
/// <param name="Code">A machine-readable validation code (e.g., "NotEmpty", "GreaterThan").</param>
/// <param name="Message">A human-friendly message describing the validation failure.</param>
/// <param name="Values">Optional comparison or constraint values extracted from placeholders.</param>
public sealed record ValidationError(string Property, string Code, string Message, string[]? Values = null);
