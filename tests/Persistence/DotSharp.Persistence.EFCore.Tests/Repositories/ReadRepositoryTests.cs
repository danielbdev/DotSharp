using DotSharp.Persistence.Abstractions.Specifications;
using DotSharp.Persistence.EFCore.Contexts;
using DotSharp.Persistence.EFCore.Repositories;
using DotSharp.Persistence.EFCore.Specifications;
using DotSharp.Primitives.Aggregates;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DotSharp.Persistence.EFCore.Tests.Repositories;

public sealed class ReadRepositoryTests : IDisposable
{
    #region Test doubles

    private sealed class Order : AggregateRoot<Guid>
    {
        public string CustomerName { get; private set; } = null!;
        public bool IsDeleted { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public Order() { }

        public Order(Guid id, string customerName)
        {
            Id = id;
            CustomerName = customerName;
            CreatedAt = DateTime.UtcNow;
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

    private sealed class ActiveOrdersSpec : Specification<Order>
    {
        public ActiveOrdersSpec()
        {
            SetCriteria(x => !x.IsDeleted);
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

    private sealed class PagedOrderSummarySpec : Specification<Order, string>
    {
        public PagedOrderSummarySpec(int page, int size)
        {
            SetCriteria(x => !x.IsDeleted);
            ApplySelector(x => x.CustomerName);
            ApplyPaging(new Abstractions.Pagination.Paging(page, size));
            ApplyOrderBy(x => x.CreatedAt);
            AsNoTracking();
        }
    }

    #endregion

    private readonly TestDbContext _context;
    private readonly ReadRepository<Order, Guid> _repository = null!;
    private readonly Guid _existingId = Guid.NewGuid();

    public ReadRepositoryTests()
    {
        DbContextOptions options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new TestDbContext(options);
        _repository = new ReadRepository<Order, Guid>(_context, new SpecificationEvaluator());

        SeedData();
    }

    private void SeedData()
    {
        _context.Orders.AddRange(
            new Order(_existingId, "John"),
            new Order(Guid.NewGuid(), "Jane"),
            new Order(Guid.NewGuid(), "Bob")
        );
        _context.SaveChanges();
    }

    public void Dispose() => _context.Dispose();

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsEntity()
    {
        Order? result = await _repository.GetByIdAsync(_existingId, TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        result!.Id.Should().Be(_existingId);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotExists_ReturnsNull()
    {
        Order? result = await _repository.GetByIdAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);

        result.Should().BeNull();
    }

    #endregion

    #region ExistsAsync

    [Fact]
    public async Task ExistsAsync_WhenExists_ReturnsTrue()
    {
        bool result = await _repository.ExistsAsync(_existingId, TestContext.Current.CancellationToken);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WhenNotExists_ReturnsFalse()
    {
        bool result = await _repository.ExistsAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);

        result.Should().BeFalse();
    }

    #endregion

    #region AnyAsync

    [Fact]
    public async Task AnyAsync_WhenMatchingEntitiesExist_ReturnsTrue()
    {
        bool result = await _repository.AnyAsync(new ActiveOrdersSpec(), TestContext.Current.CancellationToken);

        result.Should().BeTrue();
    }

    #endregion

    #region CountAsync

    [Fact]
    public async Task CountAsync_ReturnsCorrectCount()
    {
        int result = await _repository.CountAsync(new ActiveOrdersSpec(), TestContext.Current.CancellationToken);

        result.Should().Be(3);
    }

    #endregion

    #region GetSingleAsync

    [Fact]
    public async Task GetSingleAsync_WhenMatchingEntityExists_ReturnsEntity()
    {
        Order? result = await _repository.GetSingleAsync(new ActiveOrdersSpec(), TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetSingleAsync_WithProjection_ReturnsProjectedResult()
    {
        string? result = await _repository.GetSingleAsync(new OrderSummarySpec(), TestContext.Current.CancellationToken);

        result.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region ListAsync

    [Fact]
    public async Task ListAsync_WithSpecification_ReturnsMatchingEntities()
    {
        IReadOnlyList<Order> result = await _repository.ListAsync(new ActiveOrdersSpec(), TestContext.Current.CancellationToken);

        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task ListAsync_WithProjection_ReturnsProjectedResults()
    {
        IReadOnlyList<string> result = await _repository.ListAsync(new OrderSummarySpec(), TestContext.Current.CancellationToken);

        result.Should().HaveCount(3);
    }

    #endregion

    #region PageAsync

    [Fact]
    public async Task PageAsync_ReturnsPaginatedResult()
    {
        var result = await _repository.PageAsync(new PagedOrdersSpec(1, 2), TestContext.Current.CancellationToken);

        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(3);
        result.TotalPages.Should().Be(2);
    }

    [Fact]
    public async Task PageAsync_WithProjection_ReturnsPaginatedResult()
    {
        var result = await _repository.PageAsync(new PagedOrderSummarySpec(1, 2), TestContext.Current.CancellationToken);

        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(3);
    }

    #endregion
}
