namespace DotSharp.Primitives.Auditing;

/// <summary>
/// Interface that extends the functionality of IAuditableEntity and IDeletionAudit,
/// providing a complete audit trail, including creation, update, and deletion information.
/// </summary>
public interface IFullAuditable : IAuditable, IDeletionAudit { }