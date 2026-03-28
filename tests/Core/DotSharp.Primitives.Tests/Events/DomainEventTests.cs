using DotSharp.Primitives.Events;
using FluentAssertions;
using Xunit;

namespace DotSharp.Primitives.Tests.Events;

public sealed class DomainEventTests
{
    #region Test doubles

    private sealed class OrderCreatedEvent(string? correlationId = null) : DomainEvent(correlationId) { }

    #endregion

    #region EventId

    [Fact]
    public void EventId_IsUniquePerInstance()
    {
        OrderCreatedEvent first = new OrderCreatedEvent();
        OrderCreatedEvent second = new OrderCreatedEvent();

        first.EventId.Should().NotBe(second.EventId);
    }

    [Fact]
    public void EventId_IsNotEmpty()
    {
        OrderCreatedEvent domainEvent = new OrderCreatedEvent();

        domainEvent.EventId.Should().NotBeEmpty();
    }

    #endregion

    #region OccurredOnUtc

    [Fact]
    public void OccurredOnUtc_IsSetToUtcNow()
    {
        DateTimeOffset before = DateTimeOffset.UtcNow;

        OrderCreatedEvent domainEvent = new OrderCreatedEvent();

        domainEvent.OccurredOnUtc.Should().BeOnOrAfter(before).And.BeOnOrBefore(DateTimeOffset.UtcNow);
    }

    [Fact]
    public void OccurredOnUtc_IsUtc()
    {
        OrderCreatedEvent domainEvent = new OrderCreatedEvent();

        domainEvent.OccurredOnUtc.Offset.Should().Be(TimeSpan.Zero);
    }

    #endregion

    #region CorrelationId

    [Fact]
    public void CorrelationId_WhenProvided_IsSet()
    {
        OrderCreatedEvent domainEvent = new OrderCreatedEvent("correlation-123");

        domainEvent.CorrelationId.Should().Be("correlation-123");
    }

    [Fact]
    public void CorrelationId_WhenNotProvided_IsNull()
    {
        OrderCreatedEvent domainEvent = new OrderCreatedEvent();

        domainEvent.CorrelationId.Should().BeNull();
    }

    #endregion

    #region Contracts

    [Fact]
    public void DomainEvent_ImplementsIDomainEvent()
    {
        OrderCreatedEvent domainEvent = new OrderCreatedEvent();

        domainEvent.Should().BeAssignableTo<IDomainEvent>();
    }

    #endregion
}
