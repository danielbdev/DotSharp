using DotSharp.Primitives.Guards;
using FluentAssertions;
using System.Text.RegularExpressions;
using Xunit;

namespace DotSharp.Primitives.Tests.Guards;

public sealed class GuardTests
{
    #region NotNull

    [Fact]
    public void NotNull_WhenValueIsNotNull_ReturnsValue()
    {
        var result = Guard.NotNull("value", "param");

        result.Should().Be("value");
    }

    [Fact]
    public void NotNull_WhenValueIsNull_ThrowsArgumentNullException()
    {
        Action act = () => Guard.NotNull<string>(null, "param");

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("param");
    }

    #endregion

    #region NotNullOrEmpty (string)

    [Fact]
    public void NotNullOrEmpty_WhenValueIsValid_ReturnsValue()
    {
        var result = Guard.NotNullOrEmpty("value", "param");

        result.Should().Be("value");
    }

    [Fact]
    public void NotNullOrEmpty_WhenValueIsNull_ThrowsArgumentException()
    {
        Action act = () => Guard.NotNullOrEmpty(null, "param");

        act.Should().Throw<ArgumentException>()
            .WithParameterName("param");
    }

    [Fact]
    public void NotNullOrEmpty_WhenValueIsEmpty_ThrowsArgumentException()
    {
        Action act = () => Guard.NotNullOrEmpty(string.Empty, "param");

        act.Should().Throw<ArgumentException>()
            .WithParameterName("param");
    }

    #endregion

    #region NotNullOrWhiteSpace

    [Fact]
    public void NotNullOrWhiteSpace_WhenValueIsValid_ReturnsValue()
    {
        var result = Guard.NotNullOrWhiteSpace("value", "param");

        result.Should().Be("value");
    }

    [Fact]
    public void NotNullOrWhiteSpace_WhenValueIsWhiteSpace_ThrowsArgumentException()
    {
        Action act = () => Guard.NotNullOrWhiteSpace("   ", "param");

        act.Should().Throw<ArgumentException>()
            .WithParameterName("param");
    }

    [Fact]
    public void NotNullOrWhiteSpace_WhenValueIsNull_ThrowsArgumentException()
    {
        Action act = () => Guard.NotNullOrWhiteSpace(null, "param");

        act.Should().Throw<ArgumentException>()
            .WithParameterName("param");
    }

    #endregion

    #region NotNullOrEmpty (collection)

    [Fact]
    public void NotNullOrEmpty_WhenCollectionHasItems_ReturnsCollection()
    {
        var collection = new[] { 1, 2, 3 };

        var result = Guard.NotNullOrEmpty(collection, "param");

        result.Should().BeEquivalentTo(collection);
    }

    [Fact]
    public void NotNullOrEmpty_WhenCollectionIsNull_ThrowsArgumentException()
    {
        Action act = () => Guard.NotNullOrEmpty<int>(null, "param");

        act.Should().Throw<ArgumentException>()
            .WithParameterName("param");
    }

    [Fact]
    public void NotNullOrEmpty_WhenCollectionIsEmpty_ThrowsArgumentException()
    {
        Action act = () => Guard.NotNullOrEmpty(Array.Empty<int>(), "param");

        act.Should().Throw<ArgumentException>()
            .WithParameterName("param");
    }

    #endregion

    #region Positive (int)

    [Fact]
    public void Positive_Int_WhenValueIsPositive_ReturnsValue()
    {
        var result = Guard.Positive(5, "param");

        result.Should().Be(5);
    }

    [Fact]
    public void Positive_Int_WhenValueIsZero_ThrowsArgumentOutOfRangeException()
    {
        Action act = () => Guard.Positive(0, "param");

        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("param");
    }

    [Fact]
    public void Positive_Int_WhenValueIsNegative_ThrowsArgumentOutOfRangeException()
    {
        Action act = () => Guard.Positive(-1, "param");

        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("param");
    }

    #endregion

    #region Positive (decimal)

    [Fact]
    public void Positive_Decimal_WhenValueIsPositive_ReturnsValue()
    {
        var result = Guard.Positive(5.5m, "param");

        result.Should().Be(5.5m);
    }

