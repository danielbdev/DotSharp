using DotSharp.Caching.Abstractions;
using FluentAssertions;
using Xunit;

namespace DotSharp.Caching.Abstractions.Tests;

public sealed class CacheKeyTests
{
    #region Implicit conversion from string

    [Fact]
    public void ImplicitConversion_FromString_ReturnsKeyWithSameValue()
    {
        CacheKey key = "product:42";

        key.ToString().Should().Be("product:42");
    }

    [Fact]
    public void ImplicitConversion_TwoSameStrings_ProduceEqualKeys()
    {
        CacheKey key1 = "product:42";
        CacheKey key2 = "product:42";

        key1.ToString().Should().Be(key2.ToString());
    }

    #endregion

    #region For<T> builder

    [Fact]
    public void For_Builder_ReturnsBuilderWithTypeEmbedded()
    {
        var key = CacheKey.For<Product>()
            .WithSegment("42")
            .Build();

        key.ToString().Should().Contain("Product");
    }

    [Fact]
    public void Builder_WithMultipleSegments_ProducesDeterministicKey()
    {
        var key1 = CacheKey.For<Product>()
            .WithPrefix("hot")
            .WithSegment("42")
            .WithSegment("active")
            .Build();

        var key2 = CacheKey.For<Product>()
            .WithPrefix("hot")
            .WithSegment("42")
            .WithSegment("active")
            .Build();

        key1.ToString().Should().Be(key2.ToString());
    }

    [Fact]
    public void Builder_WithoutPrefix_OmitsPrefixFromOutput()
    {
        var key = CacheKey.For<Product>()
            .WithSegment("42")
            .Build();

        key.ToString().Should().Be("Product:42");
    }

    [Fact]
    public void Builder_WithPrefix_PrependsPrefix()
    {
        var key = CacheKey.For<Product>()
            .WithPrefix("hot")
            .WithSegment("42")
            .Build();

        key.ToString().Should().Be("hot:Product:42");
    }

    [Fact]
    public void Builder_DifferentTypes_ProduceDifferentKeys()
    {
        var productKey = CacheKey.For<Product>()
            .WithSegment("42")
            .Build();

        var orderKey = CacheKey.For<Order>()
            .WithSegment("42")
            .Build();

        productKey.ToString().Should().NotBe(orderKey.ToString());
    }

    [Fact]
    public void Builder_WithNumerousSegments_JoinsThemCorrectly()
    {
        var key = CacheKey.For<Product>()
            .WithPrefix("cache")
            .WithSegment("a")
            .WithSegment("b")
            .WithSegment("c")
            .Build();

        key.ToString().Should().Be("cache:Product:a:b:c");
    }

    #endregion

    #region String conversion

    [Fact]
    public void ToString_WithExplicitConstruction_ReturnsExpectedFormat()
    {
        var key = CacheKey.For<Order>()
            .WithSegment("99")
            .Build();

        key.ToString().Should().Be("Order:99");
    }

    #endregion

    #region Test types

    private sealed class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }

    private sealed class Order
    {
        public int Id { get; set; }
        public string Customer { get; set; } = null!;
    }

    #endregion
}
