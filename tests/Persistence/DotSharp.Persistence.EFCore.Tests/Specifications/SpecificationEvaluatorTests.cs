using DotSharp.Persistence.Abstractions.Specifications;
using DotSharp.Persistence.EFCore.AuditLog;
using DotSharp.Persistence.EFCore.Contexts;
using DotSharp.Persistence.EFCore.Inbox;
using DotSharp.Persistence.EFCore.Specifications;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DotSharp.Persistence.EFCore.Tests.Specifications;

public sealed class SpecificationEvaluatorTests : IDisposable
{
    #region Test doubles

    private sealed class Order
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
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

    private sealed class ActiveOrdersSpec : Specification<Order>
    {
        public ActiveOrdersSpec()
        {
            SetCriteria(x => !x.IsDeleted);
            AsNoTracking();
        }
    }

    private sealed class OrdersByCustomerSpec : Specification<Order>
    {
        public OrdersByCustomerSpec(string customerName)
        {
            SetCriteria(x => x.CustomerName == customerName);
            ApplyOrderBy(x => x.CreatedAt);
            AsNoTracking();
        }
    }

    private sealed class PagedOrdersSpec : Specification<Order>
    {
        public PagedOrdersSpec(int page, int size)
        {
            SetCriteria(x => !x.IsDeleted);
            ApplyPaging(new Abstractions.Pagination.Paging(page, size));
            ApplyOrderBy(x => x.CreatedAt);
            AsNoTracking();
        }
    }

    private sealed class OrderSummarySpec : Specification<Order, string>
    {
        public OrderSummarySpec()
        {
            SetCriteria(x => !x.IsDeleted);
            ApplySelector(x => x.CustomerName);
            AsNoTracking();
        }
    }

    private sealed class MultipleOrdersByCustomerSpec : Specification<Order>
    {
        public MultipleOrdersByCustomerSpec(string customerName)
        {
            SetCriteria(x => x.CustomerName == customerName);
            ApplyOrderBy(x => x.CustomerName);
            ThenOrderBy(x => x.CreatedAt, OrderDirection.Desc);
            AsNoTracking();
        }
    }

    private sealed class CombinedCriteriaSpec : Specification<Order>
    {
        public CombinedCriteriaSpec()
        {
            SetCriteria(x => !x.IsDeleted);
            AndCriteria(x => x.CustomerName == "John");
            AsNoTracking();
        }
    }

    private sealed class IncludeStringSpec : Specification<Order>
    {
        public IncludeStringSpec()
        {
            AddInclude("CustomerName"); // Just to test it doesn't crash in Evaluator
            AsNoTracking();
        }
    }

    #endregion

    private readonly TestDbContext _context;
    private readonly SpecificationEvaluator _evaluator;

    public SpecificationEvaluatorTests()
    {
        DbContextOptions options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new TestDbContext(options);
        _evaluator = new SpecificationEvaluator();

        SeedData();
    }

    private void SeedData()
    {
        _context.Orders.AddRange(
            new Order { Id = Guid.NewGuid(), CustomerName = "John", CreatedAt = DateTime.UtcNow.AddDays(-3), IsDeleted = false },
            new Order { Id = Guid.NewGuid(), CustomerName = "Jane", CreatedAt = DateTime.UtcNow.AddDays(-2), IsDeleted = false },
            new Order { Id = Guid.NewGuid(), CustomerName = "John", CreatedAt = DateTime.UtcNow.AddDays(-1), IsDeleted = false },
            new Order { Id = Guid.NewGuid(), CustomerName = "John", CreatedAt = DateTime.UtcNow.AddDays(0), IsDeleted = true }
        );
        _context.SaveChanges();
    }

    public void Dispose() => _context.Dispose();

    #region Criteria

    [Fact]
    public void Apply_WhenCriteriaSet_FiltersResults()
    {
        IQueryable<Order> query = _evaluator.Apply(_context.Orders.AsQueryable(), new ActiveOrdersSpec());

        query.Count().Should().Be(3);
    }

    [Fact]
    public void Apply_WhenCombinedCriteriaSet_FiltersResults()
    {
        IQueryable<Order> query = _evaluator.Apply(_context.Orders.AsQueryable(), new CombinedCriteriaSpec());

        query.Count().Should().Be(2); // Active + John
    }

    #endregion

    #region Ordering

    [Fact]
    public void Apply_WhenOrderingSet_OrdersResults()
    {
        IQueryable<Order> query = _evaluator.Apply(_context.Orders.AsQueryable(), new OrdersByCustomerSpec("John"));

        List<Order> results = [.. query];

        results.Should().BeInAscendingOrder(x => x.CreatedAt);
    }

    [Fact]
    public void Apply_WhenSecondaryOrderingSet_OrdersResults()
    {
        IQueryable<Order> query = _evaluator.Apply(_context.Orders.AsQueryable(), new MultipleOrdersByCustomerSpec("John"));

        List<Order> results = [.. query];

        results.Should().BeInDescendingOrder(x => x.CreatedAt);
    }

    #endregion

    #region Paging

    [Fact]
    public void Apply_WhenPagingSet_PaginatesResults()
    {
        IQueryable<Order> query = _evaluator.Apply(_context.Orders.AsQueryable(), new PagedOrdersSpec(1, 1));

        query.Count().Should().Be(1);
    }

    [Fact]
    public void Apply_WhenPagingNotApplied_ReturnsAllResults()
    {
        IQueryable<Order> query = _evaluator.Apply(_context.Orders.AsQueryable(), new ActiveOrdersSpec(), applyPaging: false);

        query.Count().Should().Be(3);
    }

    #endregion

    #region Includes

    [Fact]
    public void Apply_WithIncludeStrings_ReturnsQueryable()
    {
        var query = _evaluator.Apply(_context.Orders.AsQueryable(), new IncludeStringSpec());

        query.Should().NotBeNull();
    }

    #endregion

    #region Projection

    [Fact]
    public void Apply_WithSelector_ProjectsResults()
    {
        IQueryable<string> query = _evaluator.Apply(_context.Orders.AsQueryable(), new OrderSummarySpec());

        query.ToList().Should().AllBeOfType<string>();
    }

    #endregion

    #region Null specification

    [Fact]
    public void Apply_WhenSpecificationIsNull_ReturnsUnfilteredQuery()
    {
        IQueryable<Order> query = _evaluator.Apply(_context.Orders.AsQueryable(), null);

        query.Count().Should().Be(4);
    }

    #endregion
}