    [Fact]
    public void Positive_Decimal_WhenValueIsZero_ThrowsArgumentOutOfRangeException()
    {
        Action act = () => Guard.Positive(0m, "param");

        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("param");
    }

    #endregion

    #region Positive (double)

    [Fact]
    public void Positive_Double_WhenValueIsPositive_ReturnsValue()
    {
        var result = Guard.Positive(5.5, "param");

        result.Should().Be(5.5);
    }

    [Fact]
    public void Positive_Double_WhenValueIsZero_ThrowsArgumentOutOfRangeException()
    {
        Action act = () => Guard.Positive(0.0, "param");

        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("param");
    }

    #endregion

    #region NotNegative

    [Fact]
    public void NotNegative_WhenValueIsZero_ReturnsValue()
    {
        var result = Guard.NotNegative(0, "param");

        result.Should().Be(0);
    }

    [Fact]
    public void NotNegative_WhenValueIsPositive_ReturnsValue()
    {
        var result = Guard.NotNegative(5, "param");

        result.Should().Be(5);
    }

    [Fact]
    public void NotNegative_WhenValueIsNegative_ThrowsArgumentOutOfRangeException()
    {
        Action act = () => Guard.NotNegative(-1, "param");

        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("param");
    }

    #endregion

    #region InRange (int)

    [Fact]
    public void InRange_Int_WhenValueIsWithinRange_ReturnsValue()
    {
        var result = Guard.InRange(5, 1, 10, "param");

        result.Should().Be(5);
    }

    [Fact]
    public void InRange_Int_WhenValueIsAtMin_ReturnsValue()
    {
        var result = Guard.InRange(1, 1, 10, "param");

        result.Should().Be(1);
    }

    [Fact]
    public void InRange_Int_WhenValueIsAtMax_ReturnsValue()
    {
        var result = Guard.InRange(10, 1, 10, "param");

        result.Should().Be(10);
    }

    [Fact]
    public void InRange_Int_WhenValueIsBelowMin_ThrowsArgumentOutOfRangeException()
    {
        Action act = () => Guard.InRange(0, 1, 10, "param");

        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("param");
    }

    [Fact]
    public void InRange_Int_WhenValueIsAboveMax_ThrowsArgumentOutOfRangeException()
    {
        Action act = () => Guard.InRange(11, 1, 10, "param");

        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("param");
    }

    #endregion

    #region LengthInRange

    [Fact]
    public void LengthInRange_WhenLengthIsValid_ReturnsTrimmedValue()
    {
        var result = Guard.LengthInRange("  hello  ", 1, 10, "param");

        result.Should().Be("hello");
    }

    [Fact]
    public void LengthInRange_WhenLengthIsTooShort_ThrowsArgumentException()
    {
        Action act = () => Guard.LengthInRange("hi", 5, 10, "param");

        act.Should().Throw<ArgumentException>()
            .WithParameterName("param");
    }

    [Fact]
    public void LengthInRange_WhenLengthIsTooLong_ThrowsArgumentException()
    {
        Action act = () => Guard.LengthInRange("hello world", 1, 5, "param");

        act.Should().Throw<ArgumentException>()
            .WithParameterName("param");
    }

    [Fact]
    public void LengthInRange_WhenValueIsWhiteSpace_ThrowsArgumentException()
    {
        Action act = () => Guard.LengthInRange("   ", 1, 10, "param");

        act.Should().Throw<ArgumentException>()
            .WithParameterName("param");
    }

    #endregion

    #region NotEmpty (Guid)

    [Fact]
    public void NotEmpty_WhenGuidIsNotEmpty_ReturnsGuid()
    {
        var id = Guid.NewGuid();

        var result = Guard.NotEmpty(id, "param");

        result.Should().Be(id);
    }

    [Fact]
    public void NotEmpty_WhenGuidIsEmpty_ThrowsArgumentException()
    {
        Action act = () => Guard.NotEmpty(Guid.Empty, "param");

        act.Should().Throw<ArgumentException>()
            .WithParameterName("param");
    }

    #endregion

    #region NotDefault

    [Fact]
    public void NotDefault_WhenValueIsNotDefault_ReturnsValue()
    {
        var result = Guard.NotDefault(42, "param");

        result.Should().Be(42);
    }

