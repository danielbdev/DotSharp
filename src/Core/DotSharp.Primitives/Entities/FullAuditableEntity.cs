using DotSharp.Primitives.Auditing;

namespace DotSharp.Primitives.Entities;

/// <summary>
/// Abstract base class that implements IFullAuditableEntity and provides functionality
/// for tracking creation, update, and deletion information of an entity.
/// </summary>
public abstract class FullAuditableEntity : AuditableEntity, IFullAuditable
{
    /// <summary>
    /// Gets or sets a value indicating whether the entity is marked as deleted.
    /// </summary>
    public bool IsDeleted { get; protected set; }

    /// <summary>
    /// Gets or sets the timestamp of when the entity was deleted.
    /// </summary>
    public DateTime? DeletedAt { get; protected set; }

    /// <summary>
    /// Gets or sets the user who deleted the entity.
    /// </summary>
    public string? DeletedBy { get; protected set; }

    /// <summary>
    /// Default constructor.
    /// </summary>
    public FullAuditableEntity() { }

    /// <summary>
    /// Audits the deletion of the entity by marking it as deleted,
    /// setting the deletion timestamp, and recording the user who deleted it.
    /// </summary>
    /// <param name="deletedBy">The user who deleted the entity.</param>
    public void AuditDeletion(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
}

/// <summary>
/// Generic version of the FullAuditableEntity class that supports entities with a custom key type.
/// Inherits from AuditableEntity{TKey} and implements IFullAuditableEntity.
/// </summary>
/// <typeparam name="TKey"></typeparam>
public abstract class FullAuditableEntity<TKey> : AuditableEntity<TKey>, IFullAuditable
{
    /// <summary>
    /// Gets or sets a value indicating whether the entity is marked as deleted.
    /// </summary>
    public bool IsDeleted { get; protected set; }

    /// <summary>
    /// Gets or sets the timestamp of when the entity was deleted.
    /// </summary>
    public DateTime? DeletedAt { get; protected set; }

    /// <summary>
    /// Gets or sets the user who deleted the entity.
    /// </summary>
    public string? DeletedBy { get; protected set; }

    /// <summary>
    /// Default constructor.
    /// </summary>
    public FullAuditableEntity() { }

    /// <summary>
    /// Audits the deletion of the entity by marking it as deleted,
    /// setting the deletion timestamp, and recording the user who deleted it.
    /// </summary>
    /// <param name="deletedBy">The user who deleted the entity.</param>
    public void AuditDeletion(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
}