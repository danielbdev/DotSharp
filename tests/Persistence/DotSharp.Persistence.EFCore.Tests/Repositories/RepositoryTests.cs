using DotSharp.Persistence.EFCore.Contexts;
using DotSharp.Persistence.EFCore.Repositories;
using DotSharp.Persistence.EFCore.Specifications;
using DotSharp.Primitives.Aggregates;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DotSharp.Persistence.EFCore.Tests.Repositories;

public sealed class RepositoryTests : IDisposable
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Order>().HasKey(x => x.Id);
        }
    }

    #endregion

    private readonly TestDbContext _context;
    private readonly Repository<Order, Guid> _repository = null!;

    public RepositoryTests()
    {
        DbContextOptions options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new TestDbContext(options);
        _repository = new Repository<Order, Guid>(_context, new SpecificationEvaluator());
    }

    public void Dispose() => _context.Dispose();

    #region Add

    [Fact]
    public async Task Add_WhenCalled_AddsEntityToContext()
    {
        var order = new Order(Guid.NewGuid(), "John");

        _repository.Add(order);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        Order? result = await _repository.GetByIdAsync(order.Id, TestContext.Current.CancellationToken);
        result.Should().NotBeNull();
    }

    #endregion

    #region AddRange

    [Fact]
    public async Task AddRange_WhenCalled_AddsAllEntitiesToContext()
    {
        var orders = new[]
        {
            new Order(Guid.NewGuid(), "John"),
            new Order(Guid.NewGuid(), "Jane")
        };

        _repository.AddRange(orders);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        bool john = await _repository.ExistsAsync(orders[0].Id, TestContext.Current.CancellationToken);
        bool jane = await _repository.ExistsAsync(orders[1].Id, TestContext.Current.CancellationToken);

        john.Should().BeTrue();
        jane.Should().BeTrue();
    }

    #endregion

    #region Update

    [Fact]
    public async Task Update_WhenCalled_UpdatesEntityInContext()
    {
        var id = Guid.NewGuid();
        var order = new Order(id, "John");
        _repository.Add(order);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        _context.ChangeTracker.Clear();

        var updated = new Order(id, "Jane");
        _repository.Update(updated);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        Order? result = await _repository.GetByIdAsync(id, TestContext.Current.CancellationToken);
        result!.CustomerName.Should().Be("Jane");
    }

    #endregion

    #region Remove

    [Fact]
    public async Task Remove_WhenCalled_RemovesEntityFromContext()
    {
        var order = new Order(Guid.NewGuid(), "John");
        _repository.Add(order);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        _context.ChangeTracker.Clear();

        Order? toRemove = await _repository.GetByIdAsync(order.Id, TestContext.Current.CancellationToken);
        _repository.Remove(toRemove!);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        Order? result = await _repository.GetByIdAsync(order.Id, TestContext.Current.CancellationToken);
        result.Should().BeNull();
    }

    #endregion

    #region RemoveRange

    [Fact]
    public async Task RemoveRange_WhenCalled_RemovesAllEntitiesFromContext()
    {
        var orders = new[]
        {
            new Order(Guid.NewGuid(), "John"),
            new Order(Guid.NewGuid(), "Jane")
        };

        _repository.AddRange(orders);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        _context.ChangeTracker.Clear();

        var toRemove = new List<Order>();
        foreach (var o in orders)
        {
            Order? found = await _repository.GetByIdAsync(o.Id, TestContext.Current.CancellationToken);
            toRemove.Add(found!);
        }

        _repository.RemoveRange(toRemove);
        await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        bool john = await _repository.ExistsAsync(orders[0].Id, TestContext.Current.CancellationToken);
        bool jane = await _repository.ExistsAsync(orders[1].Id, TestContext.Current.CancellationToken);

        john.Should().BeFalse();
        jane.Should().BeFalse();
    }

    #endregion
}
