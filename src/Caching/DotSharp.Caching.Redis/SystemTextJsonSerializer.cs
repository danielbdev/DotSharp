using System.Text.Json;

namespace DotSharp.Caching.Redis;

/// <summary>
/// Default <see cref="ISerializer"/> implementation using <see cref="System.Text.Json"/>.
/// Thread-safe and allocation-efficient via <c>JsonSerializer.SerializeToUtf8Bytes</c>.
/// </summary>
public sealed class SystemTextJsonSerializer : ISerializer
{
    private readonly JsonSerializerOptions _options;

    /// <summary>
    /// Initializes a new instance with default <see cref="JsonSerializerOptions"/>.
    /// </summary>
    public SystemTextJsonSerializer()
        : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance with the specified <see cref="JsonSerializerOptions"/>.
    /// </summary>
    /// <param name="options">The serialization options, or <c>null</c> for defaults.</param>
    public SystemTextJsonSerializer(JsonSerializerOptions? options)
    {
        _options = options ?? JsonSerializerOptions.Default;
    }

    /// <inheritdoc />
    public byte[] Serialize<T>(T value)
    {
        return JsonSerializer.SerializeToUtf8Bytes(value, _options);
    }

    /// <inheritdoc />
    public T? Deserialize<T>(byte[]? bytes)
    {
        if (bytes is null)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(bytes, _options);
    }
}
