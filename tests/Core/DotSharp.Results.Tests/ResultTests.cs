using FluentAssertions;
using Xunit;

namespace DotSharp.Results.Tests;

public sealed class ResultTests
{
    #region Success

    [Fact]
    public void Success_IsSuccess_ReturnsTrue()
    {
        Result result = Result.Success();

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Success_IsFailure_ReturnsFalse()
    {
        Result result = Result.Success();

        result.IsFailure.Should().BeFalse();
    }

    [Fact]
    public void Success_Error_IsNull()
    {
        Result result = Result.Success();

        result.Error.Should().BeNull();
    }

    #endregion

    #region Failure

    [Fact]
    public void Failure_IsSuccess_ReturnsFalse()
    {
        Result result = Result.Failure(Errors.NotFound("not found"));

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void Failure_IsFailure_ReturnsTrue()
    {
        Result result = Result.Failure(Errors.NotFound("not found"));

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Failure_Error_IsSet()
    {
        Error error = Errors.NotFound("not found");

        Result result = Result.Failure(error);

        result.Error.Should().Be(error);
    }

    [Fact]
    public void Failure_WhenErrorIsNull_ThrowsArgumentNullException()
    {
        Action act = () => Result.Failure(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region Implicit operator

    [Fact]
    public void ImplicitOperator_FromError_CreatesFailureResult()
    {
        Error error = Errors.NotFound("not found");

        Result result = error;

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    #endregion

    #region ToString

    [Fact]
    public void ToString_WhenSuccess_ReturnsSuccessString()
    {
        Result result = Result.Success();

        result.ToString().Should().Be("Success");
    }

    [Fact]
    public void ToString_WhenFailure_ReturnsFailureString()
    {
        Error error = Errors.NotFound("not found");

        Result result = Result.Failure(error);

        result.ToString().Should().Be($"Failure({error.Code}: {error.Message})");
    }

    #endregion
}
