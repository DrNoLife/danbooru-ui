using Danbooru.UI.Models;
using FluentAssertions;

namespace Danbooru.UI.Tests.Models;

public class LinksTests
{
    [Fact]
    public void Browse_ShouldEscapeTags()
    {
        Links.Browse(SearchQuery.Parse("fate/grand_order -comic"), 3).Should().Be("/?tags=fate%2Fgrand_order%20-comic&page=3");
    }

    [Fact]
    public void Browse_ShouldBeRootForAnEmptyFirstPage()
    {
        Links.Browse(SearchQuery.Empty).Should().Be("/");
    }

    [Fact]
    public void Feed_ShouldCarryTheQuery()
    {
        Links.Feed(SearchQuery.Parse("black_&_white")).Should().Be("/feed?tags=black_%26_white");
    }
}
