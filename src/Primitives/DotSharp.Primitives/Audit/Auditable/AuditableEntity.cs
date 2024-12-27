using DotSharp.Primitives.Entities;

namespace DotSharp.Primitives.Audit.Auditable;

/// <summary>
/// Abstract base class for entities that need audit capabilities.
/// Tracks creation and last modification information such as the timestamp and the user.
/// </summary>
public abstract class AuditableEntity : Entity, IAuditableEntity
{
    /// <summary>
    /// Gets or sets the timestamp of when the entity was created.
    /// </summary>
    public DateTime CreatedAt { get; protected set; }

    /// <summary>
    /// Gets or sets the user who created the entity.
    /// </summary>
    public string CreatedBy { get; protected set; }

    /// <summary>
    /// Gets or sets the timestamp of when the entity was last modified.
    /// </summary>
    public DateTime? LastModifiedAt { get; protected set; }

    /// <summary>
    /// Gets or sets the user who last modified the entity.
    /// </summary>
    public string? LastModifiedBy { get; protected set; }

    /// <summary>
    /// Default constructor.
    /// </summary>
    protected AuditableEntity()
    { }

    /// <summary>
    /// Audits the creation of the entity by setting the created timestamp and user.
    /// </summary>
    /// <param name="createdBy">The user who created the entity.</param
    public void AuditCreation(string createdBy)
    {
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    /// <summary>
    /// Audits the update of the entity by setting the last modified timestamp and user.
    /// </summary>
    /// <param name="lastModifiedBy">The user who last modified the entity.</param>
    public void AuditUpdate(string lastModifiedBy)
    {
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = lastModifiedBy;
    }
}

/// <summary>
/// Abstract base class for entities that need audit capabilities.
/// Tracks creation and last modification information such as the timestamp and the user.
/// This version works with entities identified by a key of type <typeparamref name="TKey"/>.
/// </summary>
public abstract class AuditableEntity<TKey> : Entity<TKey>, IAuditableEntity
{
    /// <summary>
    /// Gets or sets the timestamp of when the entity was created.
    /// </summary>
    public DateTime CreatedAt { get; protected set; }

    /// <summary>
    /// Gets or sets the user who created the entity.
    /// </summary>
    public string CreatedBy { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the timestamp of when the entity was last modified.
    /// </summary>
    public DateTime? LastModifiedAt { get; protected set; }

    /// <summary>
    /// Gets or sets the user who last modified the entity.
    /// </summary>
    public string? LastModifiedBy { get; protected set; }

    /// <summary>
    /// Default constructor.
    /// </summary>
    protected AuditableEntity()
    { }

    /// <summary>
    /// Audits the creation of the entity by setting the created timestamp and user.
    /// </summary>
    /// <param name="createdBy">The user who created the entity.</param>
    public void AuditCreation(string createdBy)
    {
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    /// <summary>
    /// Audits the update of the entity by setting the last modified timestamp and user.
    /// </summary>
    /// <param name="lastModifiedBy">The user who last modified the entity.</param>
    public void AuditUpdate(string lastModifiedBy)
    {
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = lastModifiedBy;
    }
}