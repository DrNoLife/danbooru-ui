namespace Danbooru.UI.Models;

/// <summary>
/// Content ratings as a set, so several can be shown at once.
/// </summary>
[Flags]
public enum Rating
{
    None = 0,
    General = 1,
    Sensitive = 2,
    Questionable = 4,
    Explicit = 8,
    All = General | Sensitive | Questionable | Explicit
}

public static class RatingExtensions
{
    public static readonly Rating[] Each = [Rating.General, Rating.Sensitive, Rating.Questionable, Rating.Explicit];

    /// <summary>
    /// The single letter Danbooru uses for a rating, both in the API and in the rating: metatag.
    /// </summary>
    public static string Letter(this Rating rating) => rating switch
    {
        Rating.General => "g",
        Rating.Sensitive => "s",
        Rating.Questionable => "q",
        Rating.Explicit => "e",
        _ => throw new ArgumentOutOfRangeException(nameof(rating), rating, "Only single ratings have a letter.")
    };

    public static Rating FromLetter(string? letter) => letter switch
    {
        "g" => Rating.General,
        "s" => Rating.Sensitive,
        "q" => Rating.Questionable,
        "e" => Rating.Explicit,
        _ => Rating.None
    };

    /// <summary>
    /// The rating: metatag for a set, or an empty string when every rating is allowed. Danbooru does not count it toward the tag limit.
    /// </summary>
    public static string ToMetatag(this Rating ratings)
    {
        if (ratings is Rating.All or Rating.None)
        {
            return String.Empty;
        }

        return "rating:" + String.Join(',', Each.Where(x => ratings.HasFlag(x)).Select(x => x.Letter()));
    }

    public static string Label(this Rating ratings) => ratings switch
    {
        Rating.All or Rating.None => "All ratings",
        Rating.General => "Safe",
        Rating.General | Rating.Sensitive => "Safe-ish",
        _ => String.Join(" + ", Each.Where(x => ratings.HasFlag(x)).Select(x => x.ToString()))
    };

    /// <summary>
    /// CSS modifier used for the rating's colors, e.g. "g" in rating-g.
    /// </summary>
    public static string CssClass(this Rating rating) => "rating-" + rating.Letter();
}
