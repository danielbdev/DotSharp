using DotSharp.Primitives.AuditLog;
using FluentAssertions;
using Xunit;

namespace DotSharp.Primitives.Tests.Auditing;

public sealed class AuditUserTests
{
    #region Properties

    [Fact]
    public void AuditUser_SetsUserName()
    {
        AuditUser auditUser = new AuditUser("john.doe");

        auditUser.UserName.Should().Be("john.doe");
    }

    [Fact]
    public void AuditUser_WhenUserIdProvided_SetsUserId()
    {
        AuditUser auditUser = new AuditUser("john.doe", "123");

        auditUser.UserId.Should().Be("123");
    }

    [Fact]
    public void AuditUser_WhenUserIdNotProvided_IsNull()
    {
        AuditUser auditUser = new AuditUser("john.doe");

        auditUser.UserId.Should().BeNull();
    }

    #endregion

    #region Equality

    [Fact]
    public void AuditUser_WhenSameValues_AreEqual()
    {
        AuditUser left = new AuditUser("john.doe", "123");
        AuditUser right = new AuditUser("john.doe", "123");

        left.Should().Be(right);
    }

    [Fact]
    public void AuditUser_WhenDifferentUserName_AreNotEqual()
    {
        AuditUser left = new AuditUser("john.doe");
        AuditUser right = new AuditUser("jane.doe");

        left.Should().NotBe(right);
    }

    [Fact]
    public void AuditUser_WhenDifferentUserId_AreNotEqual()
    {
        AuditUser left = new AuditUser("john.doe", "123");
        AuditUser right = new AuditUser("john.doe", "456");

        left.Should().NotBe(right);
    }

    #endregion
}
