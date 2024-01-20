using Danbooru.ApiWrapper.Models;
using FluentAssertions;
using System.Reflection;

namespace Danbooru.ApiWrapper.Tests.Models;

public class PostTests
{
    [Theory]
    [InlineData("", new string[] { })] 
    [InlineData("tag1 tag2", new[] { "tag1", "tag2" })] 
    public void TagsGeneral_ShouldReturnExpectedTags(string tagString, string[] expectedTags)
    {
        var post = new Post { TagStringGeneral = tagString };
        post.TagsGeneral.Should().BeEquivalentTo(expectedTags);
    }

    [Theory]
    [InlineData("", new string[] { })] 
    [InlineData("artist1 artist2", new[] { "artist1", "artist2" })] 
    public void TagsArtist_ShouldReturnExpectedTags(string tagString, string[] expectedTags)
    {
        var post = new Post { TagStringArtist = tagString };
        post.TagsArtist.Should().BeEquivalentTo(expectedTags);
    }

    [Theory]
    [InlineData("", new string[] { })] 
    [InlineData("character1 character2", new[] { "character1", "character2" })] 
    [InlineData("1girl alternate_costume blush", new[] { "1girl", "alternate_costume", "blush" })]
    public void TagsCharacter_ShouldReturnExpectedTags(string tagString, string[] expectedTags)
    {
        var post = new Post { TagStringCharacter = tagString };
        post.TagsCharacter.Should().BeEquivalentTo(expectedTags);
    }

    [Theory]
    [InlineData("", new string[] { })] 
    [InlineData("copyright1 copyright2", new[] { "copyright1", "copyright2" })] 
    public void TagsCopyright_ShouldReturnExpectedTags(string tagString, string[] expectedTags)
    {
        var post = new Post { TagStringCopyright = tagString };
        post.TagsCopyright.Should().BeEquivalentTo(expectedTags);
    }

    [Theory]
    [InlineData("", new string[] { })] 
    [InlineData("meta1 meta2", new[] { "meta1", "meta2" })] 
    public void TagsMeta_ShouldReturnExpectedTags(string tagString, string[] expectedTags)
    {
        var post = new Post { TagStringMeta = tagString };
        post.TagsMeta.Should().BeEquivalentTo(expectedTags);
    }

    [Theory]
    [InlineData("jpg", true)]
    [InlineData("png", true)]
    [InlineData("webm", false)]
    [InlineData("mp4", false)]
    [InlineData("zip", false)]
    public void IsImage_ShouldReturnCorrectValue_BasedOnFileExtension(string fileExt, bool expectedResult)
    {
        var post = new Post { FileExt = fileExt };
        post.IsImage.Should().Be(expectedResult);
    }

    [Fact]
    public void GetTagsListFromString_ShouldReturnCorrectTags()
    {
        var post = new Post();
        var method = post.GetType().GetMethod("GetTagsListFromString", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new NullReferenceException();
        var result = method.Invoke(post, new object[] { "tag1 tag2" }) as List<string>;

        result.Should().Contain(new[] { "tag1", "tag2" });
    }
}
