namespace Danbooru.ApiWrapper.Helpers;

public static class UrlHelper
{
    public static string TransformTagStringToUrlFormat(string tagString)
    {
        // Replace spaces with '+'
        string modified = tagString.Replace(" ", "_");

        // Replace '(' with '%28' and ')' with '%29'
        modified = modified.Replace("(", "%28").Replace(")", "%29");

        // Convert to lowercase
        modified = modified.ToLower();

        return modified;
    }
}
