using DotSharp.Primitives.Entities;
using FluentAssertions;
using Xunit;

namespace DotSharp.Primitives.Tests.Entities;

public sealed class AuditableEntityTests
{
    #region Test doubles

    private sealed class OrderEntity : AuditableEntity<Guid>
    {
        public OrderEntity() { }
    }

    #endregion

    #region AuditCreation

    [Fact]
    public void AuditCreation_SetsCreatedBy()
    {
        OrderEntity entity = new OrderEntity();

        entity.AuditCreation("john.doe");

        entity.CreatedBy.Should().Be("john.doe");
    }

    [Fact]
    public void AuditCreation_SetsCreatedAtToUtcNow()
    {
        OrderEntity entity = new OrderEntity();
        DateTime before = DateTime.UtcNow;

        entity.AuditCreation("john.doe");

        entity.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(DateTime.UtcNow);
    }

    [Fact]
    public void AuditCreation_CreatedAtIsUtc()
    {
        OrderEntity entity = new OrderEntity();

        entity.AuditCreation("john.doe");

        entity.CreatedAt.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void AuditCreation_LastModifiedRemainsNull()
    {
        OrderEntity entity = new OrderEntity();

        entity.AuditCreation("john.doe");

        entity.LastModifiedAt.Should().BeNull();
        entity.LastModifiedBy.Should().BeNull();
    }

    #endregion

    #region AuditUpdate

    [Fact]
    public void AuditUpdate_SetsLastModifiedBy()
    {
        OrderEntity entity = new OrderEntity();
        entity.AuditCreation("john.doe");

        entity.AuditUpdate("jane.doe");

        entity.LastModifiedBy.Should().Be("jane.doe");
    }

    [Fact]
    public void AuditUpdate_SetsLastModifiedAtToUtcNow()
    {
        OrderEntity entity = new OrderEntity();
        entity.AuditCreation("john.doe");
        DateTime before = DateTime.UtcNow;

        entity.AuditUpdate("jane.doe");

        entity.LastModifiedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(DateTime.UtcNow);
    }

    [Fact]
    public void AuditUpdate_LastModifiedAtIsUtc()
    {
        OrderEntity entity = new OrderEntity();
        entity.AuditCreation("john.doe");

        entity.AuditUpdate("jane.doe");

        entity.LastModifiedAt!.Value.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void AuditUpdate_DoesNotChangeCreatedBy()
    {
        OrderEntity entity = new OrderEntity();
        entity.AuditCreation("john.doe");

        entity.AuditUpdate("jane.doe");

        entity.CreatedBy.Should().Be("john.doe");
    }

    #endregion
}
