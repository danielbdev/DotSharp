using DotSharp.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DotSharp.Web.Tests.Controllers;

/// <summary>
/// Tests for <see cref="DotSharpController"/> — MessageBus lazy resolution.
/// </summary>
public sealed class DotSharpControllerTests
{
    [Fact]
    public void MessageBus_ResolvesFromServiceProvider_Lazily()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<DotSharp.Application.Abstractions.Messaging.IMessageBus>(
            new DotSharpControllerTests_FakeMessageBus());
        var provider = services.BuildServiceProvider();
        var controller = new TestDotSharpController();
        controller.ControllerContext = new()
        {
            HttpContext = new DefaultHttpContext { RequestServices = provider },
        };

        // Act
        var messageBus = controller.ExposeMessageBus();

        // Assert
        Assert.NotNull(messageBus);
        Assert.IsType<DotSharpControllerTests_FakeMessageBus>(messageBus);
    }

    [Fact]
    public void MessageBus_SameInstance_OnSubsequentAccesses()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<DotSharp.Application.Abstractions.Messaging.IMessageBus>(
            new DotSharpControllerTests_FakeMessageBus());
        var provider = services.BuildServiceProvider();
        var controller = new TestDotSharpController();
        controller.ControllerContext = new()
        {
            HttpContext = new DefaultHttpContext { RequestServices = provider },
        };

        // Act
        var first = controller.ExposeMessageBus();
        var second = controller.ExposeMessageBus();

        // Assert
        Assert.Same(first, second);
    }

    /// <summary>
    /// Testable subclass that exposes the protected MessageBus property.
    /// </summary>
    private sealed class TestDotSharpController : DotSharpController
    {
        public DotSharp.Application.Abstractions.Messaging.IMessageBus ExposeMessageBus() => MessageBus;
    }
}

/// <summary>
/// Fake message bus for testing lazy resolution.
/// </summary>
public sealed class DotSharpControllerTests_FakeMessageBus
    : DotSharp.Application.Abstractions.Messaging.IMessageBus
{
    public Task<TResponse> Send<TResponse>(DotSharp.Application.Abstractions.Messaging.IRequest<TResponse> request, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task Publish(DotSharp.Application.Abstractions.Messaging.INotification notification, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task PublishMany(IEnumerable<DotSharp.Application.Abstractions.Messaging.INotification> notifications, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
}
