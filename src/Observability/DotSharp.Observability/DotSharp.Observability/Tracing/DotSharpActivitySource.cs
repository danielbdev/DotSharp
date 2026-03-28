using System.Diagnostics;

namespace DotSharp.Observability.Tracing;

/// <summary>
/// Shared <see cref="ActivitySource"/> for creating spans across DotSharp components.
/// </summary>
public static class DotSharpActivitySource
{
    /// <summary>
    /// The shared <see cref="ActivitySource"/> instance.
    /// </summary>
    public static readonly ActivitySource Instance = new("DotSharp");
}
