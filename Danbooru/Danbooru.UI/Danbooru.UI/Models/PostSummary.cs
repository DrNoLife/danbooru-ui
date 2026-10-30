using Danbooru.ApiWrapper.Models;

namespace Danbooru.UI.Models;

/// <summary>
/// The part of a post that grids, the feed and the viewer's strip need. Small enough to keep a long result list in memory
/// and to hand from prerendering to the interactive circuit.
/// </summary>
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
    string Tags,
    DateTime CreatedAt)
{
    /// <summary>
    /// Width divided by height, clamped so panoramas and long strips still fit a justified row.
    /// </summary>
    public double LayoutRatio => Height <= 0 ? 1 : Math.Clamp((double)Width / Height, 0.5, 2.4);

    /// <summary>
    /// Danbooru hides some posts (e.g. banned artists) from anonymous users, which leaves them without URLs.
    /// </summary>
    public bool HasMedia => !String.IsNullOrEmpty(PreviewUrl);

    public static PostSummary From(Post post) => new(
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
        String.Join(' ', post.TagsCharacter.Concat(post.TagsCopyright).Concat(post.TagsGeneral)),
        post.CreatedAt);
}
