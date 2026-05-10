using DotSharp.Persistence.EFCore.Contexts;
using DotSharp.Primitives.AuditLog;

namespace DotSharp.Persistence.EFCore.AuditLog;

/// <summary>
/// EF Core implementation of <see cref="IAuditLog"/>.
/// Persists audit log entries to the database.
/// </summary>
public sealed class EfCoreAuditLog(DotSharpDbContext context) : IAuditLog
{
    /// <inheritdoc />
    public async Task LogAsync(IAuditLogEntry entry, CancellationToken cancellationToken = default)
    {
        context.AuditLogEntries.Add(new AuditLogEntry(entry));

        await context.SaveChangesAsync(cancellationToken);
    }
}
