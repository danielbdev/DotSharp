namespace DotSharp.Primitives.AuditLog;

/// <summary>
/// Represents the user associated with an audited operation.
/// </summary>
/// <param name="UserName">The display name or username of the user.</param>
/// <param name="UserId">The optional unique identifier of the user.</param>
public sealed record AuditUser(string UserName, string? UserId = null);