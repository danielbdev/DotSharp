using DotSharp.Primitives.Pagination;
using FluentAssertions;
using Xunit;

namespace DotSharp.Primitives.Tests.Pagination;

public sealed class PaginationResultTests
{
    #region Create

    [Fact]
    public void Create_WhenValidParameters_SetsProperties()
    {
        int[] items = [1, 2, 3];

        var result = PaginationResult<int>.Create(items, 30, 1, 10);

        result.Items.Should().BeEquivalentTo(items);
        result.TotalCount.Should().Be(30);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    [Fact]
    public void Create_WhenPageNumberIsZero_ThrowsArgumentOutOfRangeException()
    {
        Action act = () => PaginationResult<int>.Create([], 10, 0, 10);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Create_WhenPageNumberIsNegative_ThrowsArgumentOutOfRangeException()
    {
        Action act = () => PaginationResult<int>.Create([], 10, -1, 10);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Create_WhenPageSizeIsZero_ThrowsArgumentOutOfRangeException()
    {
        Action act = () => PaginationResult<int>.Create([], 10, 1, 0);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Create_WhenPageSizeIsNegative_ThrowsArgumentOutOfRangeException()
    {
        Action act = () => PaginationResult<int>.Create([], 10, 1, -1);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Create_WhenTotalCountIsNegative_ThrowsArgumentOutOfRangeException()
    {
        Action act = () => PaginationResult<int>.Create([], -1, 1, 10);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Create_WhenItemsIsNull_ReturnsEmptyItems()
    {
        PaginationResult<int> result = PaginationResult<int>.Create(null!, 0, 1, 10);

        result.Items.Should().BeEmpty();
    }

    #endregion

    #region TotalPages

    [Fact]
    public void TotalPages_WhenItemsDivideEvenly_ReturnsCorrectCount()
    {
        PaginationResult<int> result = PaginationResult<int>.Create([], 30, 1, 10);

        result.TotalPages.Should().Be(3);
    }

    [Fact]
    public void TotalPages_WhenItemsDoNotDivideEvenly_RoundsUp()
    {
        PaginationResult<int> result = PaginationResult<int>.Create([], 31, 1, 10);

        result.TotalPages.Should().Be(4);
    }

    [Fact]
    public void TotalPages_WhenTotalCountIsZero_ReturnsZero()
    {
        PaginationResult<int> result = PaginationResult<int>.Create([], 0, 1, 10);

        result.TotalPages.Should().Be(0);
    }

    #endregion

    #region HasPreviousPage

    [Fact]
    public void HasPreviousPage_WhenOnFirstPage_ReturnsFalse()
    {
        PaginationResult<int> result = PaginationResult<int>.Create([], 30, 1, 10);

        result.HasPreviousPage.Should().BeFalse();
    }

    [Fact]
    public void HasPreviousPage_WhenOnSecondPage_ReturnsTrue()
    {
        PaginationResult<int> result = PaginationResult<int>.Create([], 30, 2, 10);

        result.HasPreviousPage.Should().BeTrue();
    }

    #endregion

    #region HasNextPage

    [Fact]
    public void HasNextPage_WhenOnLastPage_ReturnsFalse()
    {
        PaginationResult<int> result = PaginationResult<int>.Create([], 30, 3, 10);

        result.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public void HasNextPage_WhenNotOnLastPage_ReturnsTrue()
    {
        PaginationResult<int> result = PaginationResult<int>.Create([], 30, 1, 10);

        result.HasNextPage.Should().BeTrue();
    }

    #endregion

    #region Items

    [Fact]
    public void Items_IsReadOnly()
    {
        PaginationResult<int> result = PaginationResult<int>.Create([1, 2, 3], 3, 1, 10);

        result.Items.Should().BeAssignableTo<IReadOnlyList<int>>();
    }

    #endregion
}
