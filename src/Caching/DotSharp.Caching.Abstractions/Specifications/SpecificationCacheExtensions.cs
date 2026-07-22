using System.Security.Cryptography;
using System.Text;
using DotSharp.Persistence.Abstractions.Specifications;

namespace DotSharp.Caching.Abstractions.Specifications;

/// <summary>
/// Extension methods that generate deterministic cache keys from <see cref="ISpecification{T}"/> instances.
/// The key is built by hashing the expression tree string of the specification's
/// <see cref="ISpecification{T}.Criteria"/> via SHA256 and taking the first 16 characters of the Base64 representation.
/// </summary>
public static class SpecificationCacheExtensions
{
    /// <summary>
    /// Generates a deterministic cache key from the specification's criteria and ordering expressions.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="spec">The specification.</param>
    /// <returns>A 16-character Base64 string derived from SHA256 of the criteria expression.</returns>
    public static string ToCacheKey<T>(this ISpecification<T> spec) where T : class
    {
        var sb = new StringBuilder();

        // Hash the Criteria expression to a stable string
        sb.Append(spec.Criteria.ToString());

        // Include ordering expressions in the key fingerprint
        foreach (var order in spec.OrderExpressions)
        {
            sb.Append('|');
            sb.Append(order.Expression.ToString());
            sb.Append(':');
            sb.Append(order.Direction);
            sb.Append(':');
            sb.Append(order.IsThenBy);
        }

        var input = sb.ToString();
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(input));

        // First 12 bytes → 16 characters of Base64 (12 * 8 / 6 = 16)
        return Convert.ToBase64String(hash.AsSpan(0, 12));
    }
}
