using DotSharp.Persistence.Abstractions.Pagination;
using DotSharp.Persistence.Abstractions.Specifications;
using FluentAssertions;
using Xunit;

namespace DotSharp.Persistence.Abstractions.Tests.Specifications;

public sealed class SpecificationTests
{
    #region Test doubles

    private sealed class Order
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }

    private sealed class OrderByCustomerSpec : Specification<Order>
    {
        public OrderByCustomerSpec(string customerName)
        {
            SetCriteria(x => x.CustomerName == customerName);
        }
    }

    private sealed class ActiveOrdersSpec : Specification<Order>
    {
        public ActiveOrdersSpec()
        {
            SetCriteria(x => !x.IsDeleted);
        }

        public void AndByCustomer(string customerName)
        {
            AndCriteria(x => x.CustomerName == customerName);
        }
    }

    private sealed class PagedOrdersSpec : Specification<Order>
    {
        public PagedOrdersSpec(int page, int size)
        {
            SetCriteria(x => !x.IsDeleted);
            ApplyPaging(new Paging(page, size));
            ApplyOrderBy(x => x.CreatedAt);
        }
    }

    private sealed class OrderWithIncludeSpec : Specification<Order>
    {
        public OrderWithIncludeSpec()
        {
            AddInclude(x => x.CustomerName);
            AddInclude("Customer.Address");
        }
    }

    private sealed class OrderProjectionSpec : Specification<Order, string>
    {
        public OrderProjectionSpec()
        {
            SetCriteria(x => !x.IsDeleted);
            ApplySelector(x => x.CustomerName);
        }
    }

    private sealed class TrackingSpec : Specification<Order>
    {
        public TrackingSpec() => AsTracking();
    }

    private sealed class NoTrackingSpec : Specification<Order>
    {
        public NoTrackingSpec() => AsNoTracking();
    }

    private sealed class NoTrackingWithIdentityResolutionSpec : Specification<Order>
    {
        public NoTrackingWithIdentityResolutionSpec() => AsNoTrackingWithIdentityResolution();
    }

    private sealed class SplitQuerySpec : Specification<Order>
    {
        public SplitQuerySpec() => AsSplitQuery();
    }

    private sealed class ThenOrderBySpec : Specification<Order>
    {
        public ThenOrderBySpec()
        {
            ApplyOrderBy(x => x.CreatedAt);
            ThenOrderBy(x => x.CustomerName);
        }
    }

    #endregion

    #region SetCriteria

    [Fact]
    public void SetCriteria_SetsCriteriaExpression()
    {
        OrderByCustomerSpec spec = new OrderByCustomerSpec("John");

        spec.Criteria.Should().NotBeNull();
    }

    [Fact]
    public void SetCriteria_DefaultCriteria_ReturnsTrue()
    {
        ActiveOrdersSpec spec = new ActiveOrdersSpec();
        Order order = new Order { IsDeleted = false };

        Func<Order, bool> compiled = spec.Criteria.Compile();

        compiled(order).Should().BeTrue();
    }

    #endregion

    #region AndCriteria

    [Fact]
    public void AndCriteria_WhenNoPreviousCriteria_SetsCriteriaDirectly()
    {
        ActiveOrdersSpec spec = new ActiveOrdersSpec();
        spec.AndByCustomer("John");

        Order order = new Order { IsDeleted = false, CustomerName = "John" };
        Func<Order, bool> compiled = spec.Criteria.Compile();

        compiled(order).Should().BeTrue();
    }

    [Fact]
    public void AndCriteria_CombinesTwoCriteria()
    {
        ActiveOrdersSpec spec = new ActiveOrdersSpec();
        spec.AndByCustomer("John");

        Order deletedOrder = new Order { IsDeleted = true, CustomerName = "John" };
        Func<Order, bool> compiled = spec.Criteria.Compile();

        compiled(deletedOrder).Should().BeFalse();
    }

    [Fact]
    public void AndCriteria_WhenOneConditionFails_ReturnsFalse()
    {
        ActiveOrdersSpec spec = new ActiveOrdersSpec();
        spec.AndByCustomer("John");

        Order wrongCustomer = new Order { IsDeleted = false, CustomerName = "Jane" };
        Func<Order, bool> compiled = spec.Criteria.Compile();

        compiled(wrongCustomer).Should().BeFalse();
    }

    #endregion

    #region Paging

    [Fact]
    public void ApplyPaging_SetsPaging()
    {
        PagedOrdersSpec spec = new PagedOrdersSpec(2, 10);

        spec.Paging.Should().NotBeNull();
        spec.Paging!.Page.Should().Be(2);
        spec.Paging.Size.Should().Be(10);
    }

    [Fact]
    public void Paging_WhenNotApplied_IsNull()
    {
        ActiveOrdersSpec spec = new ActiveOrdersSpec();

        spec.Paging.Should().BeNull();
    }

    #endregion

    #region Ordering

    [Fact]
    public void ApplyOrderBy_AddsOrderExpression()
    {
        PagedOrdersSpec spec = new PagedOrdersSpec(1, 10);

        spec.OrderExpressions.Should().ContainSingle();
        spec.OrderExpressions[0].IsThenBy.Should().BeFalse();
        spec.OrderExpressions[0].Direction.Should().Be(OrderDirection.Asc);
    }

    [Fact]
    public void ThenOrderBy_AddsSecondaryOrderExpression()
    {
        ThenOrderBySpec spec = new ThenOrderBySpec();

        spec.OrderExpressions.Should().HaveCount(2);
        spec.OrderExpressions[1].IsThenBy.Should().BeTrue();
    }

    #endregion

    #region Includes

    [Fact]
    public void AddInclude_WithExpression_AddsToIncludes()
    {
        OrderWithIncludeSpec spec = new OrderWithIncludeSpec();

        spec.Includes.Should().ContainSingle();
    }

    [Fact]
    public void AddInclude_WithString_AddsToIncludeStrings()
    {
        OrderWithIncludeSpec spec = new OrderWithIncludeSpec();

        spec.IncludeStrings.Should().ContainSingle()
            .Which.Should().Be("Customer.Address");
    }

    #endregion

    #region Tracking

    [Fact]
    public void AsTracking_SetsIsTrackingTrue()
    {
        TrackingSpec spec = new TrackingSpec();

        spec.IsTracking.Should().BeTrue();
        spec.IsNoTracking.Should().BeFalse();
        spec.IsNoTrackingWithIdentityResolution.Should().BeFalse();
    }

    [Fact]
    public void AsNoTracking_SetsIsNoTrackingTrue()
    {
        NoTrackingSpec spec = new NoTrackingSpec();

        spec.IsNoTracking.Should().BeTrue();
        spec.IsTracking.Should().BeFalse();
        spec.IsNoTrackingWithIdentityResolution.Should().BeFalse();
    }

    [Fact]
    public void AsNoTrackingWithIdentityResolution_SetsCorrectFlags()
    {
        NoTrackingWithIdentityResolutionSpec spec = new NoTrackingWithIdentityResolutionSpec();

        spec.IsNoTrackingWithIdentityResolution.Should().BeTrue();
        spec.IsTracking.Should().BeFalse();
        spec.IsNoTracking.Should().BeFalse();
    }

    #endregion

    #region SplitQuery

    [Fact]
    public void AsSplitQuery_SetsIsSplitQueryTrue()
    {
        SplitQuerySpec spec = new SplitQuerySpec();

        spec.IsSplitQuery.Should().BeTrue();
    }

    #endregion

    #region Selector

    [Fact]
    public void ApplySelector_SetsSelectorExpression()
    {
        OrderProjectionSpec spec = new OrderProjectionSpec();

        spec.Selector.Should().NotBeNull();
    }

    [Fact]
    public void ApplySelector_CompilesCorrectly()
    {
        OrderProjectionSpec spec = new OrderProjectionSpec();
        Order order = new Order { CustomerName = "John" };

        Func<Order, string> compiled = spec.Selector.Compile();

        compiled(order).Should().Be("John");
    }

    #endregion
}
