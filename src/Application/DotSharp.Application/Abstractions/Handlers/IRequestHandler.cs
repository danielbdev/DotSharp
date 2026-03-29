using DotSharp.Application.Abstractions.Messaging;

namespace DotSharp.Application.Abstractions.Handlers;

/// <summary>
/// Handles a request of type <typeparamref name="TRequest"/> and returns a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Handles the request.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
