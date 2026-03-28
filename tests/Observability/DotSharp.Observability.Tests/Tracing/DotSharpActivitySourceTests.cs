using System.Diagnostics;
using DotSharp.Observability.Tracing;
using FluentAssertions;
using Xunit;

namespace DotSharp.Observability.Tests.Tracing;

public sealed class DotSharpActivitySourceTests
{
    [Fact]
    public void Instance_IsNotNull()
    {
        DotSharpActivitySource.Instance.Should().NotBeNull();
    }

    [Fact]
    public void Instance_HasCorrectName()
    {
        DotSharpActivitySource.Instance.Name.Should().Be("DotSharp");
    }

    [Fact]
    public void Instance_IsSameReference()
    {
        ActivitySource first = DotSharpActivitySource.Instance;
        ActivitySource second = DotSharpActivitySource.Instance;

        first.Should().BeSameAs(second);
    }
}
