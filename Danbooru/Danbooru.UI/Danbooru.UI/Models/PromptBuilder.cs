using Danbooru.ApiWrapper.Models;
using System.Text.RegularExpressions;

namespace Danbooru.UI.Models;

/// <summary>
/// Turns a post's tags into a prompt for anime image models trained on Danbooru tags (Illustrious, NoobAI and similar):
/// subject count, characters, series, artists, general tags, then quality tags.
/// </summary>
public static partial class PromptBuilder
{
    public static readonly IReadOnlyList<string> QualityTags = ["masterpiece", "best quality", "amazing quality"];

    public static string Build(Post post)
    {
        var counts = post.TagsGeneral.Where(IsCountTag);
        var general = post.TagsGeneral.Where(x => !IsCountTag(x));

        // Meta tags (highres, commentary, ...) describe the upload, not the picture, so they are left out.
        var tags = counts
            .Concat(post.TagsCharacter)
            .Concat(post.TagsCopyright)
            .Concat(post.TagsArtist)
            .Concat(general)
            .Select(Format)
            .Concat(QualityTags)
            .Distinct();

        return String.Join(", ", tags);
    }

    /// <summary>
    /// Spaces instead of underscores, and escaped parentheses, which prompt syntax uses for weighting.
    /// Short emoticon tags such as ^_^ keep their underscores.
    /// </summary>
    public static string Format(string tag)
    {
        var text = tag.Length <= 3 ? tag : tag.Replace('_', ' ');
        return text.Replace("(", @"\(").Replace(")", @"\)");
    }

    private static bool IsCountTag(string tag) => CountTag().IsMatch(tag);

    // 1girl, 2boys, 6+girls, 1other, multiple_girls, ...
    [GeneratedRegex(@"^(\d+\+?|multiple_)(girl|boy|other)s?$")]
    private static partial Regex CountTag();
}
