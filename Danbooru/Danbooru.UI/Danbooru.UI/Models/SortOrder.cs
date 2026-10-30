namespace Danbooru.UI.Models;

public enum SortOrder
{
    Newest,
    Hot,
    Score,
    Favorites
}

public static class SortOrderExtensions
{
    public static readonly SortOrder[] Each = [SortOrder.Newest, SortOrder.Hot, SortOrder.Score, SortOrder.Favorites];

    /// <summary>
    /// The order: metatag. Unlike rating:, Danbooru counts it toward the tag limit.
    /// </summary>
    public static string ToMetatag(this SortOrder order) => order switch
    {
        SortOrder.Hot => "order:rank",
        SortOrder.Score => "order:score",
        SortOrder.Favorites => "order:favcount",
        _ => String.Empty
    };

    public static string Label(this SortOrder order) => order switch
    {
        SortOrder.Hot => "Hot",
        SortOrder.Score => "Top score",
        SortOrder.Favorites => "Most favorited",
        _ => "Newest"
    };

    /// <summary>
    /// The value used in the order= URL parameter; empty for the default.
    /// </summary>
    public static string UrlValue(this SortOrder order) => order == SortOrder.Newest ? String.Empty : order.ToString().ToLowerInvariant();

    public static SortOrder Parse(string? value) =>
        Enum.TryParse<SortOrder>(value, ignoreCase: true, out var order) && Enum.IsDefined(order) ? order : SortOrder.Newest;
}
