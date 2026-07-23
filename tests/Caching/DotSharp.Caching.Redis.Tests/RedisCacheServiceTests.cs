using System.Collections.Concurrent;
using DotSharp.Caching.Abstractions;
using DotSharp.Caching.Redis;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Xunit;

namespace DotSharp.Caching.Redis.Tests;

public sealed class RedisCacheServiceTests
{
    private sealed record TestPayload(string Name, int Value);

    /// <summary>
    /// In-memory <see cref="IDistributedCache"/> implementation for testing.
    /// Tracks stored entries and last-set options in a <see cref="ConcurrentDictionary{TKey,TValue}"/>.
    /// </summary>
    private sealed class FakeDistributedCache : IDistributedCache
    {
        private readonly ConcurrentDictionary<string, byte[]> _store = new();

        /// <summary>
        /// Gets the last <see cref="DistributedCacheEntryOptions"/> passed to <see cref="SetAsync"/>.
        /// </summary>
        public DistributedCacheEntryOptions? LastOptions { get; private set; }

        /// <summary>
        /// Returns a snapshot of all stored key/value pairs for inspection.
        /// </summary>
        public IReadOnlyDictionary<string, byte[]> Store => _store;

        public byte[]? Get(string key)
        {
            _store.TryGetValue(key, out var value);
            return value;
        }

        public Task<byte[]?> GetAsync(string key, CancellationToken token = default)
        {
            return Task.FromResult(Get(key));
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            _store[key] = value;
            LastOptions = options;
        }

        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            Set(key, value, options);
            return Task.CompletedTask;
        }

        public void Refresh(string key)
        {
            // No-op for tests — real implementations reset sliding expiration
        }

        public Task RefreshAsync(string key, CancellationToken token = default)
        {
            Refresh(key);
            return Task.CompletedTask;
        }

        public void Remove(string key)
        {
            _store.TryRemove(key, out _);
        }

