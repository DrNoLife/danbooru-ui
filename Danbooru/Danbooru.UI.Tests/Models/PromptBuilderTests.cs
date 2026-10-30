using Danbooru.ApiWrapper.Models;
using Danbooru.UI.Models;
using FluentAssertions;

namespace Danbooru.UI.Tests.Models;

public class PromptBuilderTests
{
    [Fact]
    public void Build_ShouldOrderTagsLikeDanbooruTrainedModelsExpect()
    {
        var post = new Post
        {
            TagStringGeneral = "1girl long_hair smile solo",
            TagStringCharacter = "hatsune_miku",
            TagStringCopyright = "vocaloid",
            TagStringArtist = "some_artist",
            TagStringMeta = "highres commentary_request"
        };

        PromptBuilder.Build(post).Should().Be(
            "1girl, hatsune miku, vocaloid, some artist, long hair, smile, solo, masterpiece, best quality, amazing quality");
    }

    [Theory]
    [InlineData("shielder_(fate/grand_order)", @"shielder \(fate/grand order\)")]
    [InlineData("^_^", "^_^")]
    [InlineData("multiple_girls", "multiple girls")]
    public void Format_ShouldEscapeParenthesesAndKeepEmoticons(string tag, string expected)
    {
        PromptBuilder.Format(tag).Should().Be(expected);
    }

    [Fact]
    public void Build_ShouldPutEveryCountTagFirst()
    {
        var post = new Post { TagStringGeneral = "1boy 2girls hug multiple_girls", TagStringCharacter = "a" };

        PromptBuilder.Build(post).Should().StartWith("1boy, 2girls, multiple girls, a, hug");
    }
}
