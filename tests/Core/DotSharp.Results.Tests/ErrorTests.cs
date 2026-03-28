using FluentAssertions;
using Xunit;

namespace DotSharp.Results.Tests;

public sealed class ErrorTests
{
    #region Constructor

    [Fact]
    public void Error_SetsCode()
    {
        Error error = new Error(ErrorCodes.NotFound, "not found");

        error.Code.Should().Be(ErrorCodes.NotFound);
    }

    [Fact]
    public void Error_SetsMessage()
    {
        Error error = new Error(ErrorCodes.NotFound, "not found");

        error.Message.Should().Be("not found");
    }

    [Fact]
    public void Error_WhenNoMetadata_MetadataIsNull()
    {
        Error error = new Error(ErrorCodes.NotFound, "not found");

        error.Metadata.Should().BeNull();
    }

    [Fact]
    public void Error_WhenNoDetails_DetailsIsNull()
    {
        Error error = new Error(ErrorCodes.NotFound, "not found");

        error.Details.Should().BeNull();
    }

    #endregion

    #region With

    [Fact]
    public void With_AddsMetadataEntry()
    {
        Error error = new Error(ErrorCodes.NotFound, "not found");

        Error result = error.With("orderId", "123");

        result.Metadata.Should().ContainKey("orderId")
            .WhoseValue.Should().Be("123");
    }

    [Fact]
    public void With_ReplacesExistingMetadataEntry()
    {
        Error error = new Error(ErrorCodes.NotFound, "not found")
            .With("orderId", "123");

        Error result = error.With("orderId", "456");

        result.Metadata!["orderId"].Should().Be("456");
    }

    [Fact]
    public void With_ReturnsNewInstance()
    {
        Error error = new Error(ErrorCodes.NotFound, "not found");

        Error result = error.With("orderId", "123");

        result.Should().NotBeSameAs(error);
    }

    [Fact]
    public void With_DoesNotMutateOriginal()
    {
        Error error = new Error(ErrorCodes.NotFound, "not found");

        error.With("orderId", "123");

        error.Metadata.Should().BeNull();
    }

    [Fact]
    public void With_PreservesExistingMetadata()
    {
        Error error = new Error(ErrorCodes.NotFound, "not found")
            .With("orderId", "123");

        Error result = error.With("customerId", "456");

        result.Metadata.Should().ContainKey("orderId")
            .And.ContainKey("customerId");
    }

    #endregion

    #region WithDetails

    [Fact]
    public void WithDetails_SetsDetails()
    {
        Error error = new Error(ErrorCodes.Validation, "validation failed");
        List<ValidationError> details =
        [
            new("CustomerName", "NotEmpty", "The field 'CustomerName' is required.")
        ];

        var result = error.WithDetails(details);

        result.Details.Should().BeEquivalentTo(details);
    }

    [Fact]
    public void WithDetails_ReturnsNewInstance()
    {
        Error error = new Error(ErrorCodes.Validation, "validation failed");
        List<ValidationError> details =
        [
            new("CustomerName", "NotEmpty", "The field 'CustomerName' is required.")
        ];

        var result = error.WithDetails(details);

        result.Should().NotBeSameAs(error);
    }

    [Fact]
    public void WithDetails_DoesNotMutateOriginal()
    {
        Error error = new Error(ErrorCodes.Validation, "validation failed");
        List<ValidationError> details =
        [
            new("CustomerName", "NotEmpty", "The field 'CustomerName' is required.")
        ];

        error.WithDetails(details);

        error.Details.Should().BeNull();
    }

    #endregion
}
