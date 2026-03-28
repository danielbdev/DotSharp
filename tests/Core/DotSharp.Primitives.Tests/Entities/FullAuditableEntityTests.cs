using DotSharp.Primitives.Entities;
using FluentAssertions;
using Xunit;

namespace DotSharp.Primitives.Tests.Entities;

public sealed class FullAuditableEntityTests
{
    #region Test doubles

    private sealed class OrderEntity : FullAuditableEntity<Guid>
    {
        public OrderEntity() { }
    }

    #endregion

    #region Initial state

    [Fact]
    public void IsDeleted_BeforeDeletion_IsFalse()
    {
        OrderEntity entity = new OrderEntity();

        entity.IsDeleted.Should().BeFalse();
        entity.DeletedAt.Should().BeNull();
        entity.DeletedBy.Should().BeNull();
    }

    #endregion

    #region AuditDeletion

    [Fact]
    public void AuditDeletion_SetsIsDeletedToTrue()
    {
        OrderEntity entity = new OrderEntity();

        entity.AuditDeletion("john.doe");

        entity.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public void AuditDeletion_SetsDeletedBy()
    {
        OrderEntity entity = new OrderEntity();

        entity.AuditDeletion("john.doe");

        entity.DeletedBy.Should().Be("john.doe");
    }

    [Fact]
    public void AuditDeletion_SetsDeletedAtToUtcNow()
    {
        OrderEntity entity = new OrderEntity();
        DateTime before = DateTime.UtcNow;

        entity.AuditDeletion("john.doe");

        entity.DeletedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(DateTime.UtcNow);
    }

    [Fact]
    public void AuditDeletion_DeletedAtIsUtc()
    {
        OrderEntity entity = new OrderEntity();

        entity.AuditDeletion("john.doe");

        entity.DeletedAt!.Value.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void AuditDeletion_DoesNotChangeCreatedBy()
    {
        OrderEntity entity = new OrderEntity();
        entity.AuditCreation("john.doe");

        entity.AuditDeletion("admin");

        entity.CreatedBy.Should().Be("john.doe");
    }

    #endregion
}
