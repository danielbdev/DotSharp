using DotSharp.Primitives.Auditing;

namespace DotSharp.Primitives.Entities;

/// <summary>
/// Abstract base class for entities that track creation audit information only.
/// </summary>
public abstract class CreationAuditableEntity : Entity, ICreationAudit
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
    /// Default constructor.
    /// </summary>
    protected CreationAuditableEntity() { }

    /// <summary>
    /// Audits the creation of the entity by setting the created timestamp and user.
    /// </summary>
    /// <param name="createdBy">The user who created the entity.</param>
    public void AuditCreation(string createdBy)
    {
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }
}

/// <summary>
/// Abstract base class for entities with a typed identifier <typeparamref name="TKey"/> that track creation audit information only.
/// </summary>
/// <typeparam name="TKey">The type of the entity identifier.</typeparam>
public abstract class CreationAuditableEntity<TKey> : Entity<TKey>, ICreationAudit
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
    /// Default constructor.
    /// </summary>
    protected CreationAuditableEntity() { }

    /// <summary>
    /// Audits the creation of the entity by setting the created timestamp and user.
    /// </summary>
    /// <param name="createdBy">The user who created the entity.</param>
    public void AuditCreation(string createdBy)
    {
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }
}