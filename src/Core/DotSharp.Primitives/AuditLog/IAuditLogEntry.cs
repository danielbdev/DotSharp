namespace DotSharp.Primitives.AuditLog;

/// <summary>
/// Represents a single audit log entry capturing a change made to an entity.
/// </summary>
public interface IAuditLogEntry
{
    /// <summary>
    /// Gets the name of the entity type that was modified.
    /// </summary>
    string EntityName { get; }

    /// <summary>
    /// Gets the string representation of the entity's identifier.
    /// </summary>
    string EntityId { get; }

    /// <summary>
    /// Gets the action performed on the entity (e.g., Created, Updated, Deleted).
    /// </summary>
    string Action { get; }

    /// <summary>
    /// Gets the serialized previous state of the entity before the change, or <see langword="null"/> if not applicable.
    /// </summary>
    string? OldValues { get; }

    /// <summary>
    /// Gets the serialized new state of the entity after the change, or <see langword="null"/> if not applicable.
    /// </summary>
    string? NewValues { get; }

    /// <summary>
    /// Gets the identifier of the user who performed the change, or <see langword="null"/> if unavailable.
    /// </summary>
    string? ModifiedBy { get; }

    /// <summary>
    /// Gets the UTC timestamp of when the change occurred.
    /// </summary>
    DateTime ModifiedAt { get; }
}