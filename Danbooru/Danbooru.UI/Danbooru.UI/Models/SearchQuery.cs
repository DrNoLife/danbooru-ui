namespace Danbooru.UI.Models;

/// <summary>
/// The tags a search includes and excludes, and how it is sorted. Ratings are a viewer preference and are added separately.
/// </summary>
public sealed record SearchQuery
{
    /// <summary>
    /// Danbooru rejects anonymous searches with more tags than this. Excluded tags and a sort order count, the rating metatag does not.
    /// </summary>
    public const int AnonymousTagLimit = 2;

    public static readonly SearchQuery Empty = new([], []);

    public SearchQuery(IEnumerable<string> include, IEnumerable<string> exclude, SortOrder order = SortOrder.Newest)
    {
        Include = include.Select(Normalize).Where(x => x.Length > 0).Distinct().ToList();
        Exclude = exclude.Select(Normalize).Where(x => x.Length > 0 && !Include.Contains(x)).Distinct().ToList();
        Order = order;
    }

    public IReadOnlyList<string> Include { get; }
    public IReadOnlyList<string> Exclude { get; }
    public SortOrder Order { get; }

    /// <summary>
    /// No tags. The sort order does not matter here: "Hot" with no tags is still everything.
    /// </summary>
    public bool IsEmpty => Include.Count == 0 && Exclude.Count == 0;
    public int TagCount => Include.Count + Exclude.Count + (Order == SortOrder.Newest ? 0 : 1);
    public bool IsOverTagLimit => TagCount > AnonymousTagLimit;

    /// <summary>
    /// Parses the space separated form used in URLs, e.g. "cute_weapon -comic".
    /// </summary>
    public static SearchQuery Parse(string? text, SortOrder order = SortOrder.Newest)
    {
        var tokens = (text ?? String.Empty).Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return new SearchQuery(
            tokens.Where(x => !x.StartsWith('-')),
            tokens.Where(x => x.StartsWith('-')).Select(x => x[1..]),
            order);
    }

    public SearchQuery WithInclude(string tag) => new(Include.Append(tag), Exclude.Where(x => x != Normalize(tag)), Order);
    public SearchQuery WithExclude(string tag) => new(Include.Where(x => x != Normalize(tag)), Exclude.Append(tag), Order);
    public SearchQuery Without(string tag) => new(Include.Where(x => x != tag), Exclude.Where(x => x != tag), Order);
    public SearchQuery WithOrder(SortOrder order) => new(Include, Exclude, order);

    /// <summary>
    /// The tags in their space separated form, e.g. "cute_weapon -comic". Used in URLs and shown to the user.
    /// </summary>
    public override string ToString() => String.Join(' ', Include.Concat(Exclude.Select(x => "-" + x)));

    /// <summary>
    /// The full Danbooru tag query, including the sort and rating metatags.
    /// </summary>
    public string ToDanbooruQuery(Rating ratings) =>
        String.Join(' ', new[] { ToString(), Order.ToMetatag(), ratings.ToMetatag() }.Where(x => x.Length > 0));

    public bool Equals(SearchQuery? other) => other is not null && ToString() == other.ToString() && Order == other.Order;
    public override int GetHashCode() => HashCode.Combine(ToString(), Order);

    private static string Normalize(string tag) => tag.Trim().Replace(' ', '_').ToLowerInvariant();
}
