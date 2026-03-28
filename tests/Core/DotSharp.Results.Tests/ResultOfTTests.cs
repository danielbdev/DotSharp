using FluentAssertions;
using Xunit;

namespace DotSharp.Results.Tests;

public sealed class ResultOfTTests
{
    #region Success

    [Fact]
    public void Success_IsSuccess_ReturnsTrue()
    {
        Result<Guid> result = Result<Guid>.Success(Guid.NewGuid());

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Success_IsFailure_ReturnsFalse()
    {
        Result<Guid> result = Result<Guid>.Success(Guid.NewGuid());

        result.IsFailure.Should().BeFalse();
    }

    [Fact]
    public void Success_Error_IsNull()
    {
        Result<Guid> result = Result<Guid>.Success(Guid.NewGuid());

        result.Error.Should().BeNull();
    }

    [Fact]
    public void Success_Value_ReturnsValue()
    {
        Guid id = Guid.NewGuid();

        Result<Guid> result = Result<Guid>.Success(id);

        result.Value.Should().Be(id);
    }

    #endregion

    #region Failure

    [Fact]
    public void Failure_IsSuccess_ReturnsFalse()
    {
        Result<Guid> result = Result<Guid>.Failure(Errors.NotFound("not found"));

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void Failure_IsFailure_ReturnsTrue()
    {
        Result<Guid> result = Result<Guid>.Failure(Errors.NotFound("not found"));

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Failure_Error_IsSet()
    {
        Error error = Errors.NotFound("not found");

        Result<Guid> result = Result<Guid>.Failure(error);

        result.Error.Should().Be(error);
    }

    [Fact]
    public void Failure_WhenErrorIsNull_ThrowsArgumentNullException()
    {
        Action act = () => Result<Guid>.Failure(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Failure_Value_ThrowsInvalidOperationException()
    {
        Result<Guid> result = Result<Guid>.Failure(Errors.NotFound("not found"));

        Action act = () => _ = result.Value;

        act.Should().Throw<InvalidOperationException>();
    }

    #endregion

    #region Implicit operators

    [Fact]
    public void ImplicitOperator_FromValue_CreatesSuccessResult()
    {
        Guid id = Guid.NewGuid();

        Result<Guid> result = id;

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(id);
    }

    [Fact]
    public void ImplicitOperator_FromError_CreatesFailureResult()
    {
        Error error = Errors.NotFound("not found");

        Result<Guid> result = error;

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    #endregion

    #region ToString

    [Fact]
    public void ToString_WhenSuccess_ReturnsSuccessString()
    {
        Result<Guid> result = Result<Guid>.Success(Guid.NewGuid());

        result.ToString().Should().Be($"Success({typeof(Guid).Name})");
    }

    [Fact]
    public void ToString_WhenFailure_ReturnsFailureString()
    {
        Error error = Errors.NotFound("not found");

        Result<Guid> result = Result<Guid>.Failure(error);

        result.ToString().Should().Be($"Failure({error.Code}: {error.Message})");
    }

    #endregion
}
