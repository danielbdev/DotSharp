using DotSharp.Web.Correlation;
using Microsoft.AspNetCore.Builder;

namespace DotSharp.Web;

/// <summary>
/// Registration extensions for DotSharp middlewares in the request pipeline.
/// </summary>
public static class AppExtensions
{
    /// <summary>
    /// Adds the global exception handler to the pipeline.
    /// Must be registered via <see cref="ServiceExtensions.AddDotSharpWeb"/> first.
    /// Place it early in the pipeline (before routing/endpoints).
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder for fluent chaining.</returns>
    public static IApplicationBuilder UseDotSharpExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseExceptionHandler();
    }

    /// <summary>
    /// Adds the correlation middleware to the pipeline.
    /// Reads or generates the <c>X-Correlation-Id</c> header and registers
    /// <see cref="DotSharp.Observability.Correlation.ICorrelationContext"/> for the request lifetime.
    /// Must be registered via <see cref="ServiceExtensions.AddDotSharpWeb"/> first.
    /// Place it as the first middleware in the pipeline.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder for fluent chaining.</returns>
    public static IApplicationBuilder UseDotSharpCorrelation(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CorrelationMiddleware>();
    }
}
