namespace DotSharp.Primitives.Audit;

/// <summary>
/// Represents an audit log entry for tracking changes made to an entity in the system.
/// Contains information about the entity being modified, the type of action,
/// the old and new values, and the user who made the change.
/// </summary>
public class AuditLog
{
    /// <summary>
    /// Gets the unique identifier of the audit log entry.
    /// </summary>
    public Guid Id { get; protected set; }

    /// <summary>
    /// Gets the name of the entity that was changed (e.g., "User", "Product").
    /// </summary>
    public string EntityName { get; protected set; } = null!;

    /// <summary>
    /// Gets the identifier of the entity being audited.
    /// </summary>
    public string EntityId { get; protected set; } = null!;

    /// <summary>
    /// Gets the type of action performed (e.g., "Create", "Update", "Delete").
    /// </summary>
    public string ActionType { get; protected set; } = null!;

    /// <summary>
    /// Gets the old value of the entity property before the change.
    /// </summary>
    public string OldValue { get; protected set; } = null!;

    /// <summary>
    /// Gets the new value of the entity property after the change.
    /// </summary>
    public string NewValue { get; protected set; } = null!;

    /// <summary>
    /// Gets the user who made the change.
    /// </summary>
    public string ChangedBy { get; protected set; } = null!;

    /// <summary>
    /// Gets the timestamp of when the change occurred.
    /// </summary>
    public DateTime ChangeDate { get; protected set; }

    /// <summary>
    /// Creates a new instance of the <see cref="AuditLog"/> class with the specified details.
    /// </summary>
    /// <param name="entityName">The name of the entity being modified (e.g., "User").</param>
    /// <param name="entityId">The identifier of the entity being modified.</param>
    /// <param name="actionType">The type of action (e.g., "Create", "Update", "Delete").</param>
    /// <param name="oldValue">The old value of the property being changed.</param>
    /// <param name="newValue">The new value of the property being changed.</param>
    /// <param name="changedBy">The user who made the change.</param>
    /// <returns>A new instance of the <see cref="AuditLog"/> class.</returns>
    public static AuditLog Create(string entityName, string? entityId, string actionType, string oldValue, string newValue, string changedBy)
    {
        return new AuditLog()
        {
            Id = Guid.NewGuid(),
            EntityName = entityName,
            EntityId = entityId ?? string.Empty,
            ActionType = actionType,
            OldValue = oldValue,
            NewValue = newValue,
            ChangedBy = changedBy,
            ChangeDate = DateTime.UtcNow
        };
    }
}