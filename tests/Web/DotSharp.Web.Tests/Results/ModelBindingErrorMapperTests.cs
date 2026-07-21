using DotSharp.Results;
using DotSharp.Web.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Xunit;

namespace DotSharp.Web.Tests.Results;

/// <summary>
/// Tests for <see cref="ModelBindingErrorMapper.ToError"/> — ModelState → Error.Validation mapping.
/// </summary>
public sealed class ModelBindingErrorMapperTests
{
    [Fact]
    public void ToError_RequiredFieldBindingError_MapsToValidation()
    {
        // Arrange
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("Email", "The Email field is required.");

        // Act
        var error = ModelBindingErrorMapper.ToError(modelState);

        // Assert
        Assert.Equal(ErrorCodes.Validation, error.Code);
        Assert.Contains("One or more validation errors occurred.", error.Message);
        Assert.NotNull(error.Details);
        var detail = Assert.Single(error.Details);
        Assert.Equal("Email", detail.Property);
        Assert.Equal("ModelBinding", detail.Code);
        Assert.Contains("required", detail.Message);
    }

    [Fact]
    public void ToError_InvalidFormatBindingError_IncludesDetail()
    {
        // Arrange
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("Age", "The value 'abc' is not valid for Age.");

        // Act
        var error = ModelBindingErrorMapper.ToError(modelState);

        // Assert
        Assert.NotNull(error.Details);
        var detail = Assert.Single(error.Details);
        Assert.Equal("Age", detail.Property);
        Assert.Equal("ModelBinding", detail.Code);
        Assert.Contains("abc", detail.Message);
    }

    [Fact]
    public void ToError_MultipleErrors_ReturnsAllDetails()
    {
        // Arrange
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("Name", "The Name field is required.");
        modelState.AddModelError("Email", "The Email field is required.");
        modelState.AddModelError("Email", "The Email field is not a valid e-mail address.");

        // Act
        var error = ModelBindingErrorMapper.ToError(modelState);

        // Assert
        Assert.NotNull(error.Details);
        Assert.Equal(3, error.Details.Count);
        Assert.Contains(error.Details, d => d.Property == "Name");
        Assert.Equal(2, error.Details.Count(d => d.Property == "Email"));
    }

    [Fact]
    public void ToError_ValidModelState_ReturnsValidationErrorWithNoDetails()
    {
        // Arrange
        var modelState = new ModelStateDictionary();

        // Act
        var error = ModelBindingErrorMapper.ToError(modelState);

        // Assert
        Assert.Equal(ErrorCodes.Validation, error.Code);
        Assert.NotNull(error.Message);
        Assert.Null(error.Details);
    }
}
