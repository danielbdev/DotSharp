namespace DotSharp.Application.Abstractions.Behaviors;

/// <summary>
/// Defines a behavior in the request pipeline.
/// Behaviors are executed in order around the request handler.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public interface IRequestPipelineBehavior<TRequest, TResult>
{
    /// <summary>
    /// Handles the request by executing the behavior and calling the next delegate.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<TResult> Handle(TRequest request, Func<Task<TResult>> next, CancellationToken cancellationToken);
}
