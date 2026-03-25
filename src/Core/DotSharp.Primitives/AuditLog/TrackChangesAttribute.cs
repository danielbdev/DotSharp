namespace DotSharp.Primitives.AuditLog;

/// <summary>
/// Marks an entity class so that changes to it are automatically captured in the audit log.
/// Inherited by derived classes.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public sealed class TrackChangesAttribute : Attribute { }