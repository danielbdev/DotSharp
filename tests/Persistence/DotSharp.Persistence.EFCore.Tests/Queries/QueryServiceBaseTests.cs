using DotSharp.Persistence.Abstractions.Pagination;
using DotSharp.Persistence.EFCore.Contexts;
using DotSharp.Persistence.EFCore.Queries;
using DotSharp.Primitives.Pagination;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DotSharp.Persistence.EFCore.Tests.Queries;

public sealed class QueryServiceBaseTests : IDisposable
{
    #region Test doubles

    private sealed class Order
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; } = null!;
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

    private sealed class TestQueryService(TestDbContext context) : QueryServiceBase(context)
    {
        public Task<PaginationResult<Order>> GetPagedOrdersAsync(Paging paging)
            => PaginateAsync(Query<Order>(), paging);

        public Task<Order?> GetOrderByIdAsync(Guid id)
            => SingleAsync(Query<Order>().Where(x => x.Id == id));
    }

    #endregion

    private readonly TestDbContext _context;
    private readonly TestQueryService _queryService;

    public QueryServiceBaseTests()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new TestDbContext(options);
        _queryService = new TestQueryService(_context);

        SeedData();
    }

    private void SeedData()
    {
        _context.Orders.AddRange(
            new Order { Id = Guid.NewGuid(), CustomerName = "Order 1" },
            new Order { Id = Guid.NewGuid(), CustomerName = "Order 2" },
            new Order { Id = Guid.NewGuid(), CustomerName = "Order 3" }
        );
        _context.SaveChanges();
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task PaginateAsync_ReturnsCorrectPage()
    {
        var paging = new Paging(1, 2);

        var result = await _queryService.GetPagedOrdersAsync(paging);

        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(3);
        result.TotalPages.Should().Be(2);
    }

    [Fact]
    public async Task SingleAsync_ReturnsMatchingItem()
    {
        var existing = await _context.Orders.FirstAsync(TestContext.Current.CancellationToken);

        var result = await _queryService.GetOrderByIdAsync(existing.Id);

        result.Should().NotBeNull();
        result!.CustomerName.Should().Be(existing.CustomerName);
    }

    [Fact]
    public async Task Query_ReturnsNoTrackingQueryable()
    {
        _context.ChangeTracker.Clear();
        
        var results = await _queryService.Query<Order>().ToListAsync(TestContext.Current.CancellationToken);

        results.Should().NotBeEmpty();
        _context.ChangeTracker.Entries().Should().BeEmpty();
    }
}
