using DotSharp.Application.Abstractions.Messaging;
using DotSharp.Application.Observability.Behaviors;
using DotSharp.Observability.Correlation;
using DotSharp.Observability.Tracing;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace DotSharp.Application.Observability.Tests.Behaviors;

public sealed class NotificationObservabilityBehaviorTests
{
    #region Test doubles

    private sealed record OrderCreatedNotification(Guid OrderId) : INotification;

    private sealed class StubCorrelationContext : ICorrelationContext
    {
        public string CorrelationId => "test-correlation-id";
    }

    private sealed class StubTraceContext : ITraceContext
    {
        public string TraceId => "test-trace-id";
        public string SpanId => "test-span-id";
    }

    #endregion

    #region Handle

    [Fact]
    public async Task Handle_CallsNext()
    {
        NotificationObservabilityBehavior<OrderCreatedNotification> behavior = new NotificationObservabilityBehavior<OrderCreatedNotification>(
            NullLogger<NotificationObservabilityBehavior<OrderCreatedNotification>>.Instance,
            new StubCorrelationContext(),
            new StubTraceContext());

        bool nextCalled = false;
        OrderCreatedNotification notification = new OrderCreatedNotification(Guid.NewGuid());

        await behavior.Handle(notification, () =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        }, CancellationToken.None);

        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_DoesNotThrow()
    {
        NotificationObservabilityBehavior<OrderCreatedNotification> behavior = new NotificationObservabilityBehavior<OrderCreatedNotification>(
            NullLogger<NotificationObservabilityBehavior<OrderCreatedNotification>>.Instance,
            new StubCorrelationContext(),
            new StubTraceContext());

        OrderCreatedNotification notification = new OrderCreatedNotification(Guid.NewGuid());

        Func<Task> act = () => behavior.Handle(notification,
            () => Task.CompletedTask,
            CancellationToken.None);

        await act.Should().NotThrowAsync();
    }

    #endregion
}
