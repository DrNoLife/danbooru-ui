using Danbooru.UI.Models;
using FluentAssertions;

namespace Danbooru.UI.Tests.Models;

public class SearchQueryTests
{
    [Fact]
    public void Parse_ShouldSplitIncludedAndExcludedTags()
    {
        var query = SearchQuery.Parse("  cute_weapon   -comic ");

        query.Include.Should().Equal("cute_weapon");
        query.Exclude.Should().Equal("comic");
        query.ToString().Should().Be("cute_weapon -comic");
    }

    [Fact]
    public void Parse_ShouldTreatNullAsEmpty()
    {
        SearchQuery.Parse(null).IsEmpty.Should().BeTrue();
    }

    [Fact]
    public void WithExclude_ShouldMoveAnIncludedTagToExcluded()
    {
        var query = SearchQuery.Parse("blush").WithExclude("blush");

        query.Include.Should().BeEmpty();
        query.Exclude.Should().Equal("blush");
    }

    [Fact]
    public void Tags_ShouldBeNormalizedToDanbooruForm()
    {
        new SearchQuery(["Hatsune Miku"], []).ToString().Should().Be("hatsune_miku");
    }

    [Theory]
    [InlineData("a b", false)]
    [InlineData("a -b", false)]
    [InlineData("a b -c", true)]
    public void IsOverTagLimit_ShouldCountExcludedTags(string text, bool expected)
    {
        SearchQuery.Parse(text).IsOverTagLimit.Should().Be(expected);
    }

    [Theory]
    [InlineData(Rating.All, "fate/grand_order -comic")]
    [InlineData(Rating.General | Rating.Sensitive, "fate/grand_order -comic rating:g,s")]
    [InlineData(Rating.Explicit, "fate/grand_order -comic rating:e")]
    public void ToDanbooruQuery_ShouldAddRatingMetatag(Rating ratings, string expected)
    {
        SearchQuery.Parse("fate/grand_order -comic").ToDanbooruQuery(ratings).Should().Be(expected);
    }

    [Fact]
    public void SortOrder_ShouldCountTowardTheTagLimit()
    {
        var query = SearchQuery.Parse("a b", SortOrder.Score);

        query.IsOverTagLimit.Should().BeTrue();
        query.ToDanbooruQuery(Rating.All).Should().Be("a b order:score");
    }

    [Fact]
    public void WithInclude_ShouldKeepTheSortOrder()
    {
        SearchQuery.Parse("a", SortOrder.Hot).WithInclude("b").Order.Should().Be(SortOrder.Hot);
    }

    [Fact]
    public void Equality_ShouldIncludeTheSortOrder()
    {
        SearchQuery.Parse("a", SortOrder.Hot).Should().NotBe(SearchQuery.Parse("a"));
    }

    [Fact]
    public void Equality_ShouldCompareTags()
    {
        SearchQuery.Parse("a -b").Should().Be(new SearchQuery(["a"], ["b"]));
    }
}
