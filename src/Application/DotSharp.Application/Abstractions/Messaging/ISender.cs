namespace DotSharp.Application.Abstractions.Messaging;

/// <summary>
/// Sends a request to its corresponding handler through the pipeline.
/// </summary>
public interface ISender
{
    /// <summary>
    /// Sends a request and returns the response.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="request">The request to send.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
}
