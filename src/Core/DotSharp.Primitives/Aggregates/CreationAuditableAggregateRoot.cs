using DotSharp.Primitives.Auditing;

namespace DotSharp.Primitives.Aggregates;

/// <summary>
/// Base aggregate root with creation audit data only.
/// </summary>
public abstract class CreationAuditableAggregateRoot : AggregateRoot, ICreationAudit
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
    /// Default constructor.
    /// </summary>
    protected CreationAuditableAggregateRoot() { }

    /// <summary>
    /// Audits the creation of the aggregate root by setting the created timestamp and user.
    /// </summary>
    /// <param name="createdBy">The user who created the aggregate root.</param>
    public void AuditCreation(string createdBy)
    {
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }    
}

/// <summary>
/// Base aggregate root with typed identifier and creation audit data only.
/// </summary>
/// <typeparam name="TKey">The type of the aggregate root identifier.</typeparam>
public abstract class CreationAuditableAggregateRoot<TKey> : AggregateRoot<TKey>, ICreationAudit
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
    /// Default constructor.
    /// </summary>
    protected CreationAuditableAggregateRoot() { }

    /// <summary>
    /// Audits the creation of the aggregate root by setting the created timestamp and user.
    /// </summary>
    /// <param name="createdBy">The user who created the aggregate root.</param>
    public void AuditCreation(string createdBy)
    {
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }
}