using DotSharp.Observability.Tracing;
using FluentAssertions;
using Xunit;

namespace DotSharp.Observability.Tests.Tracing;

public sealed class TraceContextTests
{
    [Fact]
    public void TraceId_WhenNoActiveActivity_ReturnsEmptyString()
    {
        TraceContext context = new TraceContext();

        context.TraceId.Should().BeEmpty();
    }

    [Fact]
    public void SpanId_WhenNoActiveActivity_ReturnsEmptyString()
    {
        TraceContext context = new TraceContext();

        context.SpanId.Should().BeEmpty();
    }

    [Fact]
    public void TraceContext_ImplementsITraceContext()
    {
        TraceContext context = new TraceContext();

        context.Should().BeAssignableTo<ITraceContext>();
    }
}
