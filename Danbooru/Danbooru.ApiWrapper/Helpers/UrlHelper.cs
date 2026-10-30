namespace Danbooru.ApiWrapper.Helpers;

public static class UrlHelper
{
    public static string TransformTagStringToUrlFormat(string tagString)
    {
        // Tags are separated by '+'. Escape each tag on its own, so characters like '&', '#', '/' and '(' survive the query string.
        var tags = tagString
            .ToLower()
            .Split('+', StringSplitOptions.RemoveEmptyEntries)
            .Select(tag => Uri.EscapeDataString(tag.Trim().Replace(' ', '_')));

        return String.Join('+', tags);
    }
}
