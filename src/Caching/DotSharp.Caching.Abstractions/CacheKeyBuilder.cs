using System.Text;

namespace DotSharp.Caching.Abstractions;

/// <summary>
/// Fluent builder for constructing structured <see cref="CacheKey"/> instances.
/// Produces keys in the format "prefix:T:seg1:seg2" where T is the type name.
/// </summary>
public sealed class CacheKeyBuilder
{
    private readonly Type _type;
    private string? _prefix;
    private readonly List<string> _segments = [];

    internal CacheKeyBuilder(Type type) => _type = type;

    /// <summary>
    /// Adds a prefix to the key (e.g., "hot" → "hot:Product:42").
    /// </summary>
    /// <param name="prefix">The prefix value.</param>
    public CacheKeyBuilder WithPrefix(string prefix)
    {
        _prefix = prefix;
        return this;
    }

    /// <summary>
    /// Adds a segment to the key (e.g., "42" → "Product:42").
    /// </summary>
    /// <param name="segment">The segment value.</param>
    public CacheKeyBuilder WithSegment(string segment)
    {
        _segments.Add(segment);
        return this;
    }

    /// <summary>
    /// Adds multiple segments to the key at once.
    /// </summary>
    /// <param name="segments">The segment values.</param>
    public CacheKeyBuilder WithSegments(params string[] segments)
    {
        _segments.AddRange(segments);
        return this;
    }

    /// <summary>
    /// Builds the final <see cref="CacheKey"/>.
    /// </summary>
    public CacheKey Build()
    {
        var sb = new StringBuilder();

        if (_prefix is not null)
        {
            sb.Append(_prefix);
            sb.Append(':');
        }

        sb.Append(_type.Name);
        sb.Append(':');

        for (int i = 0; i < _segments.Count; i++)
        {
            if (i > 0)
            {
                sb.Append(':');
            }
            sb.Append(_segments[i]);
        }

        // Remove trailing colon if no segments
        if (_segments.Count == 0)
        {
            sb.Length--;
        }

        return new CacheKey(sb.ToString());
    }

}
