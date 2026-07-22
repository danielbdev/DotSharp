using DotSharp.Primitives.Pagination;
using DotSharp.Web.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Xunit;

namespace DotSharp.Web.Tests.Filters;

/// <summary>
/// Tests for <see cref="PaginationHeaderFilter"/> — automatic pagination header detection.
/// </summary>
public sealed class PaginationHeaderFilterTests
{
    private static HttpContext CreateHttpContext()
    {
        var httpContext = new DefaultHttpContext();
        return httpContext;
    }

    [Fact]
    public async Task OnResultExecution_PaginationResult_SetsAllHeaders()
    {
        // Arrange
        var filter = new PaginationHeaderFilter();
        var items = new List<string> { "Item 1", "Item 2" };
        var pagination = PaginationResult<string>.Create(items, totalCount: 100, pageNumber: 3, pageSize: 20);
        var httpContext = CreateHttpContext();
        var actionContext = new ActionContext(httpContext, new(), new());
        var objectResult = new ObjectResult(pagination);
        var context = new ResultExecutingContext(actionContext, [filter], objectResult, new());
        var executedContext = new ResultExecutedContext(actionContext, [], context.Result, new());
        ResultExecutionDelegate next = () => Task.FromResult(executedContext);

        // Act
        await filter.OnResultExecutionAsync(context, next);

        // Assert
        Assert.Equal("3", httpContext.Response.Headers["X-Pagination-PageNumber"]);
        Assert.Equal("20", httpContext.Response.Headers["X-Pagination-PageSize"]);
        Assert.Equal("100", httpContext.Response.Headers["X-Pagination-TotalCount"]);
        Assert.Equal("5", httpContext.Response.Headers["X-Pagination-TotalPages"]);

        // Body should be just the items list
        var bodyItems = Assert.IsAssignableFrom<IReadOnlyList<string>>(objectResult.Value);
        Assert.Equal(2, bodyItems.Count);
        Assert.Equal("Item 1", bodyItems[0]);
        Assert.Equal("Item 2", bodyItems[1]);
    }

    [Fact]
    public async Task OnResultExecution_PaginationResult_ReplacesValueWithItems()
    {
        // Arrange
        var filter = new PaginationHeaderFilter();
        var items = new List<string> { "A" };
        var pagination = PaginationResult<string>.Create(items, totalCount: 1, pageNumber: 1, pageSize: 10);
        var httpContext = CreateHttpContext();
        var actionContext = new ActionContext(httpContext, new(), new());
        var objectResult = new ObjectResult(pagination);
        var context = new ResultExecutingContext(actionContext, [filter], objectResult, new());
        var executedContext = new ResultExecutedContext(actionContext, [], context.Result, new());
        ResultExecutionDelegate next = () => Task.FromResult(executedContext);

        // Act
        await filter.OnResultExecutionAsync(context, next);

        // Assert
        Assert.IsAssignableFrom<IReadOnlyList<string>>(objectResult.Value);
        Assert.NotSame(pagination, objectResult.Value);
    }

    [Fact]
    public async Task OnResultExecution_EmptyPagination_SetsCorrectHeaders()
    {
        // Arrange
        var filter = new PaginationHeaderFilter();
        var pagination = PaginationResult<object>.Create([], totalCount: 0, pageNumber: 1, pageSize: 10);
        var httpContext = CreateHttpContext();
        var actionContext = new ActionContext(httpContext, new(), new());
        var objectResult = new ObjectResult(pagination);
        var context = new ResultExecutingContext(actionContext, [filter], objectResult, new());
        var executedContext = new ResultExecutedContext(actionContext, [], context.Result, new());
        ResultExecutionDelegate next = () => Task.FromResult(executedContext);

        // Act
        await filter.OnResultExecutionAsync(context, next);

        // Assert
        Assert.Equal("0", httpContext.Response.Headers["X-Pagination-TotalCount"]);
        Assert.Equal("0", httpContext.Response.Headers["X-Pagination-TotalPages"]);
        Assert.Equal("1", httpContext.Response.Headers["X-Pagination-PageNumber"]);
        Assert.Equal("10", httpContext.Response.Headers["X-Pagination-PageSize"]);
    }

    [Fact]
    public async Task OnResultExecution_NonPaginationObject_PassesThrough()
    {
        // Arrange
        var filter = new PaginationHeaderFilter();
        var value = new { Name = "Not Pagination" };
        var httpContext = CreateHttpContext();
        var actionContext = new ActionContext(httpContext, new(), new());
        var objectResult = new ObjectResult(value);
        var context = new ResultExecutingContext(actionContext, [filter], objectResult, new());
        var executedContext = new ResultExecutedContext(actionContext, [], context.Result, new());
        ResultExecutionDelegate next = () => Task.FromResult(executedContext);

        // Act
        await filter.OnResultExecutionAsync(context, next);

        // Assert
        Assert.Same(objectResult, context.Result);
        Assert.Same(value, objectResult.Value);
        Assert.False(httpContext.Response.Headers.ContainsKey("X-Pagination-PageNumber"));
    }
}
