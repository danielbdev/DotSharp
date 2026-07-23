using DotSharp.Caching.Redis;
using FluentAssertions;
using System.Text.Json;
using Xunit;

namespace DotSharp.Caching.Redis.Tests;

public sealed class SystemTextJsonSerializerTests
{
    private sealed record TestPayload(string Name, int Value);

    private sealed record NestedPayload(string Id, List<TestPayload> Items, Dictionary<string, int> Metadata);

    private readonly ISerializer _sut = new SystemTextJsonSerializer();

    #region Serialize / Deserialize round-trip

    [Fact]
    public void Serialize_Deserialize_RoundTrip_ReturnsEquivalentObject()
    {
        var original = new TestPayload("Hello", 42);

        var bytes = _sut.Serialize(original);
        var result = _sut.Deserialize<TestPayload>(bytes);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Hello");
        result.Value.Should().Be(42);
    }

    [Fact]
    public void Serialize_Deserialize_RoundTrip_ComplexNestedObject_ReturnsEquivalent()
    {
        var original = new NestedPayload(
            "root",
            new List<TestPayload>
            {
                new("Alpha", 1),
                new("Beta", 2),
            },
            new Dictionary<string, int>
            {
                { "x", 100 },
                { "y", 200 },
            });

        var bytes = _sut.Serialize(original);
        var result = _sut.Deserialize<NestedPayload>(bytes);

        result.Should().NotBeNull();
        result!.Id.Should().Be("root");
        result.Items.Should().HaveCount(2);
        result.Items[0].Name.Should().Be("Alpha");
        result.Items[1].Name.Should().Be("Beta");
        result.Metadata.Should().ContainKeys("x", "y");
        result.Metadata["x"].Should().Be(100);
    }

    [Fact]
    public void Serialize_Deserialize_WithCustomOptions_UsesConfiguredNaming()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var sut = new SystemTextJsonSerializer(options);
        var original = new TestPayload("Test", 7);

        var bytes = sut.Serialize(original);
        var json = System.Text.Encoding.UTF8.GetString(bytes);

        // Verify camelCase property names in output
        json.Should().Contain("\"name\"");
        json.Should().Contain("\"value\"");
    }

    #endregion

    #region Deserialize null / edge cases

    [Fact]
    public void Deserialize_NullBytes_ReturnsDefault()
    {
        var result = _sut.Deserialize<TestPayload>(null);

        result.Should().BeNull();
    }

    [Fact]
    public void Serialize_Deserialize_ValueType_RoundTrip()
    {
        var original = 12345;

        var bytes = _sut.Serialize(original);
        var result = _sut.Deserialize<int>(bytes);

        result.Should().Be(12345);
    }

    #endregion
}
