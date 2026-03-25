namespace DotSharp.Primitives.Events;

/// <summary>
/// Stable identifier for outbox events (avoids FullName coupling).
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class OutboxEventAttribute(string name, int version = 1) : Attribute
{
    /// <summary>
    /// Gets the stable name used to identify this event type in the outbox.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Gets the version of the event schema. Defaults to 1.
    /// </summary>
    public int Version { get; } = version;
}