using DotSharp.Application.Abstractions.Behaviors;
using DotSharp.Application.Abstractions.Messaging;

namespace DotSharp.Application.Messaging.Requests;

/// <summary>
/// Executes the request pipeline by chaining behaviors around the handler.
/// </summary>
internal sealed class RequestPipelineExecutor<TRequest, TResult>(
    IEnumerable<IRequestPipelineBehavior<TRequest, TResult>> behaviors,
    Func<Task<TResult>> handler,
    TRequest request)
    where TRequest : IRequest<TResult>
{
    private readonly IReadOnlyList<IRequestPipelineBehavior<TRequest, TResult>> _behaviors = [.. behaviors.Reverse()];
    private readonly TRequest _request = request;

    /// <summary>
    /// Executes the pipeline.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task<TResult> Execute(CancellationToken cancellationToken)
    {
        Func<Task<TResult>> next = handler;

        foreach (IRequestPipelineBehavior<TRequest, TResult> behavior in _behaviors)
        {
            Func<Task<TResult>> current = next;
            TRequest request = _request;
            next = () => behavior.Handle(request, current, cancellationToken);
        }

        return next();
    }
}
