using MediatR;

namespace DotSharp.Primitives.Events;

/// <summary>
/// Represents a domain event in the system. Implements INotification for MediatR event handling.
/// </summary>
public abstract class DomainEvent : INotification
{
}