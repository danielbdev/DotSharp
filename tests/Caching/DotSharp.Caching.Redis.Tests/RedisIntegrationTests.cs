using DotSharp.Caching.Abstractions;
using DotSharp.Caching.Redis;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using Testcontainers.Redis;
using Xunit;

namespace DotSharp.Caching.Redis.Tests;

/// <summary>
/// Integration tests for <see cref="RedisCacheService{T}"/> using a real Redis container via Testcontainers.
/// Requires Docker to be running. Skipped automatically when Docker is unavailable.
/// </summary>
public sealed class RedisIntegrationTests : IAsyncDisposable
{
    private sealed record TestPayload(string Name, int Value);

    private RedisContainer? _redisContainer;
    private RedisCacheService<TestPayload>? _sut;
    private ISerializer? _serializer;

    private async Task EnsureContainerAsync()
    {
        if (_sut is not null)
        {
            return;
        }

        try
        {
            _redisContainer = new RedisBuilder("redis:7-alpine").Build();
            await _redisContainer.StartAsync();
        }
        catch (Exception ex) when (ex.Message.Contains("Docker", StringComparison.OrdinalIgnoreCase))
        {
            Assert.Skip("Docker is not available. Skipping Redis integration tests.");
            return; // unreachable
        }

        var options = Options.Create(new RedisCacheOptions
        {
            Configuration = _redisContainer.GetConnectionString()
        });

        IDistributedCache distributedCache = new RedisCache(options);
        _serializer = new SystemTextJsonSerializer();
        _sut = new RedisCacheService<TestPayload>(distributedCache, _serializer);
    }

    public async ValueTask DisposeAsync()
    {
        if (_redisContainer is not null)
        {
            try
            {
                await _redisContainer.DisposeAsync();
            }
            catch
            {
                // Container cleanup failure is not a test failure
            }
        }
    }

    [Fact]
    public async Task GetAsync_WhenEntryExists_ReturnsDeserializedValue()
    {
        await EnsureContainerAsync();

        var payload = new TestPayload("redis-value", 99);
        await _sut!.SetAsync("int-key1", payload, ct: TestContext.Current.CancellationToken);

        var result = await _sut.GetAsync("int-key1", TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        result!.Name.Should().Be("redis-value");
        result.Value.Should().Be(99);
    }

    [Fact]
    public async Task GetAsync_WhenEntryDoesNotExist_ReturnsNull()
    {
        await EnsureContainerAsync();

        var result = await _sut!.GetAsync("int-nonexistent", TestContext.Current.CancellationToken);

        result.Should().BeNull();
    }

    [Fact]
    public async Task SetAsync_StoresValue()
    {
        await EnsureContainerAsync();

        var payload = new TestPayload("stored-in-redis", 42);

        await _sut!.SetAsync("int-key2", payload, ct: TestContext.Current.CancellationToken);

        var result = await _sut.GetAsync("int-key2", TestContext.Current.CancellationToken);
        result.Should().NotBeNull();
        result!.Name.Should().Be("stored-in-redis");
        result.Value.Should().Be(42);
    }

    [Fact]
    public async Task GetOrSetAsync_OnMiss_InvokesFactoryAndStores()
    {
        await EnsureContainerAsync();

        var result = await _sut!.GetOrSetAsync(
            "int-key3",
            _ => ValueTask.FromResult<TestPayload?>(new TestPayload("fresh-redis", 7)),
            ct: TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        result!.Name.Should().Be("fresh-redis");

        // Subsequent call should return cached value
        var cached = await _sut.GetAsync("int-key3", TestContext.Current.CancellationToken);
        cached.Should().NotBeNull();
        cached!.Name.Should().Be("fresh-redis");
    }

    [Fact]
    public async Task GetOrSetAsync_WhenEntryExists_ReturnsCachedAndSkipsFactory()
    {
        await EnsureContainerAsync();

        var payload = new TestPayload("existing-redis", 100);
        await _sut!.SetAsync("int-key4", payload, ct: TestContext.Current.CancellationToken);
        var factoryCalled = false;

        var result = await _sut.GetOrSetAsync(
            "int-key4",
            _ =>
            {
                factoryCalled = true;
                return ValueTask.FromResult<TestPayload?>(new TestPayload("factory", 0));
            },
            ct: TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        result!.Name.Should().Be("existing-redis");
        factoryCalled.Should().BeFalse();
    }

    [Fact]
    public async Task RemoveAsync_RemovesEntry()
    {
        await EnsureContainerAsync();

        var payload = new TestPayload("to-remove-redis", 1);
        await _sut!.SetAsync("int-key5", payload, ct: TestContext.Current.CancellationToken);

        await _sut.RemoveAsync("int-key5", TestContext.Current.CancellationToken);

        var result = await _sut.GetAsync("int-key5", TestContext.Current.CancellationToken);
        result.Should().BeNull();
    }

    [Fact]
    public async Task ExistsAsync_ReturnsCorrectPresence()
    {
        await EnsureContainerAsync();

        await _sut!.SetAsync("int-key6", new TestPayload("exists-check", 1), ct: TestContext.Current.CancellationToken);

        var exists = await _sut.ExistsAsync("int-key6", TestContext.Current.CancellationToken);
        exists.Should().BeTrue();

        var notExists = await _sut.ExistsAsync("int-key99", TestContext.Current.CancellationToken);
        notExists.Should().BeFalse();
    }

    [Fact]
    public async Task SetAsync_WithExpiration_ExpiresAfterDuration()
    {
        await EnsureContainerAsync();

        var options = new CacheOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(1)
        };

        await _sut!.SetAsync("int-expiring", new TestPayload("ephemeral", 1), options, TestContext.Current.CancellationToken);

        // Immediately available
        var immediate = await _sut.GetAsync("int-expiring", TestContext.Current.CancellationToken);
        immediate.Should().NotBeNull();

        // Wait for expiration
        await Task.Delay(1500, TestContext.Current.CancellationToken);

        var expired = await _sut.GetAsync("int-expiring", TestContext.Current.CancellationToken);
        expired.Should().BeNull();
    }
}
