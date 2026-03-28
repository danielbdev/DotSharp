using DotSharp.Observability.Correlation;
using FluentAssertions;
using Xunit;

namespace DotSharp.Observability.Tests.Correlation;

public sealed class CorrelationContextTests
{
    [Fact]
    public void CorrelationContext_SetsCorrelationId()
    {
        CorrelationContext context = new CorrelationContext("abc-123");

        context.CorrelationId.Should().Be("abc-123");
    }

    [Fact]
    public void CorrelationContext_ImplementsICorrelationContext()
    {
        CorrelationContext context = new CorrelationContext("abc-123");

        context.Should().BeAssignableTo<ICorrelationContext>();
    }
}
