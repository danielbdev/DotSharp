namespace DotSharp.Primitives.AuditLog;

/// <summary>
/// Defines a contract for resolving the current user performing an operation, used to populate audit fields.
/// </summary>
public interface IAuditUserProvider
{
    /// <summary>
    /// Returns the current <see cref="AuditUser"/> for the active request or operation context.
    /// </summary>
    AuditUser GetCurrent();
}