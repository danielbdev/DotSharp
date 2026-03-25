namespace DotSharp.Primitives.Auditing;

/// <summary>
/// Interface that combines creation and update audit functionality.
/// Implementing classes must track when the entity was created and last modified, and by whom.
/// </summary>
public interface IAuditable : ICreationAudit, IModificationAudit { }