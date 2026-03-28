using DotSharp.Primitives.Entities;
using FluentAssertions;
using Xunit;

namespace DotSharp.Primitives.Tests.Entities;

public sealed class CreationAuditableEntityTests
{
    #region Test doubles

    private sealed class OrderEntity : CreationAuditableEntity<Guid>
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

    #endregion
}
