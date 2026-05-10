using DotSharp.Persistence.EFCore.Contexts;
using DotSharp.Persistence.EFCore.Interceptors;
using DotSharp.Primitives.AuditLog;
using DotSharp.Primitives.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DotSharp.Persistence.EFCore.Tests.Interceptors;

public sealed class AuditSaveChangesInterceptorTests : IDisposable
{
    #region Test doubles

    private sealed class AuditableEntity : FullAuditableEntity<Guid>
    {
        public string Name { get; set; } = string.Empty;

        public void SetId(Guid id) => Id = id;
    }

    private sealed class TestDbContext : DotSharpDbContext
    {
        private readonly AuditSaveChangesInterceptor _interceptor;

        public DbSet<AuditableEntity> AuditableEntities { get; set; }

        public TestDbContext(DbContextOptions options, AuditSaveChangesInterceptor interceptor) 
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
            modelBuilder.Entity<AuditableEntity>().HasKey(x => x.Id);
        }
    }

    private sealed class TestUserProvider(string userName) : IAuditUserProvider
    {
        public AuditUser GetCurrent() => new(userName, Guid.NewGuid().ToString());
    }

    #endregion

    private readonly TestDbContext _context;
    private const string CurrentUser = "admin";

    public AuditSaveChangesInterceptorTests()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var userProvider = new TestUserProvider(CurrentUser);
        var interceptor = new AuditSaveChangesInterceptor(userProvider);

        _context = new TestDbContext(options, interceptor);
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task SaveChanges_WhenEntityAdded_PopulatesCreationAuditFields()
    {
        var entity = new AuditableEntity { Name = "Test" };
        entity.SetId(Guid.NewGuid());

        _context.AuditableEntities.Add(entity);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        entity.CreatedBy.Should().Be(CurrentUser);
        entity.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task SaveChanges_WhenEntityModified_PopulatesModificationAuditFields()
    {
        var entity = new AuditableEntity { Name = "Test" };
        entity.SetId(Guid.NewGuid());
        _context.AuditableEntities.Add(entity);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        entity.Name = "Updated";
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        entity.LastModifiedBy.Should().Be(CurrentUser);
        entity.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task SaveChanges_WhenFullAuditableEntityDeleted_PerformsSoftDelete()
    {
        var entity = new AuditableEntity { Name = "Test" };
        entity.SetId(Guid.NewGuid());
        _context.AuditableEntities.Add(entity);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        _context.AuditableEntities.Remove(entity);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Verify it was NOT physically deleted but marked as deleted
        _context.Entry(entity).State.Should().Be(EntityState.Unchanged); // After SaveChanges it becomes Unchanged (or Detached if not tracked)
        entity.IsDeleted.Should().BeTrue();
        entity.DeletedBy.Should().Be(CurrentUser);
        entity.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        
        var exists = await _context.AuditableEntities.AnyAsync(x => x.Id == entity.Id, TestContext.Current.CancellationToken);
        exists.Should().BeTrue(); // Still in DB because it's soft deleted
    }
}
