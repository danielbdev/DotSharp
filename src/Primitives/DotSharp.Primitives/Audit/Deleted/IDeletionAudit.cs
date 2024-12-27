namespace DotSharp.Primitives.Audit.Deleted;

/// <summary>
/// Interface that defines the properties and methods for auditing deletion information.
/// Entities implementing this interface should track if the entity is deleted, the deletion timestamp, and the user who performed the deletion.
/// </summary>
public interface IDeletionAudit
{
    /// <summary>
    /// Gets a value indicating whether the entity is marked as deleted.
    /// </summary>
    bool IsDeleted { get; }

    /// <summary>
    /// Gets the timestamp of when the entity was deleted.
    /// </summary>
    DateTime? DeletedAt { get; }

    /// <summary>
    /// Gets the user who deleted the entity.
    /// </summary>
    string? DeletedBy { get; }

    /// <summary>
    /// Audits the deletion of the entity by setting the deletion timestamp, user, and marking the entity as deleted.
    /// </summary>
    /// <param name="deletedBy">The user who deleted the entity.</param>
    void AuditDeletion(string deletedBy);
}