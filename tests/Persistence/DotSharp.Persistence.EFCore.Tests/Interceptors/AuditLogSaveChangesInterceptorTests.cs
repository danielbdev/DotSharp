using System.Text.Json;
using DotSharp.Persistence.EFCore.AuditLog;
using DotSharp.Persistence.EFCore.Contexts;
using DotSharp.Persistence.EFCore.Interceptors;
using DotSharp.Primitives.AuditLog;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DotSharp.Persistence.EFCore.Tests.Interceptors;

public sealed class AuditLogSaveChangesInterceptorTests : IDisposable
{
    #region Test doubles

    [TrackChanges]
    private sealed class TrackedEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    private sealed class TestDbContext : DotSharpDbContext
    {
        private readonly AuditLogSaveChangesInterceptor _interceptor;

        public DbSet<TrackedEntity> TrackedEntities { get; set; }

        public TestDbContext(DbContextOptions options, AuditLogSaveChangesInterceptor interceptor) 
            : base(options)
        {
            _interceptor = interceptor;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(_interceptor);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TrackedEntity>().HasKey(x => x.Id);
        }
    }

    private sealed class TestUserProvider(string userName) : IAuditUserProvider
    {
        public AuditUser GetCurrent() => new(userName, Guid.NewGuid().ToString());
    }

    private sealed class TestAuditLog : IAuditLog
    {
        public List<IAuditLogEntry> Entries { get; } = [];
        public Task LogAsync(IAuditLogEntry entry, CancellationToken cancellationToken = default)
        {
            Entries.Add(entry);
            return Task.CompletedTask;
        }
    }

    #endregion

    private readonly TestDbContext _context;
    private readonly TestAuditLog _auditLog;
    private const string CurrentUser = "admin";

    public AuditLogSaveChangesInterceptorTests()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var userProvider = new TestUserProvider(CurrentUser);
        _auditLog = new TestAuditLog();
        var interceptor = new AuditLogSaveChangesInterceptor(userProvider, _auditLog);

        _context = new TestDbContext(options, interceptor);
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task SaveChanges_WhenTrackedEntityAdded_GeneratesAuditLog()
    {
        var entity = new TrackedEntity { Id = Guid.NewGuid(), Name = "Test" };

        _context.TrackedEntities.Add(entity);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        _auditLog.Entries.Should().HaveCount(1);
        var entry = _auditLog.Entries[0];
        entry.EntityName.Should().Be(nameof(TrackedEntity));
        entry.Action.Should().Be("Created");
        entry.ModifiedBy.Should().Be(CurrentUser);
        
        var newValues = JsonSerializer.Deserialize<Dictionary<string, object>>(entry.NewValues!);
        newValues!["Name"].ToString().Should().Be("Test");
    }

    [Fact]
    public async Task SaveChanges_WhenTrackedEntityModified_GeneratesAuditLogWithOldAndNewValues()
    {
        var id = Guid.NewGuid();
        var entity = new TrackedEntity { Id = id, Name = "Original" };
        _context.TrackedEntities.Add(entity);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        _auditLog.Entries.Clear();

        entity.Name = "Updated";
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        _auditLog.Entries.Should().HaveCount(1);
        var entry = _auditLog.Entries[0];
        entry.Action.Should().Be("Modified");
        
        var oldValues = JsonSerializer.Deserialize<Dictionary<string, object>>(entry.OldValues!);
        var newValues = JsonSerializer.Deserialize<Dictionary<string, object>>(entry.NewValues!);
        
        oldValues!["Name"].ToString().Should().Be("Original");
        newValues!["Name"].ToString().Should().Be("Updated");
    }

    [Fact]
    public async Task SaveChanges_WhenTrackedEntityDeleted_GeneratesAuditLog()
    {
        var entity = new TrackedEntity { Id = Guid.NewGuid(), Name = "ToDelete" };
        _context.TrackedEntities.Add(entity);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);
        _auditLog.Entries.Clear();

        _context.TrackedEntities.Remove(entity);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        _auditLog.Entries.Should().HaveCount(1);
        var entry = _auditLog.Entries[0];
        entry.Action.Should().Be("Deleted");
        entry.NewValues.Should().BeNull();
    }
}
