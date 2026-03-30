using DotSharp.Primitives.AuditLog;

namespace DotSharp.Persistence.EFCore.AuditLog;

/// <summary>
/// EF Core entity that represents a persisted audit log entry.
/// </summary>
public sealed class AuditLogEntry : IAuditLogEntry
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string EntityName { get; init; } = null!;
    public string EntityId { get; init; } = null!;
    public string Action { get; init; } = null!;
    public string? OldValues { get; init; }
    public string? NewValues { get; init; }
    public string? ModifiedBy { get; init; }
    public DateTime ModifiedAt { get; init; }

    public AuditLogEntry() { }

    public AuditLogEntry(IAuditLogEntry entry)
    {
        EntityName = entry.EntityName;
        EntityId = entry.EntityId;
        Action = entry.Action;
        OldValues = entry.OldValues;
        NewValues = entry.NewValues;
        ModifiedBy = entry.ModifiedBy;
        ModifiedAt = entry.ModifiedAt;
    }
}
