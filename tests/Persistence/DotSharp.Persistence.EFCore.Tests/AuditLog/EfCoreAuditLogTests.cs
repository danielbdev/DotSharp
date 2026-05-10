using DotSharp.Persistence.EFCore.AuditLog;
using DotSharp.Persistence.EFCore.Contexts;
using DotSharp.Primitives.AuditLog;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DotSharp.Persistence.EFCore.Tests.AuditLog;

public sealed class EfCoreAuditLogTests : IDisposable
{
    private sealed class TestDbContext(DbContextOptions options) : DotSharpDbContext(options);

    private readonly TestDbContext _context;
    private readonly EfCoreAuditLog _auditLog;

    public EfCoreAuditLogTests()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new TestDbContext(options);
        _auditLog = new EfCoreAuditLog(_context);
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task LogAsync_WhenCalled_PersistsEntryToDatabase()
    {
        var entry = new AuditLogEntry
        {
            EntityName = "TestEntity",
            EntityId = "123",
            Action = "Created",
            ModifiedBy = "admin",
            ModifiedAt = DateTime.UtcNow
        };

        await _auditLog.LogAsync(entry, TestContext.Current.CancellationToken);

        var persisted = await _context.AuditLogEntries.FirstOrDefaultAsync(TestContext.Current.CancellationToken);
        persisted.Should().NotBeNull();
        persisted!.EntityName.Should().Be("TestEntity");
        persisted.EntityId.Should().Be("123");
    }
}