        public Task RemoveAsync(string key, CancellationToken token = default)
        {
            Remove(key);
            return Task.CompletedTask;
        }
    }

    private readonly FakeDistributedCache _cache;
    private readonly ISerializer _serializer;
    private readonly RedisCacheService<TestPayload> _sut;

    public RedisCacheServiceTests()
    {
        _cache = new FakeDistributedCache();
        _serializer = new SystemTextJsonSerializer();
        _sut = new RedisCacheService<TestPayload>(_cache, _serializer);
    }

    #region GetAsync

    [Fact]
    public async Task GetAsync_WhenEntryExists_ReturnsDeserializedValue()
    {
        var payload = new TestPayload("hello", 42);
        var bytes = _serializer.Serialize(payload);
        await _cache.SetAsync("key1", bytes, new DistributedCacheEntryOptions(), TestContext.Current.CancellationToken);

        var result = await _sut.GetAsync("key1", TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        result!.Name.Should().Be("hello");
        result.Value.Should().Be(42);
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
    public async Task SetAsync_SerializesAndStoresValue()
    {
        var payload = new TestPayload("stored", 99);

        await _sut.SetAsync("key1", payload, ct: TestContext.Current.CancellationToken);

        _cache.Store.Should().ContainKey("key1");
        var deserialized = _serializer.Deserialize<TestPayload>(_cache.Store["key1"]);
        deserialized.Should().NotBeNull();
        deserialized!.Name.Should().Be("stored");
        deserialized.Value.Should().Be(99);
    }

    [Fact]
    public async Task SetAsync_WithCacheOptions_MapsToDistributedCacheEntryOptions()
    {
        var options = new CacheOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
            SlidingExpiration = TimeSpan.FromSeconds(30),
        };

        await _sut.SetAsync("key1", new TestPayload("opts", 1), options, TestContext.Current.CancellationToken);

        _cache.LastOptions.Should().NotBeNull();
        _cache.LastOptions!.AbsoluteExpirationRelativeToNow.Should().Be(TimeSpan.FromMinutes(5));
        _cache.LastOptions.SlidingExpiration.Should().Be(TimeSpan.FromSeconds(30));
    }

    [Fact]
    public async Task SetAsync_NullOptions_StoresWithDefaultOptions()
    {
        await _sut.SetAsync("key1", new TestPayload("default", 1), options: null, ct: TestContext.Current.CancellationToken);

        _cache.Store.Should().ContainKey("key1");
        _cache.LastOptions.Should().NotBeNull();
    }

    #endregion

    #region GetOrSetAsync

    [Fact]
    public async Task GetOrSetAsync_WhenEntryExists_ReturnsCachedValue()
    {
        var payload = new TestPayload("cached", 100);
        await _cache.SetAsync("key1", _serializer.Serialize(payload), new DistributedCacheEntryOptions(), TestContext.Current.CancellationToken);

        var result = await _sut.GetOrSetAsync(
            "key1",
            _ => ValueTask.FromResult<TestPayload?>(new TestPayload("factory", 0)),
            ct: TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        result!.Name.Should().Be("cached");
        result.Value.Should().Be(100);
    }

    [Fact]
    public async Task GetOrSetAsync_WhenEntryExists_DoesNotInvokeFactory()
    {
        var payload = new TestPayload("cached", 1);
        await _cache.SetAsync("key1", _serializer.Serialize(payload), new DistributedCacheEntryOptions(), TestContext.Current.CancellationToken);
        var factoryCalled = false;

        await _sut.GetOrSetAsync(
            "key1",
            _ =>
            {
                factoryCalled = true;
                return ValueTask.FromResult<TestPayload?>(new TestPayload("factory", 0));
            },
            ct: TestContext.Current.CancellationToken);

        factoryCalled.Should().BeFalse();
    }

    [Fact]
    public async Task GetOrSetAsync_OnMiss_InvokesFactoryAndStoresResult()
    {
        var result = await _sut.GetOrSetAsync(
            "key1",
            _ => ValueTask.FromResult<TestPayload?>(new TestPayload("fresh", 7)),
            ct: TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        result!.Name.Should().Be("fresh");
        result.Value.Should().Be(7);

        // Verify it was stored
        _cache.Store.Should().ContainKey("key1");
    }

    [Fact]
    public async Task GetOrSetAsync_OnMissWithNullFactoryResult_DoesNotStore()
    {
        var result = await _sut.GetOrSetAsync(
            "key1",
            _ => ValueTask.FromResult<TestPayload?>(null),
            ct: TestContext.Current.CancellationToken);

        result.Should().BeNull();
        _cache.Store.Should().NotContainKey("key1");
    }

    #endregion

    #region RemoveAsync

    [Fact]
    public async Task RemoveAsync_RemovesExistingEntry()
    {
        var payload = new TestPayload("toremove", 1);
        await _cache.SetAsync("key1", _serializer.Serialize(payload), new DistributedCacheEntryOptions(), TestContext.Current.CancellationToken);

        await _sut.RemoveAsync("key1", TestContext.Current.CancellationToken);

        _cache.Store.Should().NotContainKey("key1");
    }

    [Fact]
    public async Task RemoveAsync_WhenKeyDoesNotExist_DoesNotThrow()
    {
        var act = () => _sut.RemoveAsync("no-key", TestContext.Current.CancellationToken).AsTask();

        await act.Should().NotThrowAsync();
    }

    #endregion

    #region ExistsAsync

    [Fact]
    public async Task ExistsAsync_WhenEntryExists_ReturnsTrue()
    {
        var payload = new TestPayload("exists", 1);
        await _cache.SetAsync("key1", _serializer.Serialize(payload), new DistributedCacheEntryOptions(), TestContext.Current.CancellationToken);

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

    #region CacheOptions → DistributedCacheEntryOptions mapping

    [Fact]
    public async Task SetAsync_WithAbsoluteExpiration_MapsCorrectly()
    {
        var absolutePoint = DateTimeOffset.UtcNow.AddHours(1);
        var options = new CacheOptions
        {
            AbsoluteExpiration = absolutePoint,
        };

        await _sut.SetAsync("key1", new TestPayload("abs", 1), options, TestContext.Current.CancellationToken);

        _cache.LastOptions.Should().NotBeNull();
        _cache.LastOptions!.AbsoluteExpiration.Should().Be(absolutePoint);
    }

    [Fact]
    public async Task SetAsync_WithSlidingExpiration_MapsCorrectly()
    {
        var options = new CacheOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(10),
        };

        await _sut.SetAsync("key1", new TestPayload("slide", 1), options, TestContext.Current.CancellationToken);

        _cache.LastOptions.Should().NotBeNull();
        _cache.LastOptions!.SlidingExpiration.Should().Be(TimeSpan.FromMinutes(10));
    }

    #endregion
}
