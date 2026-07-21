using DotSharp.Web.Results;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DotSharp.Web.Tests.Results;

/// <summary>
/// Tests for <see cref="ValidationEndpointFilter"/> — endpoint filter validation via FluentValidation.
/// </summary>
public sealed class ValidationEndpointFilterTests
{
    private static ServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddProblemDetails();
        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task InvokeAsync_NoArguments_PassesThrough()
    {
        // Arrange
        var filter = new ValidationEndpointFilter();
        var httpContext = new DefaultHttpContext
        {
            RequestServices = CreateServiceProvider(),
        };
        var context = EndpointFilterInvocationContext.Create<object>(httpContext, new object());
        bool nextCalled = false;
        ValueTask<object?> Next(EndpointFilterInvocationContext _)
        {
            nextCalled = true;
            return ValueTask.FromResult<object?>("passed");
        }

        // Act
        var result = await filter.InvokeAsync(context, Next);

        // Assert
        Assert.True(nextCalled);
        Assert.Equal("passed", result);
    }

    [Fact]
    public async Task InvokeAsync_ArgumentWithoutValidator_PassesThrough()
    {
        // Arrange
        var filter = new ValidationEndpointFilter();
        var arg = new RequestWithoutValidator { Name = "Test" };
        var httpContext = new DefaultHttpContext
        {
            RequestServices = CreateServiceProvider(),
        };
        var context = EndpointFilterInvocationContext.Create(httpContext, arg);
        bool nextCalled = false;
        ValueTask<object?> Next(EndpointFilterInvocationContext _)
        {
            nextCalled = true;
            return ValueTask.FromResult<object?>("passed");
        }

        // Act
        var result = await filter.InvokeAsync(context, Next);

        // Assert
        Assert.True(nextCalled);
        Assert.Equal("passed", result);
    }

    [Fact]
    public async Task InvokeAsync_ValidationFailure_Returns400ProblemDetails()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddProblemDetails();
        services.AddValidatorsFromAssemblyContaining<ValidatableRequestValidator>();
        var filter = new ValidationEndpointFilter();

        var arg = new ValidatableRequest { Name = "" }; // invalid — Name is required
        var httpContext = new DefaultHttpContext
        {
            RequestServices = services.BuildServiceProvider(),
            Response = { Body = new MemoryStream() },
        };
        var context = EndpointFilterInvocationContext.Create(httpContext, arg);
        bool nextCalled = false;
        ValueTask<object?> Next(EndpointFilterInvocationContext _)
        {
            nextCalled = true;
            return ValueTask.FromResult<object?>("should-not-reach");
        }

        // Act
        var result = await filter.InvokeAsync(context, Next);

        // Assert
        Assert.False(nextCalled);
        Assert.IsAssignableFrom<IResult>(result);

        var iResult = (IResult)result!;
        await iResult.ExecuteAsync(httpContext);

        Assert.Equal(400, httpContext.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_ValidationSuccess_PassesThrough()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddProblemDetails();
        services.AddValidatorsFromAssemblyContaining<ValidatableRequestValidator>();
        var filter = new ValidationEndpointFilter();

        var arg = new ValidatableRequest { Name = "Valid Name" };
        var httpContext = new DefaultHttpContext
        {
            RequestServices = services.BuildServiceProvider(),
        };
        var context = EndpointFilterInvocationContext.Create(httpContext, arg);
        bool nextCalled = false;
        ValueTask<object?> Next(EndpointFilterInvocationContext _)
        {
            nextCalled = true;
            return ValueTask.FromResult<object?>("passed");
        }

        // Act
        var result = await filter.InvokeAsync(context, Next);

        // Assert
        Assert.True(nextCalled);
        Assert.Equal("passed", result);
    }

    /// <summary>
    /// Request DTO without a FluentValidation validator.
    /// </summary>
    public sealed class RequestWithoutValidator
    {
        public string Name { get; init; } = string.Empty;
    }

    /// <summary>
    /// Request DTO with a FluentValidation validator.
    /// </summary>
    public sealed class ValidatableRequest
    {
        public string Name { get; init; } = string.Empty;
    }

    /// <summary>
    /// Validator requiring <see cref="ValidatableRequest.Name"/> to be non-empty.
    /// </summary>
    public sealed class ValidatableRequestValidator : AbstractValidator<ValidatableRequest>
    {
        public ValidatableRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        }
    }
}
