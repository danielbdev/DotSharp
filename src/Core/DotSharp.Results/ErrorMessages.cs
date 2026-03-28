namespace DotSharp.Results;

/// <summary>
/// Defines human-readable error messages used across validation and domain errors.
/// </summary>
public static class ErrorMessages
{
    // Numeric / date comparisons
    public const string GreaterThan = "The field '{0}' must be greater than '{1}'.";
    public const string GreaterThanOrEqualTo = "The field '{0}' must be greater than or equal to '{1}'.";
    public const string LessThan = "The field '{0}' must be less than '{1}'.";
    public const string LessThanOrEqualTo = "The field '{0}' must be less than or equal to '{1}'.";
    public const string InclusiveBetween = "The field '{0}' must have a value between '{1}' and '{2}'.";

    // Required / null
    public const string NotNull = "The field '{0}' is required.";
    public const string NotEmpty = "The field '{0}' is required.";

    // Lengths
    public const string MinimumLength = "The field '{0}' must have a minimum length of '{1}' characters.";
    public const string MaximumLength = "The field '{0}' must have a maximum length of '{1}' characters.";
    public const string LengthBetween = "The field '{0}' must have a length between '{1}' and '{2}' characters.";
    public const string ExactLength = "The field '{0}' must have a length of '{1}' characters.";

    // Equality
    public const string EqualTo = "The field '{0}' must be equal to '{1}'.";
    public const string NotEqualTo = "The field '{0}' must be different from '{1}'.";

    // Collections / sets
    public const string IsIn = "The field '{0}' must be one of: '{1}'.";
    public const string AtLeastOneOf = "At least one of the following fields must be specified: {0}.";
    public const string NotEmptyCollection = "The field '{0}' must contain at least one element.";
    public const string Distinct = "The field '{0}' must not contain duplicate values.";

    // Formats
    public const string Matches = "The field '{0}' has an invalid format.";
    public const string InvalidValue = "The field '{0}' is not a valid value.";
    public const string InvalidEmail = "The field '{0}' is not a valid email address.";
    public const string ValidExtensions = "Valid formats for '{0}' are: '{1}'.";
    public const string Url = "The field '{0}' is not a valid URL.";
    public const string PhoneNumber = "The field '{0}' is not a valid phone number.";
    public const string CreditCard = "The field '{0}' is not a valid credit card number.";

    // Dates
    public const string FutureDate = "The field '{0}' cannot be a future date.";
    public const string PastDate = "The field '{0}' cannot be a past date.";
    public const string MustBeAfter = "The field '{0}' must be after '{1}'.";
    public const string MustBeBefore = "The field '{0}' must be before '{1}'.";

    // Character rules
    public const string OnlyCapitalLetters = "The field '{0}' must contain only uppercase letters.";
    public const string OnlyNumbers = "The field '{0}' must contain only numbers.";
    public const string OnlySpacesAndCapitalLetters = "The field '{0}' must contain only uppercase letters and spaces.";
    public const string OnlyLetters = "The field '{0}' must contain only letters.";
    public const string AllowedCharacters = "The field '{0}' can only contain the following characters: {1}.";

    // Numeric rules
    public const string MustBePositive = "The field '{0}' must be a positive number.";

    // Domain / business
    public const string NotFound = "{0} was not found in the system.";

    // Password policy
    public const string PasswordRequireUppercase = "The field '{0}' must contain at least one uppercase letter.";
    public const string PasswordRequireLowercase = "The field '{0}' must contain at least one lowercase letter.";
    public const string PasswordRequireDigit = "The field '{0}' must contain at least one digit.";
    public const string PasswordRequireSpecialCharacter = "The field '{0}' must contain at least one special character.";
}
