namespace DotSharp.Caching.Redis;

/// <summary>
/// Defines a serialization contract for converting values to and from byte arrays
/// for storage in a distributed cache.
/// </summary>
public interface ISerializer
{
    /// <summary>
    /// Serializes a value to a byte array.
    /// </summary>
    /// <typeparam name="T">The type of the value to serialize.</typeparam>
    /// <param name="value">The value to serialize.</param>
    /// <returns>The serialized byte array.</returns>
    byte[] Serialize<T>(T value);

    /// <summary>
    /// Deserializes a byte array back to a value. Returns <c>default</c> if the bytes are null.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to.</typeparam>
    /// <param name="bytes">The byte array to deserialize, or <c>null</c>.</param>
    /// <returns>The deserialized value, or <c>default</c> if <paramref name="bytes"/> is null.</returns>
    T? Deserialize<T>(byte[]? bytes);
}
