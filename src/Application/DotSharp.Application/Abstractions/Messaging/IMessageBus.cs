namespace DotSharp.Application.Abstractions.Messaging;

/// <summary>
/// Facade that combines <see cref="ISender"/> and <see cref="IPublisher"/>.
/// </summary>
public interface IMessageBus : ISender, IPublisher { }
