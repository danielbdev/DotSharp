using DotSharp.Results;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DotSharp.Web.Results;

/// <summary>
/// Validates minimal API request parameters using FluentValidation,
/// returning RFC 9457 <see cref="ProblemDetails"/> on validation failure.
/// </summary>
/// <remarks>
/// Resolves <see cref="IValidator{T}"/> per argument from
/// <see cref="HttpContext.RequestServices"/> at runtime.
/// </remarks>
public sealed class ValidationEndpointFilter : IEndpointFilter
{
    /// <inheritdoc />
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        foreach (var arg in context.Arguments)
        {
            if (arg is null)
            {
                continue;
            }

            var validatorType = typeof(IValidator<>).MakeGenericType(arg.GetType());
            var validator = context.HttpContext.RequestServices.GetService(validatorType);

            if (validator is null)
            {
                continue;
            }

            var validationContext = new ValidationContext<object>(arg);
            var result = await ((IValidator)validator).ValidateAsync(validationContext);

            if (result.IsValid)
            {
                continue;
            }

            var details = result.Errors
                .Select(e => new ValidationError(e.PropertyName, e.ErrorCode, e.ErrorMessage))
                .ToList();

            var error = Errors.Validation("Validation failed.").WithDetails(details);

            var pd = new ProblemDetails
            {
                Status = 400,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                Title = "Bad Request",
                Detail = error.Message,
            };

            pd.Extensions["errors"] = error.Details;

            return Microsoft.AspNetCore.Http.Results.Problem(pd);
        }

        return await next(context);
    }
}
