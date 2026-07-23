using DotSharp.Caching.Abstractions;
using DotSharp.Results;
using FluentAssertions;
using Xunit;

namespace DotSharp.Caching.Abstractions.Tests;

public sealed class CacheResultExtensionsTests
{
    #region Test doubles

    /// <summary>
    /// Fake cache that returns a pre-configured value from GetOrSetAsync.
    /// </summary>
    private sealed class FakeCache<T>(T? value) : ICacheService<T>
    {
        public T? Value { get; } = value;

        public ValueTask<T?> GetAsync(string key, CancellationToken ct = default)
            => ValueTask.FromResult(Value);

        public ValueTask<T?> GetOrSetAsync(
            string key,
            Func<CancellationToken, ValueTask<T?>> factory,
            CacheOptions? options = null,
            CancellationToken ct = default)
            => ValueTask.FromResult(Value);

        public ValueTask SetAsync(string key, T value, CacheOptions? options = null, CancellationToken ct = default)
            => ValueTask.CompletedTask;

        public ValueTask RemoveAsync(string key, CancellationToken ct = default)
            => ValueTask.CompletedTask;

        public ValueTask<bool> ExistsAsync(string key, CancellationToken ct = default)
            => ValueTask.FromResult(Value is not null);
    }

    #endregion

    #region Null → NotFound

    [Fact]
    public async Task GetOrSetResultAsync_WhenFactoryReturnsNull_ReturnsNotFoundResult()
    {
        ICacheService<string> cache = new FakeCache<string>(null);

        var result = await cache.GetOrSetResultAsync(
            "product:999",
            _ => ValueTask.FromResult<string?>(null),
            ct: TestContext.Current.CancellationToken);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be(ErrorCodes.NotFound);
    }

    [Fact]
    public async Task GetOrSetResultAsync_WhenFactoryReturnsNull_ErrorMessageContainsKey()
    {
        ICacheService<string> cache = new FakeCache<string>(null);

        var result = await cache.GetOrSetResultAsync(
            "product:999",
            _ => ValueTask.FromResult<string?>(null),
            ct: TestContext.Current.CancellationToken);

        result.Error!.Message.Should().Contain("product:999");
    }

    #endregion

    #region Hit → Success

    [Fact]
    public async Task GetOrSetResultAsync_WhenValueExists_ReturnsSuccessResult()
    {
        ICacheService<string> cache = new FakeCache<string>("Hello");

        var result = await cache.GetOrSetResultAsync(
            "product:42",
            _ => ValueTask.FromResult<string?>("ignored"),
            ct: TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("Hello");
    }

    [Fact]
    public async Task GetOrSetResultAsync_WhenValueExists_DoesNotInvokeFactory()
    {
        bool factoryCalled = false;
        ICacheService<string> cache = new FakeCache<string>("Hello");

        var result = await cache.GetOrSetResultAsync(
            "product:42",
            _ =>
            {
                factoryCalled = true;
                return ValueTask.FromResult<string?>("ShouldNotBeUsed");
            },
            ct: TestContext.Current.CancellationToken);

        factoryCalled.Should().BeFalse();
        result.Value.Should().Be("Hello");
    }

    #endregion
}
