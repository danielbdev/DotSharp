namespace DotSharp.Primitives.AuditLog;

/// <summary>
/// Defines a contract for persisting audit log entries.
/// </summary>
public interface IAuditLog
{
    /// <summary>
    /// Persists the given audit log entry asynchronously.
    /// </summary>
    /// <param name="entry">The audit log entry to persist.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task LogAsync(IAuditLogEntry entry, CancellationToken cancellationToken = default);
}