using DotSharp.Primitives.Auditing;

namespace DotSharp.Primitives.Aggregates;

/// <summary>
/// Base aggregate root with creation and modification audit data.
/// </summary>
public abstract class AuditableAggregateRoot : AggregateRoot, IAuditable
{
    /// <summary>
    /// Gets or sets the timestamp of when the aggregate root was created.
    /// </summary>
    public DateTime CreatedAt { get; protected set; }

    /// <summary>
    /// Gets or sets the user who created the aggregate root.
    /// </summary>
    public string CreatedBy { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the timestamp of when the aggregate root was last modified.
    /// </summary>
    public DateTime? LastModifiedAt { get; protected set; }

    /// <summary>
    /// Gets or sets the user who last modified the aggregate root.
    /// </summary>
    public string? LastModifiedBy { get; protected set; }

    /// <summary>
    /// Default constructor.
    /// </summary>
    protected AuditableAggregateRoot() { }

    /// <summary>
    /// Audits the creation of the aggregate root by setting the created timestamp and user.
    /// </summary>
    /// <param name="createdBy">The user who created the aggregate root.</param>
    public void AuditCreation(string createdBy)
    {
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    /// <summary>
    /// Audits the update of the aggregate root by setting the last modified timestamp and user.
    /// </summary>
    /// <param name="lastModifiedBy">The user who last modified the aggregate root.</param>
    public void AuditUpdate(string lastModifiedBy)
    {
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = lastModifiedBy;
    }
}

/// <summary>
/// Base aggregate root with typed identifier and creation/modification audit data.
/// </summary>
/// <typeparam name="TKey">The type of the aggregate root identifier.</typeparam>
public abstract class AuditableAggregateRoot<TKey> : AggregateRoot<TKey>, IAuditable
{
    /// <summary>
    /// Gets or sets the timestamp of when the aggregate root was created.
    /// </summary>
    public DateTime CreatedAt { get; protected set; }

    /// <summary>
    /// Gets or sets the user who created the aggregate root.
    /// </summary>
    public string CreatedBy { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the timestamp of when the aggregate root was last modified.
    /// </summary>
    public DateTime? LastModifiedAt { get; protected set; }

    /// <summary>
    /// Gets or sets the user who last modified the aggregate root.
    /// </summary>
    public string? LastModifiedBy { get; protected set; }

    /// <summary>
    /// Default constructor.
    /// </summary>
    protected AuditableAggregateRoot() { }

    /// <summary>
    /// Audits the creation of the aggregate root by setting the created timestamp and user.
    /// </summary>
    /// <param name="createdBy">The user who created the aggregate root.</param>
    public void AuditCreation(string createdBy)
    {
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    /// <summary>
    /// Audits the update of the aggregate root by setting the last modified timestamp and user.
    /// </summary>
    /// <param name="lastModifiedBy">The user who last modified the aggregate root.</param>
    public void AuditUpdate(string lastModifiedBy)
    {
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = lastModifiedBy;
    }
}