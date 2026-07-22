using DotSharp.Primitives.Pagination;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DotSharp.Web.Filters;

/// <summary>
/// MVC result filter that detects <see cref="PaginationResult{T}"/> in controller action results,
/// sets pagination headers, and replaces the response body with just the items list.
/// </summary>
public sealed class PaginationHeaderFilter : IAsyncResultFilter
{
    /// <inheritdoc />
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is ObjectResult objectResult && objectResult.Value is not null)
        {
            var type = objectResult.Value.GetType();

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(PaginationResult<>))
            {
                context.HttpContext.Response.Headers["X-Pagination-PageNumber"] =
                    type.GetProperty(nameof(PaginationResult<object>.PageNumber))!
                        .GetValue(objectResult.Value)!.ToString();
                context.HttpContext.Response.Headers["X-Pagination-PageSize"] =
                    type.GetProperty(nameof(PaginationResult<object>.PageSize))!
                        .GetValue(objectResult.Value)!.ToString();
                context.HttpContext.Response.Headers["X-Pagination-TotalCount"] =
                    type.GetProperty(nameof(PaginationResult<object>.TotalCount))!
                        .GetValue(objectResult.Value)!.ToString();
                context.HttpContext.Response.Headers["X-Pagination-TotalPages"] =
                    type.GetProperty(nameof(PaginationResult<object>.TotalPages))!
                        .GetValue(objectResult.Value)!.ToString();

                // Replace value with just the items list
                objectResult.Value = type.GetProperty(nameof(PaginationResult<object>.Items))!
                    .GetValue(objectResult.Value);
            }
        }

        await next();
    }
}
