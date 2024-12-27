namespace DotSharp.Primitives.Audit.Updated;

/// <summary>
/// Interface that defines the properties and methods for auditing update information.
/// Entities implementing this interface should track the timestamp of the last modification and the user who made the modification.
/// </summary>
public interface IUpdateAudit
{
    /// <summary>
    /// Gets the timestamp of when the entity was last modified.
    /// </summary>
    DateTime? LastModifiedAt { get; }

    /// <summary>
    /// Gets the user who last modified the entity.
    /// </summary>
    string? LastModifiedBy { get; }

    /// <summary>
    /// Audits the update of the entity by setting the last modified timestamp and user.
    /// </summary>
    /// <param name="lastModifiedBy">The user who last modified the entity.</param>
    void AuditUpdate(string lastModifiedBy);
}