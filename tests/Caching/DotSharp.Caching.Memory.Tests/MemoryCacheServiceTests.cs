using DotSharp.Caching.Abstractions;
using DotSharp.Caching.Memory;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Xunit;

namespace DotSharp.Caching.Memory.Tests;

public sealed class MemoryCacheServiceTests : IDisposable
{
    private readonly IMemoryCache _memoryCache;
    private readonly MemoryCacheService<string> _sut;

    public MemoryCacheServiceTests()
    {
        _memoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));
        _sut = new MemoryCacheService<string>(_memoryCache);
    }

    public void Dispose()
    {
        _memoryCache.Dispose();
    }

    #region GetAsync

    [Fact]
    public async Task GetAsync_WhenEntryExists_ReturnsValue()
    {
        _memoryCache.Set("key1", "hello");

        var result = await _sut.GetAsync("key1", TestContext.Current.CancellationToken);

        result.Should().Be("hello");
    }

    [Fact]
    public async Task GetAsync_WhenEntryDoesNotExist_ReturnsNull()
    {
        var result = await _sut.GetAsync("nonexistent", TestContext.Current.CancellationToken);

        result.Should().BeNull();
    }

    #endregion

    #region SetAsync

    [Fact]
    public async Task SetAsync_StoresValue()
    {
        await _sut.SetAsync("key1", "hello", ct: TestContext.Current.CancellationToken);

        var result = _memoryCache.Get<string>("key1");
        result.Should().Be("hello");
    }

    [Fact]
    public async Task SetAsync_OverwritesExistingValue()
    {
        _memoryCache.Set("key1", "old");
        await _sut.SetAsync("key1", "new", ct: TestContext.Current.CancellationToken);

        var result = _memoryCache.Get<string>("key1");
        result.Should().Be("new");
    }

    #endregion

    #region RemoveAsync

    [Fact]
    public async Task RemoveAsync_RemovesExistingEntry()
    {
        _memoryCache.Set("key1", "hello");
        await _sut.RemoveAsync("key1", TestContext.Current.CancellationToken);

        _memoryCache.TryGetValue("key1", out string? _).Should().BeFalse();
    }

    [Fact]
    public async Task RemoveAsync_WhenKeyDoesNotExist_DoesNotThrow()
    {
        var act = () => _sut.RemoveAsync("nonexistent", TestContext.Current.CancellationToken).AsTask();

        await act.Should().NotThrowAsync();
    }

    #endregion

    #region ExistsAsync

    [Fact]
    public async Task ExistsAsync_WhenEntryExists_ReturnsTrue()
    {
        _memoryCache.Set("key1", "hello");

        var result = await _sut.ExistsAsync("key1", TestContext.Current.CancellationToken);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WhenEntryDoesNotExist_ReturnsFalse()
    {
        var result = await _sut.ExistsAsync("nonexistent", TestContext.Current.CancellationToken);

        result.Should().BeFalse();
    }

    #endregion

    #region GetOrSetAsync — hit

    [Fact]
    public async Task GetOrSetAsync_WhenEntryExists_ReturnsCachedValue()
    {
        _memoryCache.Set("key1", "cached");

        var result = await _sut.GetOrSetAsync(
            "key1",
            _ => ValueTask.FromResult<string?>("factory-result"),
            ct: TestContext.Current.CancellationToken);

        result.Should().Be("cached");
    }

    [Fact]
    public async Task GetOrSetAsync_WhenEntryExists_DoesNotInvokeFactory()
    {
        _memoryCache.Set("key1", "cached");
        bool factoryCalled = false;

        await _sut.GetOrSetAsync(
            "key1",
            _ =>
            {
                factoryCalled = true;
                return ValueTask.FromResult<string?>("factory-result");
            },
            ct: TestContext.Current.CancellationToken);

        factoryCalled.Should().BeFalse();
    }

    #endregion

    #region GetOrSetAsync — miss

    [Fact]
    public async Task GetOrSetAsync_WhenEntryDoesNotExist_InvokesFactory()
    {
        var result = await _sut.GetOrSetAsync(
            "key1",
            _ => ValueTask.FromResult<string?>("fresh"),
            ct: TestContext.Current.CancellationToken);

        result.Should().Be("fresh");
    }

    [Fact]
    public async Task GetOrSetAsync_WhenEntryDoesNotExist_StoresResult()
    {
        await _sut.GetOrSetAsync(
            "key1",
            _ => ValueTask.FromResult<string?>("fresh"),
            ct: TestContext.Current.CancellationToken);

        _memoryCache.TryGetValue("key1", out string? stored).Should().BeTrue();
        stored.Should().Be("fresh");
    }

    #endregion

    #region Stampede protection

    [Fact]
    public async Task GetOrSetAsync_ConcurrentSameKey_InvokesFactoryOnlyOnce()
    {
        int factoryCallCount = 0;

        async ValueTask<string?> Factory(CancellationToken ct)
        {
            Interlocked.Increment(ref factoryCallCount);
            await Task.Delay(50, ct);
            return "shared";
        }

        var tasks = Enumerable.Range(0, 10)
            .Select(_ => _sut.GetOrSetAsync("concurrent-key", Factory, ct: TestContext.Current.CancellationToken).AsTask())
            .ToArray();

        var results = await Task.WhenAll(tasks);

        factoryCallCount.Should().Be(1);
        results.Should().AllBe("shared");
    }

    [Fact]
    public async Task GetOrSetAsync_ConcurrentDifferentKeys_DoNotBlockEachOther()
    {
        int factoryCallCount = 0;

        async ValueTask<string?> Factory(CancellationToken ct)
        {
            Interlocked.Increment(ref factoryCallCount);
            await Task.Delay(20, ct);
            return $"value-{factoryCallCount}";
        }

        var task1 = _sut.GetOrSetAsync("keyA", Factory, ct: TestContext.Current.CancellationToken).AsTask();
        var task2 = _sut.GetOrSetAsync("keyB", Factory, ct: TestContext.Current.CancellationToken).AsTask();

        await Task.WhenAll(task1, task2);

        factoryCallCount.Should().Be(2);
    }

    #endregion

    #region CacheOptions — expiration

    [Fact]
    public async Task SetAsync_WithAbsoluteExpirationRelativeToNow_ExpiresAfterDuration()
    {
        var options = new CacheOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMilliseconds(50)
        };

        await _sut.SetAsync("key1", "ephemeral", options, TestContext.Current.CancellationToken);

        // Immediately available
        _memoryCache.TryGetValue("key1", out string? _).Should().BeTrue();

        // Wait for expiration
        await Task.Delay(100, TestContext.Current.CancellationToken);

        _memoryCache.TryGetValue("key1", out string? _).Should().BeFalse();
    }

    [Fact]
    public async Task SetAsync_WithSlidingExpiration_ExtendsLifetimeOnAccess()
    {
        var options = new CacheOptions
        {
            SlidingExpiration = TimeSpan.FromMilliseconds(100)
        };

        await _sut.SetAsync("key1", "sliding", options, TestContext.Current.CancellationToken);

        // Access within window
        await Task.Delay(50, TestContext.Current.CancellationToken);
        _memoryCache.TryGetValue("key1", out string? _).Should().BeTrue();
        var result = await _sut.GetAsync("key1", TestContext.Current.CancellationToken);
        result.Should().Be("sliding");

        // Still alive after first access extends
        await Task.Delay(60, TestContext.Current.CancellationToken);
        _memoryCache.TryGetValue("key1", out string? _).Should().BeTrue();

        // Wait long enough that it should expire
        await Task.Delay(120, TestContext.Current.CancellationToken);
        _memoryCache.TryGetValue("key1", out string? _).Should().BeFalse();
    }

    [Fact]
    public async Task SetAsync_WithPriority_IsStored()
    {
        var options = new CacheOptions
        {
            Priority = DotSharp.Caching.Abstractions.CacheItemPriority.High
        };

        await _sut.SetAsync("key1", "important", options, TestContext.Current.CancellationToken);

        _memoryCache.TryGetValue("key1", out string? result).Should().BeTrue();
        result.Should().Be("important");
    }

    #endregion
}
