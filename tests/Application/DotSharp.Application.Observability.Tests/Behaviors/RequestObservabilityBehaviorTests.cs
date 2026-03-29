using DotSharp.Application.Abstractions.Messaging;
using DotSharp.Application.Observability.Behaviors;
using DotSharp.Observability.Correlation;
using DotSharp.Observability.Tracing;
using DotSharp.Results;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace DotSharp.Application.Observability.Tests.Behaviors;

public sealed class RequestObservabilityBehaviorTests
{
    #region Test doubles

    private sealed record CreateOrderCommand(string CustomerName) : IRequest<Result<Guid>>;

    private sealed class StubCorrelationContext : ICorrelationContext
    {
        public string CorrelationId => "test-correlation-id";
    }

    private sealed class StubTraceContext : ITraceContext
    {
        public string TraceId => "test-trace-id";
        public string SpanId => "test-span-id";
    }

    #endregion

    #region Handle

    [Fact]
    public async Task Handle_CallsNext()
    {
        RequestObservabilityBehavior<CreateOrderCommand, Result<Guid>> behavior = new RequestObservabilityBehavior<CreateOrderCommand, Result<Guid>>(
            NullLogger<RequestObservabilityBehavior<CreateOrderCommand, Result<Guid>>>.Instance,
            new StubCorrelationContext(),
            new StubTraceContext());

        bool nextCalled = false;
        CreateOrderCommand command = new CreateOrderCommand("John");

        await behavior.Handle(command, () =>
        {
            nextCalled = true;
            return Task.FromResult(Result<Guid>.Success(Guid.NewGuid()));
        }, CancellationToken.None);

        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ReturnsNextResult()
    {
        RequestObservabilityBehavior<CreateOrderCommand, Result<Guid>> behavior = new RequestObservabilityBehavior<CreateOrderCommand, Result<Guid>>(
            NullLogger<RequestObservabilityBehavior<CreateOrderCommand, Result<Guid>>>.Instance,
            new StubCorrelationContext(),
            new StubTraceContext());

        Guid expectedId = Guid.NewGuid();
        CreateOrderCommand command = new CreateOrderCommand("John");

        Result<Guid> result = await behavior.Handle(command,
            () => Task.FromResult(Result<Guid>.Success(expectedId)),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedId);
    }

    #endregion
}
