namespace Danbooru.UI.Models;

/// <summary>
/// One page of search results. The grid shows each page as its own section with a marker.
/// </summary>
public sealed record PostPage(int Number, List<PostSummary> Posts);

/// <summary>
/// The first page and count loaded while prerendering, handed to the interactive circuit so it does not load them again.
/// </summary>
/// <param name="Key">The search the snapshot belongs to; a snapshot for another search is ignored.</param>
/// <param name="Related">Refine suggestions, worked out from the full tag lists before they were trimmed.</param>
public sealed record BrowseSnapshot(string Key, PostPage FirstPage, int? Total, List<string> Related);

/// <summary>
/// The feed's first batch, handed from prerendering to the interactive circuit like <see cref="BrowseSnapshot"/>.
/// </summary>
public sealed record FeedSnapshot(string Key, List<PostSummary> Posts);

/// <summary>
/// Everything the grid loaded for one search, see <see cref="Services.SearchSession.BrowseCache"/>.
/// </summary>
public sealed record BrowseCache(string Key, List<PostPage> Pages, int? Total, bool ReachedEnd, List<string> Related);

/// <summary>
/// Everything the feed loaded for one search, see <see cref="Services.SearchSession.FeedCache"/>.
/// </summary>
/// <param name="NextPage">The next page number, for sort orders that page by number instead of by id.</param>
public sealed record FeedCache(string Key, List<PostSummary> Posts, bool ReachedEnd, int NextPage);