    [Fact]
    public void NotDefault_WhenValueIsDefault_ThrowsArgumentException()
    {
        Action act = () => Guard.NotDefault(0, "param");

        act.Should().Throw<ArgumentException>()
            .WithParameterName("param");
    }

    #endregion

    #region Matches

    [Fact]
    public void Matches_WhenValueMatchesRegex_ReturnsValue()
    {
        var regex = new Regex(@"^\d+$");

        var result = Guard.Matches("123", regex, "param", "must be digits");

        result.Should().Be("123");
    }

    [Fact]
    public void Matches_WhenValueDoesNotMatchRegex_ThrowsArgumentException()
    {
        var regex = new Regex(@"^\d+$");

        Action act = () => Guard.Matches("abc", regex, "param", "must be digits");

        act.Should().Throw<ArgumentException>()
            .WithParameterName("param");
    }

    #endregion

    #region MaxDigits

    [Fact]
    public void MaxDigits_WhenValueIsWithinLimit_ReturnsValue()
    {
        var result = Guard.MaxDigits(999, 3, "param");

        result.Should().Be(999);
    }

    [Fact]
    public void MaxDigits_WhenValueExceedsLimit_ThrowsArgumentOutOfRangeException()
    {
        Action act = () => Guard.MaxDigits(1000, 3, "param");

        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("param");
    }

    [Fact]
    public void MaxDigits_WhenMaxDigitsIsZero_ThrowsArgumentOutOfRangeException()
    {
        Action act = () => Guard.MaxDigits(1, 0, "param");

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    #endregion

    #region AllNotNull

    [Fact]
    public void AllNotNull_WhenAllItemsAreNotNull_ReturnsCollection()
    {
        var collection = new[] { "a", "b", "c" };

        var result = Guard.AllNotNull(collection, "param");

        result.Should().BeEquivalentTo(collection);
    }

    [Fact]
    public void AllNotNull_WhenCollectionContainsNull_ThrowsArgumentException()
    {
        var collection = new[] { "a", null, "c" };

        Action act = () => Guard.AllNotNull(collection, "param");

        act.Should().Throw<ArgumentException>()
            .WithParameterName("param");
    }

    #endregion

    #region AllNotNullOrWhiteSpace

    [Fact]
    public void AllNotNullOrWhiteSpace_WhenAllItemsAreValid_ReturnsTrimmedCollection()
    {
        var collection = new[] { " a ", " b ", " c " };

        var result = Guard.AllNotNullOrWhiteSpace(collection, "param");

        result.Should().BeEquivalentTo(["a", "b", "c"]);
    }

    [Fact]
    public void AllNotNullOrWhiteSpace_WhenCollectionContainsWhiteSpace_ThrowsArgumentException()
    {
        var collection = new[] { "a", "   ", "c" };

        Action act = () => Guard.AllNotNullOrWhiteSpace(collection, "param");

        act.Should().Throw<ArgumentException>()
            .WithParameterName("param");
    }

    #endregion

    #region In

    [Fact]
    public void In_WhenValueIsInAllowedSet_ReturnsValue()
    {
        var allowed = new[] { "a", "b", "c" };

        var result = Guard.In("a", allowed, "param");

        result.Should().Be("a");
    }

    [Fact]
    public void In_WhenValueIsNotInAllowedSet_ThrowsArgumentException()
    {
        var allowed = new[] { "a", "b", "c" };

        Action act = () => Guard.In("d", allowed, "param");

        act.Should().Throw<ArgumentException>()
            .WithParameterName("param");
    }

    #endregion

    #region Against

    [Fact]
    public void Against_WhenConditionIsFalse_DoesNotThrow()
    {
        Action act = () => Guard.Against(false, "must not happen");

        act.Should().NotThrow();
    }

    [Fact]
    public void Against_WhenConditionIsTrue_ThrowsArgumentException()
    {
        Action act = () => Guard.Against(true, "must not happen");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Against_WithParameterName_WhenConditionIsTrue_ThrowsArgumentException()
    {
        Action act = () => Guard.Against(true, "param", "must not happen");

        act.Should().Throw<ArgumentException>()
            .WithParameterName("param");
    }

    #endregion
}
