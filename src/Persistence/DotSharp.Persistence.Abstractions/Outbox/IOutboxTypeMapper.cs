namespace DotSharp.Persistence.Abstractions.Outbox;

/// <summary>
/// Resolves a CLR type from an event name and version.
/// </summary>
public interface IOutboxTypeMapper
{
    /// <summary>
    /// Resolves the CLR type for the given event name and version.
    /// </summary>
    /// <param name="name">The event name.</param>
    /// <param name="version">The event version.</param>
    Type Resolve(string name, int version);
}
