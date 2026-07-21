using DotSharp.Web.Correlation;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace DotSharp.Web.Tests.Correlation;

/// <summary>
/// Tests for <see cref="CorrelationMiddleware"/> — X-Correlation-Id header handling.
/// </summary>
public sealed class CorrelationMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_ExistingHeader_UsesProvidedId()
    {
        // Arrange
        var middleware = new CorrelationMiddleware();
        var context = new DefaultHttpContext();
        const string providedId = "my-id-123";
        context.Request.Headers["X-Correlation-Id"] = providedId;
        bool nextCalled = false;
        Task Next(HttpContext _) { nextCalled = true; return Task.CompletedTask; }

        // Act
        await middleware.InvokeAsync(context, Next);

        // Assert
        Assert.True(nextCalled);
        Assert.Equal(providedId, context.Items["X-Correlation-Id"]);
        Assert.Equal(providedId, context.Response.Headers["X-Correlation-Id"].ToString());
    }

    [Fact]
    public async Task InvokeAsync_MissingHeader_GeneratesNewGuid()
    {
        // Arrange
        var middleware = new CorrelationMiddleware();
        var context = new DefaultHttpContext();
        bool nextCalled = false;
        Task Next(HttpContext _) { nextCalled = true; return Task.CompletedTask; }

        // Act
        await middleware.InvokeAsync(context, Next);

        // Assert
        Assert.True(nextCalled);
        var correlationId = Assert.IsType<string>(context.Items["X-Correlation-Id"]);
        Assert.NotEmpty(correlationId);
        Assert.Equal(correlationId, context.Response.Headers["X-Correlation-Id"].ToString());
        // Guid.ToString("N") produces 32 hex chars
        Assert.Equal(32, correlationId.Length);
        Assert.True(Guid.TryParseExact(correlationId, "N", out _));
    }

    [Fact]
    public async Task InvokeAsync_HeaderSetBeforeNextIsCalled()
    {
        // Arrange
        var middleware = new CorrelationMiddleware();
        var context = new DefaultHttpContext();
        string? capturedCorrelationId = null;
        Task Next(HttpContext ctx)
        {
            // At this point, both Items and Response headers should be set
            capturedCorrelationId = ctx.Items["X-Correlation-Id"] as string;
            return Task.CompletedTask;
        }

        // Act
        await middleware.InvokeAsync(context, Next);

        // Assert
        Assert.NotNull(capturedCorrelationId);
        Assert.NotEmpty(capturedCorrelationId);
        Assert.Equal(capturedCorrelationId, context.Response.Headers["X-Correlation-Id"].ToString());
    }
}
