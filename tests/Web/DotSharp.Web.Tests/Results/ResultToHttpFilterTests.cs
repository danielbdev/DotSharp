using DotSharp.Results;
using DotSharp.Web.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Xunit;

namespace DotSharp.Web.Tests.Results;

/// <summary>
/// Tests for <see cref="ResultToHttpFilter"/> — automatic Result/Result&lt;T&gt; unwrapping.
/// </summary>
public sealed class ResultToHttpFilterTests
{
    private static IErrorHttpMapper CreateMapper() => new DefaultErrorHttpMapper();

    [Fact]
    public async Task OnResultExecution_ResultSuccess_ReturnsNoContent()
    {
        // Arrange
        var filter = new ResultToHttpFilter(CreateMapper());
        var actionContext = new ActionContext(new DefaultHttpContext(), new(), new());
        var objectResult = new ObjectResult(Result.Success());
        var context = new ResultExecutingContext(actionContext, [filter], objectResult, new());
        var executedContext = new ResultExecutedContext(actionContext, [], context.Result, new());
        ResultExecutionDelegate next = () => Task.FromResult(executedContext);

        // Act
        await filter.OnResultExecutionAsync(context, next);

        // Assert
        Assert.IsType<NoContentResult>(context.Result);
    }

    [Fact]
    public async Task OnResultExecution_ResultFailure_ReturnsProblemDetails()
    {
        // Arrange
        var filter = new ResultToHttpFilter(CreateMapper());
        var error = Errors.NotFound("Resource not found.");
        var actionContext = new ActionContext(new DefaultHttpContext(), new(), new());
        var objectResult = new ObjectResult(Result.Failure(error));
        var context = new ResultExecutingContext(actionContext, [filter], objectResult, new());
        var executedContext = new ResultExecutedContext(actionContext, [], context.Result, new());
        ResultExecutionDelegate next = () => Task.FromResult(executedContext);

        // Act
        await filter.OnResultExecutionAsync(context, next);

        // Assert
        var objResult = Assert.IsType<ObjectResult>(context.Result);
        Assert.Equal(StatusCodes.Status404NotFound, objResult.StatusCode);
        var problem = Assert.IsAssignableFrom<ProblemDetails>(objResult.Value);
        Assert.Equal("not_found", problem.Title);
    }

    [Fact]
    public async Task OnResultExecution_ResultOfTSuccess_ReturnsOkObjectResultWithValue()
    {
        // Arrange
        var filter = new ResultToHttpFilter(CreateMapper());
        var value = "Hello, World!";
        var result = Result<string>.Success(value);
        var actionContext = new ActionContext(new DefaultHttpContext(), new(), new());
        var objectResult = new ObjectResult(result);
        var context = new ResultExecutingContext(actionContext, [filter], objectResult, new());
        var executedContext = new ResultExecutedContext(actionContext, [], context.Result, new());
        ResultExecutionDelegate next = () => Task.FromResult(executedContext);

        // Act
        await filter.OnResultExecutionAsync(context, next);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(context.Result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal("Hello, World!", okResult.Value);
    }

    [Fact]
    public async Task OnResultExecution_ResultOfTFailure_ReturnsProblemDetails()
    {
        // Arrange
        var filter = new ResultToHttpFilter(CreateMapper());
        var error = Errors.Validation("Invalid input.");
        var result = Result<int>.Failure(error);
        var actionContext = new ActionContext(new DefaultHttpContext(), new(), new());
        var objectResult = new ObjectResult(result);
        var context = new ResultExecutingContext(actionContext, [filter], objectResult, new());
        var executedContext = new ResultExecutedContext(actionContext, [], context.Result, new());
        ResultExecutionDelegate next = () => Task.FromResult(executedContext);

        // Act
        await filter.OnResultExecutionAsync(context, next);

        // Assert
        var objResult = Assert.IsType<ObjectResult>(context.Result);
        Assert.Equal(StatusCodes.Status400BadRequest, objResult.StatusCode);
        var problem = Assert.IsAssignableFrom<ProblemDetails>(objResult.Value);
        Assert.Equal("validation_error", problem.Title);
    }

    [Fact]
    public async Task OnResultExecution_NonResultObject_PassesThrough()
    {
        // Arrange
        var filter = new ResultToHttpFilter(CreateMapper());
        var value = new { Name = "Not a Result" };
        var actionContext = new ActionContext(new DefaultHttpContext(), new(), new());
        var objectResult = new ObjectResult(value);
        var context = new ResultExecutingContext(actionContext, [filter], objectResult, new());
        var executedContext = new ResultExecutedContext(actionContext, [], context.Result, new());
        ResultExecutionDelegate next = () => Task.FromResult(executedContext);

        // Act
        await filter.OnResultExecutionAsync(context, next);

        // Assert
        Assert.Same(objectResult, context.Result);
        Assert.Same(value, objectResult.Value);
    }
}
