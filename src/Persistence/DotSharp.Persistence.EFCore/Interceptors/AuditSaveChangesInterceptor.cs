using DotSharp.Primitives.AuditLog;
using DotSharp.Primitives.Auditing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DotSharp.Persistence.EFCore.Interceptors;

/// <summary>
/// SaveChanges interceptor that populates audit fields on auditable entities.
/// </summary>
public sealed class AuditSaveChangesInterceptor(IAuditUserProvider userProvider) : SaveChangesInterceptor
{
    /// <inheritdoc />
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ApplyAudit(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    /// <inheritdoc />
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ApplyAudit(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ApplyAudit(DbContext? context)
    {
        if (context is null)
            return;

        string user = userProvider.GetCurrent().UserName;

        foreach (EntityEntry entry in context.ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added && entry.Entity is ICreationAudit creationAudit)
                creationAudit.AuditCreation(user);

            if (entry.State == EntityState.Modified && entry.Entity is IModificationAudit modificationAudit)
                modificationAudit.AuditUpdate(user);

            if (entry.State == EntityState.Deleted && entry.Entity is IFullAuditable fullAuditable)
            {
                entry.State = EntityState.Modified;
                fullAuditable.AuditDeletion(user);
            }
        }
    }
}
