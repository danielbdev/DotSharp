namespace DotSharp.Primitives.Clock;

/// <summary>
/// Provides an abstraction over system time to enable deterministic testing.
/// Use this instead of <see cref="DateTime.UtcNow"/> or <see cref="DateTimeOffset.UtcNow"/>
/// to make time-dependent logic testable.
/// </summary>
public interface IClock
{
    /// <summary>
    /// Gets the current date and time in Coordinated Universal Time (UTC).
    /// </summary>
    DateTimeOffset UtcNow { get; }
}
