namespace Danbooru.ApiWrapper.Models.Url;

public class UrlParameters
{
    /// <summary>
    /// Specifies pagination page.
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Specifies how many items to retrieve for each page. Default is 25.
    /// </summary>
    public int PageItems { get; set; } = 25;

    public override string ToString()
    {
        return $"?page={Page}&limit={PageItems}";
    }
}
