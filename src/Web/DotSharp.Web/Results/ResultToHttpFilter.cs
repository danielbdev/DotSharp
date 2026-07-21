using DotSharp.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DotSharp.Web.Results;

/// <summary>
/// MVC result filter that automatically unwraps <see cref="Result{T}"/> and <see cref="Result"/>
/// returned by controller actions. This avoids mapping errors manually in each controller.
/// </summary>
/// <remarks>
/// Initializes the filter with an error-to-http mapper.
/// </remarks>
public sealed class ResultToHttpFilter(IErrorHttpMapper mapper) : IAsyncResultFilter
{
    /// <inheritdoc />
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        // Controllers that already return IActionResult explicitly are left untouched,
        // except for ObjectResult containing a Result/Result<T>.
        if (context.Result is ObjectResult objectResult && objectResult.Value is not null)
        {
            object? value = objectResult.Value;
            Type? valueType = value.GetType();

            // Handle Result (non-generic)
            if (value is Result r)
            {
                context.Result = r.IsSuccess
                    ? new NoContentResult()
                    : ToProblemResult(r.Error!);

                await next();
                return;
            }

            // Handle Result<T>
            if (IsResultOfT(valueType))
            {
                context.Result = UnwrapGenericResult(value);
                await next();
                return;
            }
        }

        await next();
    }

    private ObjectResult ToProblemResult(Error error)
    {
        int status = mapper.MapStatusCode(error);
        var problem = mapper.MapProblemDetails(error, status);
        return new ObjectResult(problem) { StatusCode = status };
    }

    private static bool IsResultOfT(Type t)
        => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Result<>);

    private IActionResult UnwrapGenericResult(object boxedResult)
    {
        // boxedResult is Result<T> but we don't know T at compile time.
        // Use reflection to access IsSuccess, Value, Error.
        var t = boxedResult.GetType();

        bool isSuccess = (bool)t.GetProperty(nameof(Result<int>.IsSuccess))!.GetValue(boxedResult)!;
        if (isSuccess)
        {
            object? val = t.GetProperty("Value")!.GetValue(boxedResult);
            return new OkObjectResult(val);
        }

        var error = (Error)t.GetProperty(nameof(Result<object>.Error))!.GetValue(boxedResult)!;
        return ToProblemResult(error);
    }
}
