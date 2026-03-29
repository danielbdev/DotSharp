using DotSharp.Application.Abstractions.Messaging;
using DotSharp.Application.Messaging;
using DotSharp.Results;
using FluentAssertions;
using Xunit;

namespace DotSharp.Application.Tests.Messaging;

public sealed class MessageBusTests
{
    #region Test doubles

    private sealed record TestRequest(string Value) : IRequest<Result<Guid>>;
    private sealed record TestNotification(Guid Id) : INotification;

    private sealed class TrackingSender : ISender
    {
        public bool WasCalled { get; private set; }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            WasCalled = true;
            return Task.FromResult(default(TResponse)!);
        }
    }

    private sealed class TrackingPublisher : IPublisher
    {
        public bool PublishCalled { get; private set; }
        public bool PublishManyCalled { get; private set; }

        public Task Publish(INotification notification, CancellationToken cancellationToken = default)
        {
            PublishCalled = true;
            return Task.CompletedTask;
        }

        public Task PublishMany(IEnumerable<INotification> notifications, CancellationToken cancellationToken = default)
        {
            PublishManyCalled = true;
            return Task.CompletedTask;
        }
    }

    #endregion

    #region Send

    [Fact]
    public async Task Send_DelegatesToSender()
    {
        TrackingSender sender = new TrackingSender();
        TrackingPublisher publisher = new TrackingPublisher();
        MessageBus bus = new MessageBus(sender, publisher);

        await bus.Send(new TestRequest("test"), CancellationToken.None);

        sender.WasCalled.Should().BeTrue();
    }

    #endregion

    #region Publish

    [Fact]
    public async Task Publish_DelegatesToPublisher()
    {
        TrackingSender sender = new TrackingSender();
        TrackingPublisher publisher = new TrackingPublisher();
        MessageBus bus = new MessageBus(sender, publisher);

        await bus.Publish(new TestNotification(Guid.NewGuid()), CancellationToken.None);

        publisher.PublishCalled.Should().BeTrue();
    }

    #endregion

    #region PublishMany

    [Fact]
    public async Task PublishMany_DelegatesToPublisher()
    {
        TrackingSender sender = new TrackingSender();
        TrackingPublisher publisher = new TrackingPublisher();
        MessageBus bus = new MessageBus(sender, publisher);

        await bus.PublishMany([new TestNotification(Guid.NewGuid())], CancellationToken.None);

        publisher.PublishManyCalled.Should().BeTrue();
    }

    #endregion
}
