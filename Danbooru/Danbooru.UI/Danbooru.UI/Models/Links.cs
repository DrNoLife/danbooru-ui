namespace Danbooru.UI.Models;

/// <summary>
/// Builds the app's URLs in one place, so tags are always escaped the same way.
/// </summary>
public static class Links
{
    public static string Browse(SearchQuery query, int page = 1)
    {
        var parameters = new List<string>();
        if (!query.IsEmpty)
        {
            parameters.Add("tags=" + Uri.EscapeDataString(query.ToString()));
        }

        if (page > 1)
        {
            parameters.Add("page=" + page);
        }

        return parameters.Count == 0 ? "/" : "/?" + String.Join('&', parameters);
    }

    public static string Feed(SearchQuery query) => query.IsEmpty ? "/feed" : "/feed?tags=" + Uri.EscapeDataString(query.ToString());

    public static string Post(int id) => $"/post/{id}";

    public static string Download(int id) => $"/download/{id}";

    public static string Original(int id) => $"https://danbooru.donmai.us/posts/{id}";
}
