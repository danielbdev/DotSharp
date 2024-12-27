using DotSharp.Primitives.Audit.Created;
using DotSharp.Primitives.Audit.Updated;

namespace DotSharp.Primitives.Audit.Auditable;

/// <summary>
/// Interface that combines creation and update audit functionality.
/// Implementing classes must track when the entity was created and last modified, and by whom.
/// </summary>
public interface IAuditableEntity : ICreationAudit, IUpdateAudit
{
}