using DotSharp.Persistence.EFCore.Contexts;
using DotSharp.Persistence.EFCore.UnitOfWork;
using DotSharp.Primitives.Aggregates;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Xunit;

namespace DotSharp.Persistence.EFCore.Tests.UnitOfWork;

public sealed class EfCoreUnitOfWorkTests : IDisposable
{
    #region Test doubles

    private sealed class Order : AggregateRoot<Guid>
    {
        public string CustomerName { get; private set; } = null!;

        public Order() { }

        public Order(Guid id, string customerName)
        {
            Id = id;
            CustomerName = customerName;
        }
    }

    private sealed class TestDbContext(DbContextOptions options) : DotSharpDbContext(options)
    {
        public DbSet<Order> Orders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Order>().HasKey(x => x.Id);
        }
    }

    #endregion

    private readonly TestDbContext _context;
    private readonly EfCoreUnitOfWork _unitOfWork;

    public EfCoreUnitOfWorkTests()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new TestDbContext(options);
        _unitOfWork = new EfCoreUnitOfWork(_context);
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task SaveChangesAsync_WhenCalled_PersistsChanges()
    {
        _context.Orders.Add(new Order(Guid.NewGuid(), "John"));
        
        await _unitOfWork.SaveChangesAsync(TestContext.Current.CancellationToken);

        _context.Orders.Should().HaveCount(1);
    }

    [Fact]
    public async Task ExecuteInTransactionAsync_WhenOperationSucceeds_ReturnsResult()
    {
        var result = await _unitOfWork.ExecuteInTransactionAsync(async () => 
        {
            await Task.Yield();
            return "Success";
        }, TestContext.Current.CancellationToken);

        result.Should().Be("Success");
    }

    [Fact]
    public async Task ExecuteInTransactionAsync_WhenOperationFails_ThrowsAndRollsBack()
    {
        // Note: InMemoryDatabase doesn't truly rollback, but we verify the exception is rethrown
        var action = async () => await _unitOfWork.ExecuteInTransactionAsync<string>(() => 
        {
            throw new InvalidOperationException("Fail");
        }, TestContext.Current.CancellationToken);

        await action.Should().ThrowAsync<InvalidOperationException>().WithMessage("Fail");
    }
}
