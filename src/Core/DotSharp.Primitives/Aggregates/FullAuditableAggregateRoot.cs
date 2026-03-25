using DotSharp.Primitives.Auditing;

namespace DotSharp.Primitives.Aggregates;

/// <summary>
/// Base aggregate root with full audit information, including soft deletion metadata.
/// </summary>
public abstract class FullAuditableAggregateRoot : AuditableAggregateRoot, IFullAuditable
{
    /// <summary>
    /// Gets or sets a value indicating whether the aggregate root is marked as deleted.
    /// </summary>
    public bool IsDeleted { get; protected set; }

    /// <summary>
    /// Gets or sets the timestamp of when the aggregate root was deleted.
    /// </summary>
    public DateTime? DeletedAt { get; protected set; }

    /// <summary>
    /// Gets or sets the user who deleted the aggregate root.
    /// </summary>
    public string? DeletedBy { get; protected set; }

    /// <summary>
    /// Default constructor.
    /// </summary>
    protected FullAuditableAggregateRoot() { }

    /// <summary>
    /// Audits the deletion of the aggregate root by marking it as deleted,
    /// setting the deletion timestamp, and recording the user who deleted it.
    /// </summary>
    /// <param name="deletedBy">The user who deleted the aggregate root.</param>
    public void AuditDeletion(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
}

/// <summary>
/// Base aggregate root with typed identifier and full audit information, including soft deletion metadata.
/// </summary>
/// <typeparam name="TKey">The type of the aggregate root identifier.</typeparam>
public abstract class FullAuditableAggregateRoot<TKey> : AuditableAggregateRoot<TKey>, IFullAuditable
{
    /// <summary>
    /// Gets or sets a value indicating whether the aggregate root is marked as deleted.
    /// </summary>
    public bool IsDeleted { get; protected set; }

    /// <summary>
    /// Gets or sets the timestamp of when the aggregate root was deleted.
    /// </summary>
    public DateTime? DeletedAt { get; protected set; }

    /// <summary>
    /// Gets or sets the user who deleted the aggregate root.
    /// </summary>
    public string? DeletedBy { get; protected set; }

    /// <summary>
    /// Default constructor.
    /// </summary>
    protected FullAuditableAggregateRoot() { }

    /// <summary>
    /// Audits the deletion of the aggregate root by marking it as deleted,
    /// setting the deletion timestamp, and recording the user who deleted it.
    /// </summary>
    /// <param name="deletedBy">The user who deleted the aggregate root.</param>
    public void AuditDeletion(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
}