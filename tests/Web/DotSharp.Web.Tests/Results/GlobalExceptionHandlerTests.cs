using System.Text.Json;
using DotSharp.Observability.Correlation;
using DotSharp.Observability.Tracing;
using DotSharp.Web.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DotSharp.Web.Tests.Results;

/// <summary>
/// Tests for <see cref="GlobalExceptionHandler"/> — unhandled exception → 500 ProblemDetails.
/// </summary>
public sealed class GlobalExceptionHandlerTests
{
    [Fact]
    public async Task TryHandleAsync_UnhandledException_Returns500()
    {
        // Arrange
        var correlation = new FakeCorrelationContext("corr-500-test");
        var trace = new FakeTraceContext("trace-500-test", "span-500-test");
        var handler = new GlobalExceptionHandler(correlation, trace);
        var context = new DefaultHttpContext
        {
            Response = { Body = new MemoryStream() },
        };
        var exception = new InvalidOperationException("Test failure.");

        // Act
        var result = await handler.TryHandleAsync(context, exception, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Equal(500, context.Response.StatusCode);

        context.Response.Body.Position = 0;
        var body = await JsonSerializer.DeserializeAsync<ProblemDetails>(
            context.Response.Body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            TestContext.Current.CancellationToken);
        Assert.NotNull(body);
        Assert.Equal("An error occurred while processing your request.", body!.Title);
    }

    [Fact]
    public async Task TryHandleAsync_EnrichesResponseWithCorrelationId()
    {
        // Arrange
        var correlation = new FakeCorrelationContext("corr-enrich-123");
        var trace = new FakeTraceContext("trace-enrich-456", "span-enrich-789");
        var handler = new GlobalExceptionHandler(correlation, trace);
        var context = new DefaultHttpContext
        {
            Response = { Body = new MemoryStream() },
        };
        var exception = new InvalidOperationException("Test failure.");

        // Act
        var result = await handler.TryHandleAsync(context, exception, CancellationToken.None);

        // Assert
        Assert.True(result);

        context.Response.Body.Position = 0;
        var body = await JsonSerializer.DeserializeAsync<ProblemDetails>(
            context.Response.Body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            TestContext.Current.CancellationToken);
        Assert.NotNull(body);
        Assert.NotNull(body!.Extensions);
        Assert.True(body.Extensions.TryGetValue("correlationId", out var corrValue));
        Assert.Equal("corr-enrich-123", corrValue?.ToString());
        Assert.True(body.Extensions.TryGetValue("traceId", out var traceValue));
        Assert.Equal("trace-enrich-456", traceValue?.ToString());
    }

    private sealed class FakeCorrelationContext(string correlationId) : ICorrelationContext
    {
        public string CorrelationId => correlationId;
    }

    private sealed class FakeTraceContext(string traceId, string spanId) : ITraceContext
    {
        public string TraceId => traceId;
        public string SpanId => spanId;
    }
}
