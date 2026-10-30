using Danbooru.ApiWrapper.Helpers;
using FluentAssertions;

namespace Danbooru.ApiWrapper.Tests.Helpers;

public class UrlHelperTests
{
    [Theory]
    [InlineData("hatsune_miku", "hatsune_miku")]
    [InlineData("Hatsune Miku", "hatsune_miku")]
    [InlineData("shielder_(fate/grand_order)+blush", "shielder_%28fate%2Fgrand_order%29+blush")]
    [InlineData("black_&_white", "black_%26_white")]
    [InlineData("hatsune_miku++rating:general", "hatsune_miku+rating%3Ageneral")]
    public void TransformTagStringToUrlFormat_ShouldEscapeEachTag(string tagString, string expected)
    {
        UrlHelper.TransformTagStringToUrlFormat(tagString).Should().Be(expected);
    }
}
