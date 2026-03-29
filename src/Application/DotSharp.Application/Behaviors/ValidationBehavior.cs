using DotSharp.Application.Abstractions.Behaviors;
using DotSharp.Results;
using FluentValidation;
using FluentValidation.Results;
using System.Reflection;

namespace DotSharp.Application.Behaviors;

/// <summary>
/// Pipeline behavior that validates the request using registered <see cref="IValidator{T}"/> instances.
/// Requires <typeparamref name="TResult"/> to be <see cref="Result"/> or <see cref="Result{T}"/>.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResult">The type of the result. Must be <see cref="Result"/> or <see cref="Result{T}"/>.</typeparam>
public sealed class ValidationBehavior<TRequest, TResult>(IEnumerable<IValidator<TRequest>> validators)
    : IRequestPipelineBehavior<TRequest, TResult>
{
    /// <inheritdoc />
    public async Task<TResult> Handle(TRequest request, Func<Task<TResult>> next, CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next();

        ValidationContext<TRequest> context = new(request);

        ValidationResult[] validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        List<ValidationFailure> failures = [.. validationResults
            .Where(r => r.Errors.Count != 0)
            .SelectMany(r => r.Errors)];

        if (failures.Count == 0)
            return await next();

        List<ValidationFailure> uniqueFailures = [.. failures.DistinctBy(f => (f.PropertyName, f.ErrorCode, f.ErrorMessage))];

        ValidationError[] details = [.. uniqueFailures.Select(f =>
            new ValidationError(
                f.PropertyName,
                f.ErrorCode,
                f.ErrorMessage,
                ExtractPlaceholderValues(f)))];

        Error error = Errors.Validation("Validation failed.")
            .WithDetails(details);

        return BuildFailureResult(error);
    }

    private static string[] ExtractPlaceholderValues(ValidationFailure failure)
    {
        Dictionary<string, object>? placeholders = failure.FormattedMessagePlaceholderValues;

        if (placeholders is null || placeholders.Count == 0)
            return [];

        string[] keys =
        [
            "ComparisonValue", "PropertyValue", "MinLength", "MaxLength",
            "From", "To", "LessThan", "GreaterThan", "EqualTo"
        ];

        return [.. keys
            .Where(k => placeholders.ContainsKey(k))
            .SelectMany(k =>
            {
                object? v = placeholders[k];
                return v switch
                {
                    IEnumerable<object> list => list.Select(x => x?.ToString() ?? string.Empty),
                    _ => [v?.ToString() ?? string.Empty]
                };
            })
            .Where(s => !string.IsNullOrWhiteSpace(s))];
    }

    private static TResult BuildFailureResult(Error error)
    {
        Type resultType = typeof(TResult);

        if (resultType == typeof(Result))
            return (TResult)(object)Result.Failure(error);

        if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            Type valueType = resultType.GetGenericArguments()[0];
            Type genericResultType = typeof(Result<>).MakeGenericType(valueType);
            MethodInfo failureMethod = genericResultType.GetMethod(nameof(Result<>.Failure), BindingFlags.Public | BindingFlags.Static)!;
            return (TResult)failureMethod.Invoke(null, [error])!;
        }

        throw new InvalidOperationException(
            $"ValidationBehavior expects TResult to be Result or Result<T>, but was {resultType.FullName}.");
    }
}
