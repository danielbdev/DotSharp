using DotSharp.Results;
using DotSharp.Web.Results;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DotSharp.Web.Tests.Results;

/// <summary>
/// Tests for <see cref="ProblemDetailsFactory"/> — RFC 9457 ProblemDetails creation from <see cref="Error"/> and <see cref="Exception"/>.
/// </summary>
public sealed class ProblemDetailsFactoryTests
{
    [Fact]
    public void Create_FromError_ReturnsProblemDetailsWithDetail()
    {
        // Arrange
        var error = new Error("test_code", "Something went wrong.");

        // Act
        var result = ProblemDetailsFactory.Create(error, correlationId: null, traceId: null);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Something went wrong.", result.Detail);
    }

    [Fact]
    public void Create_FromError_EnrichesCorrelationId()
    {
        // Arrange
        var error = new Error("test_code", "Error.");
        const string correlationId = "abc-123";

        // Act
        var result = ProblemDetailsFactory.Create(error, correlationId, traceId: null);

        // Assert
        Assert.NotNull(result.Extensions);
        Assert.True(result.Extensions.ContainsKey("correlationId"));
        Assert.Equal("abc-123", result.Extensions["correlationId"]);
    }

    [Fact]
    public void Create_FromError_EnrichesTraceId()
    {
        // Arrange
        var error = new Error("test_code", "Error.");
        const string traceId = "xyz-456";

        // Act
        var result = ProblemDetailsFactory.Create(error, correlationId: null, traceId);

        // Assert
        Assert.NotNull(result.Extensions);
        Assert.True(result.Extensions.ContainsKey("traceId"));
        Assert.Equal("xyz-456", result.Extensions["traceId"]);
    }

    [Fact]
    public void Create_FromValidationError_EnrichesErrorsDetail()
    {
        // Arrange
        var details = new List<ValidationError>
        {
            new("Name", "NotEmpty", "Name is required."),
            new("Age", "GreaterThan", "Age must be greater than 0.", ["0"]),
        };
        var error = Errors.Validation("Validation failed.").WithDetails(details);

        // Act
        var result = ProblemDetailsFactory.Create(error, correlationId: null, traceId: null);

        // Assert
        Assert.NotNull(result.Extensions);
        Assert.True(result.Extensions.ContainsKey("errors"));
        var errors = Assert.IsAssignableFrom<IReadOnlyList<ValidationError>>(result.Extensions["errors"]);
        Assert.Equal(2, errors.Count);
    }

    [Fact]
    public void Create_FromError_MergesMetadataSkippingReservedKeys()
    {
        // Arrange
        var error = Errors.NotFound("Not found.")
            .With("entityId", 42)
            .With("entityType", "Order");

        // Act
        var result = ProblemDetailsFactory.Create(error, correlationId: null, traceId: null);

        // Assert
        Assert.NotNull(result.Extensions);
        Assert.Equal(42, result.Extensions["entityId"]);
        Assert.Equal("Order", result.Extensions["entityType"]);
        Assert.False(result.Extensions.ContainsKey("errors"));
        Assert.False(result.Extensions.ContainsKey("correlationId"));
        Assert.False(result.Extensions.ContainsKey("traceId"));
    }

    [Fact]
    public void Create_FromError_ReservedKeysNotOverwrittenByMetadata()
    {
        // Arrange
        var error = Errors.NotFound("Not found.")
            .With("correlationId", "metadata-correlation");

        // Act
        var result = ProblemDetailsFactory.Create(error, correlationId: "actual-correlation", traceId: null);

        // Assert
        Assert.NotNull(result.Extensions);
        // The correlationId from the parameter takes priority over metadata
        Assert.Equal("actual-correlation", result.Extensions["correlationId"]);
    }

    [Fact]
    public void Create_FromException_ReturnsProblemDetails()
    {
        // Arrange
        var exception = new InvalidOperationException("Unexpected failure.");

        // Act
        var result = ProblemDetailsFactory.Create(exception, correlationId: null, traceId: null);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Unexpected failure.", result.Detail);
    }

    [Fact]
    public void Create_FromException_EnrichesCorrelationId()
    {
        // Arrange
        var exception = new InvalidOperationException("Failure.");
        const string correlationId = "exc-corr-123";

        // Act
        var result = ProblemDetailsFactory.Create(exception, correlationId, traceId: null);

        // Assert
        Assert.NotNull(result.Extensions);
        Assert.True(result.Extensions.ContainsKey("correlationId"));
        Assert.Equal("exc-corr-123", result.Extensions["correlationId"]);
    }

    [Fact]
    public void Create_FromException_EnrichesTraceId()
    {
        // Arrange
        var exception = new InvalidOperationException("Failure.");
        const string traceId = "exc-trace-456";

        // Act
        var result = ProblemDetailsFactory.Create(exception, correlationId: null, traceId);

        // Assert
        Assert.NotNull(result.Extensions);
        Assert.True(result.Extensions.ContainsKey("traceId"));
        Assert.Equal("exc-trace-456", result.Extensions["traceId"]);
    }
}
