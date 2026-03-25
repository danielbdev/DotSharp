namespace DotSharp.Primitives.Guards;

/// <summary>
/// Provides guard clause methods for argument validation.
/// Throws <see cref="ArgumentException"/>, <see cref="ArgumentNullException"/>, or <see cref="ArgumentOutOfRangeException"/>
/// on validation failure.
/// </summary>
public static class Guard
{
    /// <summary>
    /// Ensures the value is not null.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>The value if not null.</returns>
    /// <exception cref="ArgumentNullException">Thrown when value is null.</exception>
    public static T NotNull<T>(T? value, string parameterName)
    {
        if (value is null)
            throw new ArgumentNullException(parameterName, $"{parameterName} cannot be null.");

        return value;
    }

    /// <summary>
    /// Ensures the string is not null or empty.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>The value if not null or empty.</returns>
    /// <exception cref="ArgumentException">Thrown when value is null or empty.</exception>
    public static string NotNullOrEmpty(string? value, string parameterName)
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentException($"{parameterName} cannot be null or empty.", parameterName);

        return value;
    }

    /// <summary>
    /// Ensures the string is not null, empty, or whitespace.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>The value if not null, empty, or whitespace.</returns>
    /// <exception cref="ArgumentException">Thrown when value is null, empty, or whitespace.</exception>
    public static string NotNullOrWhiteSpace(string? value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{parameterName} cannot be null, empty, or whitespace.", parameterName);

        return value;
    }

    /// <summary>
    /// Ensures the collection is not null or empty.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="value">The collection to check.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>The collection if not null or empty.</returns>
    /// <exception cref="ArgumentException">Thrown when collection is null or empty.</exception>
    public static IEnumerable<T> NotNullOrEmpty<T>(IEnumerable<T>? value, string parameterName)
    {
        if (value is null || !value.Any())
            throw new ArgumentException($"{parameterName} cannot be null or empty.", parameterName);

        return value;
    }

    /// <summary>
    /// Ensures the value is greater than zero.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>The value if greater than zero.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when value is not greater than zero.</exception>
    public static int Positive(int value, string parameterName)
    {
        if (value <= 0)
            throw new ArgumentOutOfRangeException(parameterName, value, $"{parameterName} must be greater than zero.");

        return value;
    }

    /// <summary>
    /// Ensures the value is greater than zero.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>The value if greater than zero.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when value is not greater than zero.</exception>
    public static decimal Positive(decimal value, string parameterName)
    {
        if (value <= 0)
            throw new ArgumentOutOfRangeException(parameterName, value, $"{parameterName} must be greater than zero.");

        return value;
    }

    /// <summary>
    /// Ensures the value is greater than zero.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>The value if greater than zero.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when value is not greater than zero.</exception>
    public static double Positive(double value, string parameterName)
    {
        if (value <= 0)
            throw new ArgumentOutOfRangeException(parameterName, value, $"{parameterName} must be greater than zero.");

        return value;
    }

    /// <summary>
    /// Ensures the value is zero or greater.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>The value if zero or greater.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when value is negative.</exception>
    public static int NotNegative(int value, string parameterName)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(parameterName, value, $"{parameterName} cannot be negative.");

        return value;
    }

    /// <summary>
    /// Ensures the value is within the specified range (inclusive).
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="min">The minimum value (inclusive).</param>
    /// <param name="max">The maximum value (inclusive).</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>The value if within range.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when value is out of range.</exception>
    public static int InRange(int value, int min, int max, string parameterName)
    {
        if (value < min || value > max)
            throw new ArgumentOutOfRangeException(parameterName, value, $"{parameterName} must be between {min} and {max}.");

        return value;
    }

    /// <summary>
    /// Ensures the value is within the specified range (inclusive).
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="min">The minimum value (inclusive).</param>
    /// <param name="max">The maximum value (inclusive).</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>The value if within range.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when value is out of range.</exception>
    public static double InRange(double value, int min, int max, string parameterName)
    {
        if (value < min || value > max)
            throw new ArgumentOutOfRangeException(parameterName, value, $"{parameterName} must be between {min} and {max}.");

        return value;
    }

    /// <summary>
    /// Ensures the value is within the specified range (inclusive).
    /// </summary>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="parameterName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static decimal InRange(decimal value, int min, int max, string parameterName)
    {
        if (value < min || value > max)
            throw new ArgumentOutOfRangeException(parameterName, value, $"{parameterName} must be between {min} and {max}.");

        return value;
    }

    /// <summary>
    /// Ensures the string length is within the specified range.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="minLength">The minimum length (inclusive).</param>
    /// <param name="maxLength">The maximum length (inclusive).</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>The trimmed value if the length is within range.</returns>
    /// <exception cref="ArgumentException">Thrown when value is null/whitespace or length is out of range.</exception>
    public static string LengthInRange(string? value, int minLength, int maxLength, string parameterName)
    {
        value = NotNullOrWhiteSpace(value, parameterName).Trim();

        if (value.Length < minLength || value.Length > maxLength)
            throw new ArgumentException($"{parameterName} length must be between {minLength} and {maxLength} characters.", parameterName);

        return value;
    }

    /// <summary>
    /// Ensures the GUID is not empty.
    /// </summary>
    /// <param name="value">The GUID to check.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>The GUID if not empty.</returns>
    /// <exception cref="ArgumentException">Thrown when GUID is empty.</exception>
    public static Guid NotEmpty(Guid value, string parameterName)
    {
        if (value == Guid.Empty)
            throw new ArgumentException($"{parameterName} cannot be empty.", parameterName);

        return value;
    }

    /// <summary>
    /// Ensures the value is not the default value for the given struct type.
    /// </summary>
    /// <typeparam name="T">The struct type.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>The value if it is not default.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is default.</exception>
    public static T NotDefault<T>(T value, string parameterName)
        where T : struct
    {
        if (EqualityComparer<T>.Default.Equals(value, default))
            throw new ArgumentException($"{parameterName} cannot be default.", parameterName);

        return value;
    }

    /// <summary>
    /// Ensures the string matches the specified regular expression.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="regex">The regular expression to match against.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="message">The error message to use when the match fails.</param>
    /// <returns>The value if it matches the regex.</returns>
    /// <exception cref="ArgumentException">Thrown when the value does not match the regex.</exception>
    public static string Matches(string value, System.Text.RegularExpressions.Regex regex, string parameterName, string message)
    {
        NotNull(value, parameterName);

        if (!regex.IsMatch(value))
            throw new ArgumentException(message, parameterName);

        return value;
    }

    /// <summary>
    /// Ensures the integer value has at most the specified number of digits.
    /// Useful for constraints like "max 6 digits" without hardcoding numeric limits.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="maxDigits">The maximum number of digits allowed (must be greater than zero).</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>The value if it has at most <paramref name="maxDigits"/> digits.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when maxDigits is invalid or when value exceeds the digit limit.</exception>
    public static int MaxDigits(int value, int maxDigits, string parameterName)
    {
        NotNegative(value, parameterName);

        if (maxDigits <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxDigits), maxDigits, "maxDigits must be greater than zero.");

        var maxValue = (int)Math.Pow(10, maxDigits) - 1;

        if (value > maxValue)
            throw new ArgumentOutOfRangeException(parameterName, value, $"{parameterName} must have at most {maxDigits} digits.");

        return value;
    }

    /// <summary>
    /// Ensures all elements in the collection are not null.
    /// </summary>
    /// <typeparam name="T">The reference type of items in the collection.</typeparam>
    /// <param name="value">The collection to check.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>The collection with non-null items.</returns>
    /// <exception cref="ArgumentException">Thrown when the collection contains null elements.</exception>
    public static IEnumerable<T> AllNotNull<T>(IEnumerable<T?> value, string parameterName)
        where T : class
    {
        NotNull(value, parameterName);

        if (value.Any(x => x is null))
            throw new ArgumentException($"{parameterName} cannot contain null items.", parameterName);

        return value!.Select(x => x!);
    }

    /// <summary>
    /// Ensures all strings in the collection are not null, empty, or whitespace.
    /// </summary>
    /// <param name="value">The collection to check.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>A trimmed sequence of strings.</returns>
    /// <exception cref="ArgumentException">Thrown when any element is null/empty/whitespace.</exception>
    public static IEnumerable<string> AllNotNullOrWhiteSpace(IEnumerable<string?> value, string parameterName)
    {
        NotNull(value, parameterName);

        if (value.Any(string.IsNullOrWhiteSpace))
            throw new ArgumentException($"{parameterName} cannot contain null/empty/whitespace items.", parameterName);

        return value!.Select(x => x!.Trim());
    }

    /// <summary>
    /// Ensures the value belongs to the allowed set.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="allowed">The allowed set.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>The value if it belongs to the allowed set.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not in the allowed set.</exception>
    public static T In<T>(T value, IReadOnlyCollection<T> allowed, string parameterName)
    {
        NotNull(allowed, nameof(allowed));

        if (!allowed.Contains(value))
            throw new ArgumentException($"{parameterName} has an invalid value.", parameterName);

        return value;
    }

    /// <summary>
    /// Ensures the condition is false; otherwise throws an exception.
    /// </summary>
    /// <param name="condition">The condition that must be false.</param>
    /// <param name="message">The error message.</param>
    /// <exception cref="ArgumentException">Thrown when condition is true.</exception>
    public static void Against(bool condition, string message)
    {
        if (condition)
            throw new ArgumentException(message);
    }

    /// <summary>
    /// Ensures the condition is false; otherwise throws an exception.
    /// </summary>
    /// <param name="condition">The condition that must be false.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="message">The error message.</param>
    /// <exception cref="ArgumentException">Thrown when condition is true.</exception>
    public static void Against(bool condition, string parameterName, string message)
    {
        if (condition)
            throw new ArgumentException(message, parameterName);
    }
}