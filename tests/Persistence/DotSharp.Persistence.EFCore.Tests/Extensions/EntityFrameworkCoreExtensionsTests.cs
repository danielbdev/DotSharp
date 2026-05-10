using DotSharp.Persistence.EFCore.Extensions;
using FluentAssertions;
using Xunit;

namespace DotSharp.Persistence.EFCore.Tests.Extensions;

public sealed class EntityFrameworkCoreExtensionsTests
{
    private sealed class TestItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    [Fact]
    public void Paginate_CalculatesSkipAndTakeCorrectly()
    {
        var source = new List<TestItem>
        {
            new() { Id = 1 }, new() { Id = 2 }, new() { Id = 3 },
            new() { Id = 4 }, new() { Id = 5 }, new() { Id = 6 }
        }.AsQueryable();

        // Page 2, Size 2 should return IDs 3 and 4
        var result = source.Paginate(2, 2).ToList();

        result.Should().HaveCount(2);
        result[0].Id.Should().Be(3);
        result[1].Id.Should().Be(4);
    }

    [Fact]
    public void IncludeList_WhenCalled_ReturnsQueryable()
    {
        // testing Include with InMemory is limited as it doesn't do anything 
        // unless you actually have relationships. 
        // But we can verify the extension doesn't crash.
        
        var source = new List<TestItem>().AsQueryable();
        
        var result = source.IncludeList(x => x.Name);
        
        result.Should().NotBeNull();
    }
}
