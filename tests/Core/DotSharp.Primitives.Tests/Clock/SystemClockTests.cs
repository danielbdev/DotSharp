using DotSharp.Primitives.Clock;
using FluentAssertions;
using Xunit;

namespace DotSharp.Primitives.Tests.Clock;

public sealed class SystemClockTests
{
    [Fact]
    public void UtcNow_ReturnsRecentTime()
    {
        // Arrange
        var clock = new SystemClock();

        // Act
        var result = clock.UtcNow;

        // Assert
        result.Should().BeCloseTo(System.DateTimeOffset.UtcNow, TimeSpan.FromMilliseconds(500));
        result.Offset.Should().Be(TimeSpan.Zero);
    }
}
