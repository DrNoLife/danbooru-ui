namespace Danbooru.UI.Models;

/// <summary>
/// Builds the app's URLs in one place, so tags are always escaped the same way.
/// </summary>
public static class Links
{
    public static string Browse(SearchQuery query, int page = 1) => Build("/", query, page);

    public static string Feed(SearchQuery query) => Build("/feed", query, page: 1);

    /// <summary>
    /// A search for one tag, e.g. from a tag list.
    /// </summary>
    public static string Tag(string tag) => Browse(new SearchQuery([tag], []));

    public static string Post(int id) => $"/post/{id}";

    public static string Download(int id) => $"/download/{id}";

    public static string Original(int id) => $"https://danbooru.donmai.us/posts/{id}";

    private static string Build(string path, SearchQuery query, int page)
    {
        var parameters = new List<string>();
        if (!query.IsEmpty)
        {
            parameters.Add("tags=" + Uri.EscapeDataString(query.ToString()));
        }

        if (query.Order != SortOrder.Newest)
        {
            parameters.Add("order=" + query.Order.UrlValue());
        }

        if (page > 1)
        {
            parameters.Add("page=" + page);
        }

        return parameters.Count == 0 ? path : path + "?" + String.Join('&', parameters);
    }
}
