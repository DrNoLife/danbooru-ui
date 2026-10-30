using Danbooru.ApiWrapper.Models;

namespace Danbooru.UI.Models;

/// <summary>
/// The part of a post that grids, the feed and the viewer's strip need. Small enough to keep a long result list in memory
/// and to hand from prerendering to the interactive circuit.
/// </summary>
/// <param name="TopTags">The first few character, series and general tags, space separated.</param>
/// <param name="TagCount">How many tags the post has in total.</param>
public sealed record PostSummary(
    int Id,
    int Width,
    int Height,
    string PreviewUrl,
    string SampleUrl,
    string SrcSet,
    bool IsVideo,
    double? Duration,
    Rating Rating,
    string ArtistTags,
    string TopTags,
    int TagCount,
    DateTime CreatedAt)
{
    private const int TopTagCount = 6;

    /// <summary>
    /// Width divided by height, clamped so panoramas and long strips still fit a justified row.
    /// </summary>
    public double LayoutRatio => Height <= 0 ? 1 : Math.Clamp((double)Width / Height, 0.5, 2.4);

    /// <summary>
    /// Danbooru hides some posts (e.g. banned artists) from anonymous users, which leaves them without URLs.
    /// </summary>
    public bool HasMedia => !String.IsNullOrEmpty(PreviewUrl);

    public static PostSummary From(Post post)
    {
        var tags = DescriptiveTags(post).ToList();
        return new(
            post.Id,
            post.Width,
            post.Height,
            post.GetVariantUrl("360x360") ?? post.PreviewFileUrl,
            String.IsNullOrEmpty(post.LargeFileUrl) ? post.FileUrl : post.LargeFileUrl,
            post.ScaledVariantsSrcSet,
            !post.IsImage,
            post.MediaAsset?.Duration,
            RatingExtensions.FromLetter(post.Rating),
            post.TagStringArtist,
            String.Join(' ', tags.Take(TopTagCount)),
            tags.Count,
            post.CreatedAt);
    }

    /// <summary>
    /// The tags that say what is in a post: characters and series first, then general tags.
    /// </summary>
    public static IEnumerable<string> DescriptiveTags(Post post) => post.TagsCharacter.Concat(post.TagsCopyright).Concat(post.TagsGeneral);

    /// <summary>
    /// The tags that show up most often in a set of posts, as suggestions for narrowing a search.
    /// </summary>
    public static List<string> RelatedTags(IEnumerable<Post> posts, SearchQuery query, int count = 7)
    {
        var current = query.Include.Concat(query.Exclude).ToHashSet();
        return posts
            .SelectMany(DescriptiveTags)
            .Where(x => !current.Contains(x))
            .GroupBy(x => x)
            .Where(x => x.Count() >= 3)
            .OrderByDescending(x => x.Count())
            .Take(count)
            .Select(x => x.Key)
            .ToList();
    }
}
