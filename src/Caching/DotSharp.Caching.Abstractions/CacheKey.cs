namespace DotSharp.Caching.Abstractions;

/// <summary>
/// Represents a structured cache key with compile-time type tracking.
/// Supports implicit conversion from <see cref="string"/> and a fluent builder
/// that produces keys in the format "prefix:T:seg1:seg2".
/// </summary>
public readonly struct CacheKey
{
    private readonly string _value;

    internal CacheKey(string value) => _value = value;

    /// <summary>
    /// Returns a new <see cref="CacheKeyBuilder"/> scoped to the specified type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type that this cache key is associated with.</typeparam>
    public static CacheKeyBuilder For<T>() => new(typeof(T));

    /// <summary>
    /// Allows a <see cref="string"/> to be used where a <see cref="CacheKey"/> is expected.
    /// </summary>
    public static implicit operator CacheKey(string key) => new(key);

    /// <summary>
    /// Returns the string representation of this cache key.
    /// </summary>
    /// <returns>The cache key string.</returns>
    public override string ToString() => _value;
}
