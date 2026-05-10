using System.Text.Json;
using DotSharp.Persistence.EFCore.AuditLog;
using DotSharp.Primitives.AuditLog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DotSharp.Persistence.EFCore.Interceptors;

/// <summary>
/// SaveChanges interceptor that generates audit log entries for entities marked with <see cref="TrackChangesAttribute"/>.
/// </summary>
public sealed class AuditLogSaveChangesInterceptor(
    IAuditUserProvider userProvider,
    IAuditLog auditLog) : SaveChangesInterceptor
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <inheritdoc />
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
            await ApplyAuditLogAsync(eventData.Context, cancellationToken);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private async Task ApplyAuditLogAsync(DbContext context, CancellationToken cancellationToken)
    {
        AuditUser currentUser = userProvider.GetCurrent();

        foreach (EntityEntry entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is AuditLogEntry)
                continue;

            if (!HasTrackChangesAttribute(entry.Entity.GetType()))
                continue;

            if (entry.State is not (EntityState.Added or EntityState.Modified or EntityState.Deleted))
                continue;

            string action = entry.State switch
            {
                EntityState.Added => "Created",
                EntityState.Modified => "Modified",
                _ => "Deleted"
            };

            string entityId = GetEntityId(entry);

            string? oldValues = entry.State == EntityState.Modified
                ? JsonSerializer.Serialize(
                    entry.Properties
                        .Where(p => p.IsModified)
                        .ToDictionary(p => p.Metadata.Name, p => p.OriginalValue),
                    _jsonOptions)
                : null;

            string? newValues = entry.State != EntityState.Deleted
                ? JsonSerializer.Serialize(
                    entry.Properties
                        .Where(p => p.IsModified || entry.State == EntityState.Added)
                        .ToDictionary(p => p.Metadata.Name, p => p.CurrentValue),
                    _jsonOptions)
                : null;

            await auditLog.LogAsync(new AuditLogEntry
            {
                EntityName = entry.Entity.GetType().Name,
                EntityId = entityId,
                Action = action,
                OldValues = oldValues,
                NewValues = newValues,
                ModifiedBy = currentUser.UserName,
                ModifiedAt = DateTime.UtcNow
            }, cancellationToken);
        }
    }

    private static bool HasTrackChangesAttribute(Type type)
        => type.GetCustomAttributes(typeof(TrackChangesAttribute), inherit: true).Length > 0;

    private static string GetEntityId(EntityEntry entry)
    {
        object? keyValue = entry.Properties
            .FirstOrDefault(p => p.Metadata.IsPrimaryKey())
            ?.CurrentValue;

        return keyValue?.ToString() ?? string.Empty;
    }
}
