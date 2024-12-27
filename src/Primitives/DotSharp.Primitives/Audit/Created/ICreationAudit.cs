namespace DotSharp.Primitives.Audit.Created;

/// <summary>
/// Interface that defines the properties and methods for auditing creation information.
/// Entities implementing this interface should track the creation timestamp and the user who created the entity.
/// </summary>
public interface ICreationAudit
{
    /// <summary>
    /// Gets the timestamp of when the entity was created.
    /// </summary>
    DateTime CreatedAt { get; }

    /// <summary>
    /// Gets the user who created the entity.
    /// </summary>
    string CreatedBy { get; }

    /// <summary>
    /// Audits the creation of the entity by setting the created timestamp and user.
    /// </summary>
    /// <param name="createdBy">The user who created the entity.</param>
    void AuditCreation(string createdBy);
}