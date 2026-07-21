using DotSharp.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DotSharp.Web.Results;

/// <summary>
/// Maps ASP.NET Core model binding errors to structured <see cref="Error"/> instances
/// with <see cref="ValidationError"/> details.
/// </summary>
public static class ModelBindingErrorMapper
{
    /// <summary>
    /// Converts a <see cref="ModelStateDictionary"/> (typically populated after
    /// failed model binding in MVC controllers) into an <see cref="Error"/>
    /// with code <see cref="ErrorCodes.Validation"/> and individual
    /// <see cref="ValidationError"/> details.
    /// </summary>
    /// <param name="modelState">The model state containing binding errors.</param>
    /// <returns>An <see cref="Error"/> with validation details, or a validation error
    /// with no details if the model state is valid.</returns>
    public static Error ToError(ModelStateDictionary modelState)
    {
        var errors = new List<ValidationError>();

        foreach (var entry in modelState)
        {
            if (entry.Value.Errors.Count == 0)
            {
                continue;
            }

            foreach (var modelError in entry.Value.Errors)
            {
                errors.Add(new ValidationError(entry.Key, "ModelBinding", modelError.ErrorMessage));
            }
        }

        var validationError = Errors.Validation("One or more validation errors occurred.");

        return errors.Count > 0
            ? validationError.WithDetails(errors)
            : validationError;
    }
}
