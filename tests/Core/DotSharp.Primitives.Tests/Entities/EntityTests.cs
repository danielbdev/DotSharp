using DotSharp.Primitives.Entities;
using DotSharp.Primitives.Events;
using FluentAssertions;
using Xunit;

namespace DotSharp.Primitives.Tests.Entities;

public sealed class EntityTests
{
    #region Test doubles

    private sealed class OrderEntity : Entity<Guid>
    {
        public OrderEntity(Guid id) : base(id) { }
        public OrderEntity() { }
    }

    private sealed class ProductEntity(Guid id) : Entity<Guid>(id) { }

    private sealed class TestDomainEvent : DomainEvent { }

    #endregion

    #region IsTransient

    [Fact]
    public void IsTransient_WhenIdIsDefault_ReturnsTrue()
    {
        OrderEntity entity = new OrderEntity ();

        entity.IsTransient().Should().BeTrue();
    }

    [Fact]
    public void IsTransient_WhenIdIsSet_ReturnsFalse()
    {
        OrderEntity entity = new OrderEntity (Guid.NewGuid());

        entity.IsTransient().Should().BeFalse();
    }

    #endregion

    #region Equals

    [Fact]
    public void Equals_WhenSameReference_ReturnsTrue()
    {
        OrderEntity entity = new OrderEntity (Guid.NewGuid());

        entity.Equals(entity).Should().BeTrue();
    }

    [Fact]
    public void Equals_WhenSameIdAndSameType_ReturnsTrue()
    {
        Guid id = Guid.NewGuid();
        OrderEntity left = new OrderEntity (id);
        OrderEntity right = new OrderEntity (id);

        left.Equals(right).Should().BeTrue();
    }

    [Fact]
    public void Equals_WhenDifferentId_ReturnsFalse()
    {
        OrderEntity left = new OrderEntity (Guid.NewGuid());
        OrderEntity right = new OrderEntity (Guid.NewGuid());

        left.Equals(right).Should().BeFalse();
    }

    [Fact]
    public void Equals_WhenSameIdButDifferentType_ReturnsFalse()
    {
        Guid id = Guid.NewGuid();
        OrderEntity order = new OrderEntity (id);
        ProductEntity product = new ProductEntity (id);

        order.Equals(product).Should().BeFalse();
    }

    [Fact]
    public void Equals_WhenNull_ReturnsFalse()
    {
        OrderEntity entity = new OrderEntity (Guid.NewGuid());

        entity.Equals(null).Should().BeFalse();
    }

    [Fact]
    public void Equals_WhenBothTransient_ReturnsFalse()
    {
        OrderEntity left = new OrderEntity ();
        OrderEntity right = new OrderEntity ();

        left.Equals(right).Should().BeFalse();
    }

    [Fact]
    public void Equals_WhenOneIsTransient_ReturnsFalse()
    {
        OrderEntity left = new OrderEntity (Guid.NewGuid());
        OrderEntity right = new OrderEntity ();

        left.Equals(right).Should().BeFalse();
    }

    #endregion

    #region Operators

    [Fact]
    public void EqualityOperator_WhenSameId_ReturnsTrue()
    {
        Guid id = Guid.NewGuid();
        OrderEntity left = new OrderEntity (id);
        OrderEntity right = new OrderEntity (id);

        (left == right).Should().BeTrue();
    }

    [Fact]
    public void InequalityOperator_WhenDifferentId_ReturnsTrue()
    {
        OrderEntity left = new OrderEntity (Guid.NewGuid());
        OrderEntity right = new OrderEntity (Guid.NewGuid());

        (left != right).Should().BeTrue();
    }

    [Fact]
    public void EqualityOperator_WhenBothNull_ReturnsTrue()
    {
        OrderEntity? left = null;
        OrderEntity? right = null;

        (left == right).Should().BeTrue();
    }

    [Fact]
    public void EqualityOperator_WhenOneIsNull_ReturnsFalse()
    {
        OrderEntity left = new OrderEntity (Guid.NewGuid());
        OrderEntity? right = null;

        (left == right).Should().BeFalse();
    }

    #endregion

    #region GetHashCode

    [Fact]
    public void GetHashCode_WhenSameId_ReturnsSameHash()
    {
        Guid id = Guid.NewGuid();
        OrderEntity left = new OrderEntity(id);
        OrderEntity right = new OrderEntity(id);

        left.GetHashCode().Should().Be(right.GetHashCode());
    }

    [Fact]
    public void GetHashCode_WhenTransient_ReturnsDifferentHashForEachInstance()
    {
        OrderEntity left = new OrderEntity();
        OrderEntity right = new OrderEntity();

        left.GetHashCode().Should().NotBe(right.GetHashCode());
    }

    #endregion

    #region Domain events

    [Fact]
    public void AddDomainEvent_WhenValidEvent_AddsToDomainEvents()
    {
        OrderEntity entity = new OrderEntity(Guid.NewGuid());
        TestDomainEvent domainEvent = new TestDomainEvent();

        entity.AddDomainEvent(domainEvent);

        entity.DomainEvents.Should().ContainSingle()
            .Which.Should().Be(domainEvent);
    }

    [Fact]
    public void AddDomainEvent_WhenNull_ThrowsArgumentNullException()
    {
        OrderEntity entity = new OrderEntity(Guid.NewGuid());

        Action act = () => entity.AddDomainEvent(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void RemoveDomainEvent_WhenEventExists_RemovesFromDomainEvents()
    {
        OrderEntity entity = new OrderEntity(Guid.NewGuid());
        TestDomainEvent domainEvent = new TestDomainEvent();
        entity.AddDomainEvent(domainEvent);

        entity.RemoveDomainEvent(domainEvent);

        entity.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void RemoveDomainEvent_WhenNull_ThrowsArgumentNullException()
    {
        OrderEntity entity = new OrderEntity(Guid.NewGuid());

        Action act = () => entity.RemoveDomainEvent(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ClearDomainEvents_WhenEventsExist_EmptiesDomainEvents()
    {
        OrderEntity entity = new OrderEntity(Guid.NewGuid());
        entity.AddDomainEvent(new TestDomainEvent());
        entity.AddDomainEvent(new TestDomainEvent());

        entity.ClearDomainEvents();

        entity.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void DomainEvents_IsReadOnly()
    {
        OrderEntity entity = new OrderEntity(Guid.NewGuid());

        entity.DomainEvents.Should().BeAssignableTo<IReadOnlyCollection<DomainEvent>>();
    }

    #endregion

    #region Contracts

    [Fact]
    public void Entity_ImplementsIEntity()
    {
        var entity = new OrderEntity(Guid.NewGuid());

        entity.Should().BeAssignableTo<IEntity>();
    }

    [Fact]
    public void Entity_ImplementsIEntityOfTKey()
    {
        var entity = new OrderEntity(Guid.NewGuid());

        entity.Should().BeAssignableTo<IEntity<Guid>>();
    }

    [Fact]
    public void Entity_ImplementsIHasDomainEvents()
    {
        var entity = new OrderEntity(Guid.NewGuid());

        entity.Should().BeAssignableTo<IHasDomainEvents>();
    }

    #endregion
}
