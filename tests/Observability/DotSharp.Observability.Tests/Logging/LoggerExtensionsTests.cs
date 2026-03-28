using DotSharp.Observability.Correlation;
using DotSharp.Observability.Logging;
using DotSharp.Observability.Tracing;
using FluentAssertions;
using Microsoft.Extensions.Logging.Testing;
using Xunit;

namespace DotSharp.Observability.Tests.Logging;

public sealed class LoggerExtensionsTests
{
    [Fact]
    public void BeginDotSharpScope_ReturnsNonNullScope()
    {
        FakeLogger<LoggerExtensionsTests> logger = new FakeLogger<LoggerExtensionsTests>();
        CorrelationContext correlation = new CorrelationContext("abc-123");
        TraceContext trace = new TraceContext();

        IDisposable? scope = logger.BeginDotSharpScope(correlation, trace);

        scope.Should().NotBeNull();
    }

    [Fact]
    public void BeginDotSharpScope_WhenDisposed_DoesNotThrow()
    {
        FakeLogger<LoggerExtensionsTests> logger = new FakeLogger<LoggerExtensionsTests>();
        CorrelationContext correlation = new CorrelationContext("abc-123");
        TraceContext trace = new TraceContext();

        Action act = () =>
        {
            using IDisposable? scope = logger.BeginDotSharpScope(correlation, trace);
        };

        act.Should().NotThrow();
    }
}
