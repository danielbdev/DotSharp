using DotSharp.Caching.Abstractions.Specifications;
using DotSharp.Persistence.Abstractions.Specifications;
using FluentAssertions;
using Xunit;

namespace DotSharp.Caching.Abstractions.Tests;

public sealed class SpecificationCacheExtensionsTests
{
    #region Test types

    private sealed class Product
    {
        public int Id { get; set; }
        public string Category { get; set; } = null!;
    }

    private sealed class ElectronicsSpec : Specification<Product>
    {
        public ElectronicsSpec()
        {
            SetCriteria(p => p.Category == "Electronics");
        }
    }

    private sealed class ComplexSpec : Specification<Product>
    {
        public ComplexSpec()
        {
            SetCriteria(p => p.Category == "Electronics" && p.Id > 0);
        }
    }

    #endregion

    #region ToCacheKey determinism

    [Fact]
    public void ToCacheKey_SameSpecTwice_ReturnsSameKey()
    {
        var spec1 = new ElectronicsSpec();
        var spec2 = new ElectronicsSpec();

        var key1 = spec1.ToCacheKey();
        var key2 = spec2.ToCacheKey();

        key1.Should().Be(key2);
    }

    [Fact]
    public void ToCacheKey_DifferentSpecs_ProduceDifferentKeys()
    {
        var spec1 = new ElectronicsSpec();
        var spec2 = new ComplexSpec();

        var key1 = spec1.ToCacheKey();
        var key2 = spec2.ToCacheKey();

        key1.Should().NotBe(key2);
    }

    [Fact]
    public void ToCacheKey_ReturnsValidBase64String()
    {
        var spec = new ElectronicsSpec();

        var key = spec.ToCacheKey();

        // Should be 16 characters of Base64 (no padding)
        key.Should().HaveLength(16);
    }

    [Fact]
    public void ToCacheKey_OutputContainsNoSpecialCharacters()
    {
        var spec = new ElectronicsSpec();

        var key = spec.ToCacheKey();

        key.Should().MatchRegex("^[A-Za-z0-9+/=]+$");
    }

    #endregion
}
