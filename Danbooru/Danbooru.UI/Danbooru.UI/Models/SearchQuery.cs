namespace Danbooru.UI.Models;

/// <summary>
/// The tags a search includes and excludes. Ratings are a viewer preference and are added separately.
/// </summary>
public sealed record SearchQuery
{
    /// <summary>
    /// Danbooru rejects anonymous searches with more tags than this. Excluded tags count, the rating metatag does not.
    /// </summary>
    public const int AnonymousTagLimit = 2;

    public static readonly SearchQuery Empty = new([], []);

    public SearchQuery(IEnumerable<string> include, IEnumerable<string> exclude)
    {
        Include = include.Select(Normalize).Where(x => x.Length > 0).Distinct().ToList();
        Exclude = exclude.Select(Normalize).Where(x => x.Length > 0 && !Include.Contains(x)).Distinct().ToList();
    }

    public IReadOnlyList<string> Include { get; }
    public IReadOnlyList<string> Exclude { get; }

    public bool IsEmpty => Include.Count == 0 && Exclude.Count == 0;
    public int TagCount => Include.Count + Exclude.Count;
    public bool IsOverTagLimit => TagCount > AnonymousTagLimit;

    /// <summary>
    /// Parses the space separated form used in URLs, e.g. "cute_weapon -comic".
    /// </summary>
    public static SearchQuery Parse(string? text)
    {
        var tokens = (text ?? String.Empty).Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return new SearchQuery(
            tokens.Where(x => !x.StartsWith('-')),
            tokens.Where(x => x.StartsWith('-')).Select(x => x[1..]));
    }

    public SearchQuery WithInclude(string tag) => new(Include.Append(tag), Exclude.Where(x => x != Normalize(tag)));
    public SearchQuery WithExclude(string tag) => new(Include.Where(x => x != Normalize(tag)), Exclude.Append(tag));
    public SearchQuery Without(string tag) => new(Include.Where(x => x != tag), Exclude.Where(x => x != tag));

    /// <summary>
    /// The space separated form, e.g. "cute_weapon -comic". Used in URLs and shown to the user.
    /// </summary>
    public override string ToString() => String.Join(' ', Include.Concat(Exclude.Select(x => "-" + x)));

    /// <summary>
    /// The full Danbooru tag query, including the rating metatag.
    /// </summary>
    public string ToDanbooruQuery(Rating ratings) => String.Join(' ', new[] { ToString(), ratings.ToMetatag() }.Where(x => x.Length > 0));

    public bool Equals(SearchQuery? other) => other is not null && ToString() == other.ToString();
    public override int GetHashCode() => ToString().GetHashCode();

    private static string Normalize(string tag) => tag.Trim().Replace(' ', '_').ToLowerInvariant();
}
