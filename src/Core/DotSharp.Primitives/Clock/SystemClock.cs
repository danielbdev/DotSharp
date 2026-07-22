namespace DotSharp.Primitives.Clock;

/// <summary>
/// Default implementation of <see cref="IClock"/> that delegates to <see cref="DateTimeOffset.UtcNow"/>.
/// </summary>
public sealed class SystemClock : IClock
{
    /// <inheritdoc />
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
