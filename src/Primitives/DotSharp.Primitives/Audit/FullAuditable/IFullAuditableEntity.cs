using DotSharp.Primitives.Audit.Auditable;
using DotSharp.Primitives.Audit.Deleted;

namespace DotSharp.Primitives.Audit.FullAuditable;

/// <summary>
/// Interface that extends the functionality of IAuditableEntity and IDeletionAudit,
/// providing a complete audit trail for entities, including creation, update, and deletion information.
/// </summary>
public interface IFullAuditableEntity : IAuditableEntity, IDeletionAudit
{
}