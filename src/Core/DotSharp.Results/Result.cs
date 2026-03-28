namespace DotSharp.Results;

/// <summary>
/// Represents the outcome of an operation with no return value.
/// Success/failure is modeled as data rather than exceptions.
/// </summary>
public readonly struct Result
{
    private Result(bool isSuccess, Error? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Indicates whether the operation succeeded.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Indicates whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Error information when failure; null when success.
    /// </summary>
    public Error? Error { get; }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static Result Success() => new(true, null);

    /// <summary>
    /// Creates a failure result.
    /// </summary>
    /// <param name="error">The error information.</param>
    public static Result Failure(Error error)
        => new(false, error ?? throw new ArgumentNullException(nameof(error)));

    /// <summary>
    /// Allows returning an <see cref="Error"/> directly where <see cref="Result"/> is expected.
    /// </summary>
    public static implicit operator Result(Error error) => Failure(error);

    /// <inheritdoc />
    public override string ToString() => IsSuccess ? "Success" : $"Failure({Error?.Code}: {Error?.Message})";
}

/// <summary>
/// Represents the outcome of an operation with a return value.
/// Success/failure is modeled as data rather than exceptions.
/// </summary>
/// <typeparam name="T">Value type on success.</typeparam>
public readonly struct Result<T>
{
    private readonly T? _value;

    private Result(bool isSuccess, T? value, Error? error)
    {
        IsSuccess = isSuccess;
        _value = value;
        Error = error;
    }

    /// <summary>
    /// Indicates whether the operation succeeded.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Indicates whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Error information when failure; null when success.
    /// </summary>
    public Error? Error { get; }

    /// <summary>
    /// The value when success; throws if accessed on failure.
    /// </summary>
    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access Value when result is failure.");

    /// <summary>
    /// Creates a successful result with a value.
    /// </summary>
    public static Result<T> Success(T value) => new(true, value, null);

    /// <summary>
    /// Creates a failure result.
    /// </summary>
    /// <param name="error">The error information.</param>
    public static Result<T> Failure(Error error)
        => new(false, default, error ?? throw new ArgumentNullException(nameof(error)));

    /// <summary>
    /// Allows returning a value directly where <see cref="Result{T}"/> is expected.
    /// </summary>
    public static implicit operator Result<T>(T value) => Success(value);

    /// <summary>
    /// Allows returning an <see cref="Error"/> directly where <see cref="Result{T}"/> is expected.
    /// </summary>
    public static implicit operator Result<T>(Error error) => Failure(error);

    /// <inheritdoc />
    public override string ToString() => IsSuccess ? $"Success({typeof(T).Name})" : $"Failure({Error?.Code}: {Error?.Message})";
}
