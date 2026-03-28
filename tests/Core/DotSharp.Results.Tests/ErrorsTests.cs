using FluentAssertions;
using Xunit;

namespace DotSharp.Results.Tests;

public sealed class ErrorsTests
{
    [Fact]
    public void NotFound_ReturnsErrorWithNotFoundCode()
    {
        Error error = Errors.NotFound("Order not found.");

        error.Code.Should().Be(ErrorCodes.NotFound);
        error.Message.Should().Be("Order not found.");
    }

    [Fact]
    public void Validation_ReturnsErrorWithValidationCode()
    {
        Error error = Errors.Validation("Invalid request.");

        error.Code.Should().Be(ErrorCodes.Validation);
        error.Message.Should().Be("Invalid request.");
    }

    [Fact]
    public void Conflict_ReturnsErrorWithConflictCode()
    {
        Error error = Errors.Conflict("Order already exists.");

        error.Code.Should().Be(ErrorCodes.Conflict);
        error.Message.Should().Be("Order already exists.");
    }

    [Fact]
    public void Forbidden_ReturnsErrorWithForbiddenCode()
    {
        Error error = Errors.Forbidden("You are not allowed.");

        error.Code.Should().Be(ErrorCodes.Forbidden);
        error.Message.Should().Be("You are not allowed.");
    }

    [Fact]
    public void Unauthorized_ReturnsErrorWithUnauthorizedCode()
    {
        Error error = Errors.Unauthorized("Authentication is required.");

        error.Code.Should().Be(ErrorCodes.Unauthorized);
        error.Message.Should().Be("Authentication is required.");
    }

    [Fact]
    public void Unexpected_ReturnsErrorWithUnexpectedCode()
    {
        Error error = Errors.Unexpected("An unexpected error occurred.");

        error.Code.Should().Be(ErrorCodes.Unexpected);
        error.Message.Should().Be("An unexpected error occurred.");
    }
}
