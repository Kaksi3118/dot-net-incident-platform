using FluentAssertions;
using OpsRelay.Application.Common.Models;
using Xunit;

namespace OpsRelay.UnitTests.Application;

public class PagedResultDtoTests
{
    [Fact]
    public void ComputedPropertiesReflectPagePosition()
    {
        var result = new PagedResultDto<int>([1, 2, 3], 2, 3, 7);

        result.TotalPages.Should().Be(3);
        result.HasNextPage.Should().BeTrue();
        result.HasPreviousPage.Should().BeTrue();
    }

    [Fact]
    public void FirstPageHasNoPreviousPage()
    {
        var result = new PagedResultDto<int>([1, 2], 1, 2, 4);

        result.TotalPages.Should().Be(2);
        result.HasNextPage.Should().BeTrue();
        result.HasPreviousPage.Should().BeFalse();
    }

    [Fact]
    public void ZeroPageSizeProducesNoPages()
    {
        var result = new PagedResultDto<int>([], 1, 0, 0);

        result.TotalPages.Should().Be(0);
        result.HasNextPage.Should().BeFalse();
        result.HasPreviousPage.Should().BeFalse();
    }
}
