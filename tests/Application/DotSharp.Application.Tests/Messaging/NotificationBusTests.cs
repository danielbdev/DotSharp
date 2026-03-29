using DotSharp.Application.Abstractions.Behaviors;
using DotSharp.Application.Abstractions.Handlers;
using DotSharp.Application.Abstractions.Messaging;
using DotSharp.Application.Messaging.Notifications;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DotSharp.Application.Tests.Messaging;

public sealed class NotificationBusTests
{
    #region Test doubles

    private sealed record OrderCreatedNotification(Guid OrderId) : INotification;

    private sealed class TrackingNotificationHandler : INotificationHandler<OrderCreatedNotification>
    {
        public bool WasCalled { get; private set; }

        public Task Handle(OrderCreatedNotification notification, CancellationToken cancellationToken)
        {
            WasCalled = true;
            return Task.CompletedTask;
        }
    }

    private sealed class TrackingNotificationBehavior<TNotification> : INotificationPipelineBehavior<TNotification>
        where TNotification : INotification
    {
        public bool WasCalled { get; private set; }

        public async Task Handle(TNotification notification, Func<Task> next, CancellationToken cancellationToken)
        {
            WasCalled = true;
            await next();
        }
    }

    #endregion

    #region Publish

    [Fact]
    public async Task Publish_WhenHandlerRegistered_CallsHandler()
    {
        TrackingNotificationHandler handler = new TrackingNotificationHandler();
        ServiceCollection services = new ServiceCollection();
        services.AddSingleton<INotificationHandler<OrderCreatedNotification>>(handler);
        ServiceProvider provider = services.BuildServiceProvider();

        NotificationBus bus = new NotificationBus(provider);
        await bus.Publish(new OrderCreatedNotification(Guid.NewGuid()), CancellationToken.None);

        handler.WasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Publish_WhenNoHandlersRegistered_DoesNotThrow()
    {
        ServiceCollection services = new ServiceCollection();
        ServiceProvider provider = services.BuildServiceProvider();
        NotificationBus bus = new NotificationBus(provider);

        Func<Task> act = () => bus.Publish(new OrderCreatedNotification(Guid.NewGuid()), CancellationToken.None);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Publish_WhenMultipleHandlersRegistered_CallsAllHandlers()
    {
        TrackingNotificationHandler handler1 = new TrackingNotificationHandler();
        TrackingNotificationHandler handler2 = new TrackingNotificationHandler();
        ServiceCollection services = new ServiceCollection();
        services.AddSingleton<INotificationHandler<OrderCreatedNotification>>(handler1);
        services.AddSingleton<INotificationHandler<OrderCreatedNotification>>(handler2);
        ServiceProvider provider = services.BuildServiceProvider();

        NotificationBus bus = new NotificationBus(provider);
        await bus.Publish(new OrderCreatedNotification(Guid.NewGuid()), CancellationToken.None);

        handler1.WasCalled.Should().BeTrue();
        handler2.WasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Publish_WhenNotificationIsNull_ThrowsArgumentNullException()
    {
        ServiceCollection services = new ServiceCollection();
        ServiceProvider provider = services.BuildServiceProvider();
        NotificationBus bus = new NotificationBus(provider);

        Func<Task> act = () => bus.Publish(null!, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task Publish_WhenBehaviorRegistered_ExecutesBehavior()
    {
        TrackingNotificationHandler handler = new TrackingNotificationHandler();
        TrackingNotificationBehavior<OrderCreatedNotification> behavior = new TrackingNotificationBehavior<OrderCreatedNotification>();
        ServiceCollection services = new ServiceCollection();
        services.AddSingleton<INotificationHandler<OrderCreatedNotification>>(handler);
        services.AddSingleton<INotificationPipelineBehavior<OrderCreatedNotification>>(behavior);
        ServiceProvider provider = services.BuildServiceProvider();

        NotificationBus bus = new NotificationBus(provider);
        await bus.Publish(new OrderCreatedNotification(Guid.NewGuid()), CancellationToken.None);

        behavior.WasCalled.Should().BeTrue();
    }

    #endregion

    #region PublishMany

    [Fact]
    public async Task PublishMany_WhenNotificationsProvided_CallsHandlerForEach()
    {
        TrackingNotificationHandler handler = new TrackingNotificationHandler();
        ServiceCollection services = new ServiceCollection();
        services.AddSingleton<INotificationHandler<OrderCreatedNotification>>(handler);
        ServiceProvider provider = services.BuildServiceProvider();

        NotificationBus bus = new NotificationBus(provider);
        OrderCreatedNotification[] notifications = new[]
        {
            new OrderCreatedNotification(Guid.NewGuid()),
            new OrderCreatedNotification(Guid.NewGuid())
        };

        await bus.PublishMany(notifications, CancellationToken.None);

        handler.WasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task PublishMany_WhenNotificationsIsNull_ThrowsArgumentNullException()
    {
        ServiceCollection services = new ServiceCollection();
        ServiceProvider provider = services.BuildServiceProvider();
        NotificationBus bus = new NotificationBus(provider);

        Func<Task> act = () => bus.PublishMany(null!, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    #endregion
}
