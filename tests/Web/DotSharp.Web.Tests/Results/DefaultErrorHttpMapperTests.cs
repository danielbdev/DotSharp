using DotSharp.Results;
using DotSharp.Web.Results;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace DotSharp.Web.Tests.Results;

/// <summary>
/// Tests for <see cref="DefaultErrorHttpMapper"/> — ErrorCode → HTTP status code
/// and Error → ProblemDetails mapping.
/// </summary>
public sealed class DefaultErrorHttpMapperTests
{
    [Fact]
    public void MapStatusCode_ValidationError_Returns400()
    {
        // Arrange
        var mapper = new DefaultErrorHttpMapper();
        var error = Errors.Validation("Invalid.");

        // Act
        var statusCode = mapper.MapStatusCode(error);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, statusCode);
    }

    [Fact]
    public void MapStatusCode_NotFoundError_Returns404()
    {
        // Arrange
        var mapper = new DefaultErrorHttpMapper();
        var error = Errors.NotFound("Missing.");

        // Act
        var statusCode = mapper.MapStatusCode(error);

        // Assert
        Assert.Equal(StatusCodes.Status404NotFound, statusCode);
    }

    [Fact]
    public void MapStatusCode_ConflictError_Returns409()
    {
        // Arrange
        var mapper = new DefaultErrorHttpMapper();
        var error = Errors.Conflict("Duplicate.");

        // Act
        var statusCode = mapper.MapStatusCode(error);

        // Assert
        Assert.Equal(StatusCodes.Status409Conflict, statusCode);
    }

    [Fact]
    public void MapStatusCode_ForbiddenError_Returns403()
    {
        // Arrange
        var mapper = new DefaultErrorHttpMapper();
        var error = Errors.Forbidden("Denied.");

        // Act
        var statusCode = mapper.MapStatusCode(error);

        // Assert
        Assert.Equal(StatusCodes.Status403Forbidden, statusCode);
    }

    [Fact]
    public void MapStatusCode_UnauthorizedError_Returns401()
    {
        // Arrange
        var mapper = new DefaultErrorHttpMapper();
        var error = Errors.Unauthorized("Auth required.");

        // Act
        var statusCode = mapper.MapStatusCode(error);

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, statusCode);
    }

    [Fact]
    public void MapStatusCode_UnexpectedError_Returns500()
    {
        // Arrange
        var mapper = new DefaultErrorHttpMapper();
        var error = Errors.Unexpected("Boom.");

        // Act
        var statusCode = mapper.MapStatusCode(error);

        // Assert
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCode);
    }

    [Fact]
    public void MapStatusCode_UnknownErrorCode_DefaultsTo500()
    {
        // Arrange
        var mapper = new DefaultErrorHttpMapper();
        var error = new Error("unknown_code", "Unknown error.");

        // Act
        var statusCode = mapper.MapStatusCode(error);

        // Assert
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCode);
    }

    [Fact]
    public void MapProblemDetails_SetsTitleAndDetail()
    {
        // Arrange
        var mapper = new DefaultErrorHttpMapper();
        var error = new Error("test_code", "Something went wrong.");
        const int statusCode = 500;

        // Act
        var problem = mapper.MapProblemDetails(error, statusCode);

        // Assert
        Assert.NotNull(problem);
        Assert.Equal(statusCode, problem.Status);
        Assert.Equal("test_code", problem.Title);
        Assert.Equal("Something went wrong.", problem.Detail);
    }

    [Fact]
    public void MapProblemDetails_IncludesMetadataInExtensions()
    {
        // Arrange
        var mapper = new DefaultErrorHttpMapper();
        var error = Errors.NotFound("Not found.")
            .With("entityId", 42)
            .With("entityType", "Order");
        const int statusCode = 404;

        // Act
        var problem = mapper.MapProblemDetails(error, statusCode);

        // Assert
        Assert.NotNull(problem.Extensions);
        Assert.True(problem.Extensions.TryGetValue("metadata", out var metadata));
    }

    [Fact]
    public void MapProblemDetails_IncludesDetailsInExtensions_WhenPresent()
    {
        // Arrange
        var mapper = new DefaultErrorHttpMapper();
        var details = new List<ValidationError>
        {
            new("Name", "NotEmpty", "Name is required."),
            new("Age", "GreaterThan", "Age must be greater than 0.", ["0"]),
        };
        var error = Errors.Validation("Validation failed.").WithDetails(details);
        const int statusCode = 400;

        // Act
        var problem = mapper.MapProblemDetails(error, statusCode);

        // Assert
        Assert.NotNull(problem.Extensions);
        Assert.True(problem.Extensions.TryGetValue("details", out var detailsValue));
        var detailList = Assert.IsAssignableFrom<IReadOnlyList<ValidationError>>(detailsValue);
        Assert.Equal(2, detailList.Count);
    }
}
