using DotSharp.Application.Abstractions.Behaviors;
using DotSharp.Application.Abstractions.Handlers;
using DotSharp.Application.Abstractions.Messaging;
using DotSharp.Application.Messaging.Requests;
using DotSharp.Results;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DotSharp.Application.Tests.Messaging;

public sealed class RequestBusTests
{
    #region Test doubles

    private sealed record CreateOrderCommand(string CustomerName) : IRequest<Result<Guid>>;

    private sealed class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<Guid>>
    {
        public Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
            => Task.FromResult(Result<Guid>.Success(Guid.NewGuid()));
    }

    private sealed class TrackingBehavior<TRequest, TResult> : IRequestPipelineBehavior<TRequest, TResult>
    {
        public bool WasCalled { get; private set; }

        public async Task<TResult> Handle(TRequest request, Func<Task<TResult>> next, CancellationToken cancellationToken)
        {
            WasCalled = true;
            return await next();
        }
    }

    #endregion

    #region Send

    [Fact]
    public async Task Send_WhenHandlerRegistered_ReturnsHandlerResult()
    {
        ServiceCollection services = new ServiceCollection();
        services.AddScoped<IRequestHandler<CreateOrderCommand, Result<Guid>>, CreateOrderCommandHandler>();
        ServiceProvider provider = services.BuildServiceProvider();

        RequestBus bus = new RequestBus(provider);
        Result<Guid> result = await bus.Send(new CreateOrderCommand("John"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Send_WhenBehaviorRegistered_ExecutesBehavior()
    {
        TrackingBehavior<CreateOrderCommand, Result<Guid>> tracking = new TrackingBehavior<CreateOrderCommand, Result<Guid>>();
        ServiceCollection services = new ServiceCollection();
        services.AddScoped<IRequestHandler<CreateOrderCommand, Result<Guid>>, CreateOrderCommandHandler>();
        services.AddSingleton<IRequestPipelineBehavior<CreateOrderCommand, Result<Guid>>>(tracking);
        ServiceProvider provider = services.BuildServiceProvider();

        RequestBus bus = new RequestBus(provider);
        await bus.Send(new CreateOrderCommand("John"), CancellationToken.None);

        tracking.WasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Send_WhenRequestIsNull_ThrowsArgumentNullException()
    {
        ServiceCollection services = new ServiceCollection();
        ServiceProvider provider = services.BuildServiceProvider();
        RequestBus bus = new RequestBus(provider);

        Func<Task> act = () => bus.Send<Result<Guid>>(null!, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    #endregion
}
