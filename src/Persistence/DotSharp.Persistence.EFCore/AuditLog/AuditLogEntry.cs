using DotSharp.Primitives.AuditLog;

namespace DotSharp.Persistence.EFCore.AuditLog;

/// <summary>
/// EF Core entity that represents a persisted audit log entry.
/// </summary>
public sealed record AuditLogEntry(
    string EntityName,
    string EntityId,
    string Action,
    string? OldValues,
    string? NewValues,
    string? ModifiedBy,
    DateTime ModifiedAt) : IAuditLogEntry;
