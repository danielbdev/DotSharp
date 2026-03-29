using DotSharp.Application.Abstractions.Behaviors;
using DotSharp.Application.Abstractions.Handlers;
using DotSharp.Application.Abstractions.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DotSharp.Application.Messaging.Requests;

/// <summary>
/// Default implementation of <see cref="ISender"/> that dispatches requests through the pipeline.
/// </summary>
/// <param name="provider">The service provider used to resolve handlers and behaviors.</param>
public sealed class RequestBus(IServiceProvider provider) : ISender
{
    /// <inheritdoc />
    public Task<TResult> Send<TResult>(IRequest<TResult> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        MethodInfo method = typeof(RequestBus).GetMethod(nameof(SendInternal), BindingFlags.NonPublic | BindingFlags.Instance)!;
        MethodInfo closed = method.MakeGenericMethod(request.GetType(), typeof(TResult));

        return (Task<TResult>)closed.Invoke(this, [request, cancellationToken])!;
    }

    private Task<TResult> SendInternal<TRequest, TResult>(TRequest request, CancellationToken cancellationToken)
        where TRequest : IRequest<TResult>
    {
        IRequestHandler<TRequest, TResult> handler = provider.GetRequiredService<IRequestHandler<TRequest, TResult>>();
        IEnumerable<IRequestPipelineBehavior<TRequest, TResult>> behaviors = provider.GetServices<IRequestPipelineBehavior<TRequest, TResult>>();

        RequestPipelineExecutor<TRequest, TResult> executor = new(
            behaviors,
            () => handler.Handle(request, cancellationToken),
            request);

        return executor.Execute(cancellationToken);
    }
}
