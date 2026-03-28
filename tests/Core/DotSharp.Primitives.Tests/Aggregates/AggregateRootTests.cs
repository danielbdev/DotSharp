using DotSharp.Primitives.Aggregates;
using FluentAssertions;
using Xunit;

namespace DotSharp.Primitives.Tests.Aggregates;

public sealed class AggregateRootTests
{
    #region Test doubles

    private sealed class OrderAggregate : AggregateRoot<Guid>
    {
        public OrderAggregate(Guid id) : base(id) { }
        public OrderAggregate() { }
    }

    private sealed class OrderProductAggregate : AggregateRoot
    {
    }

    #endregion

    #region Contracts

    [Fact]
    public void AggregateRoot_ImplementsIAggregateRoot()
    {
        OrderAggregate aggregate = new OrderAggregate(Guid.NewGuid());

        aggregate.Should().BeAssignableTo<IAggregateRoot>();
    }

    [Fact]
    public void AggregateRoot_ImplementsIAggregateRootOfTKey()
    {
        OrderAggregate aggregate = new OrderAggregate(Guid.NewGuid());

        aggregate.Should().BeAssignableTo<IAggregateRoot<Guid>>();
    }

    [Fact]
    public void AggregateRoot_WithoutKey_ImplementsIAggregateRoot()
    {
        OrderProductAggregate aggregate = new OrderProductAggregate();

        aggregate.Should().BeAssignableTo<IAggregateRoot>();
    }

    #endregion

    #region Identity

    [Fact]
    public void AggregateRoot_WhenIdSet_IsNotTransient()
    {
        OrderAggregate aggregate = new OrderAggregate(Guid.NewGuid());

        aggregate.IsTransient().Should().BeFalse();
    }

    [Fact]
    public void AggregateRoot_WhenNoId_IsTransient()
    {
        OrderAggregate aggregate = new OrderAggregate();

        aggregate.IsTransient().Should().BeTrue();
    }

    [Fact]
    public void AggregateRoot_ExposesIdFromInterface()
    {
        Guid id = Guid.NewGuid();
        IAggregateRoot<Guid> aggregate = new OrderAggregate(id);

        aggregate.Id.Should().Be(id);
    }

    #endregion
}
