using Danbooru.UI.Models;

namespace Danbooru.UI.Services;

/// <summary>
/// Viewer preferences that shape every search. Kept in a cookie, so prerendering already uses them and the first page
/// does not load twice. See <see cref="Components.App"/> and <see cref="Components.Routes"/>.
/// </summary>
public class Preferences(ImejiJs js)
{
    public const string CookieName = "imeji-prefs";

    /// <summary>
    /// Grid row heights in px, smallest to largest.
    /// </summary>
    public static readonly int[] RowHeights = [140, 180, 240, 300, 380];

    private const Rating DefaultRatings = Rating.General | Rating.Sensitive;
    private const int DefaultSizeIndex = 2;

    public Rating Ratings { get; private set; } = DefaultRatings;
    public int SizeIndex { get; private set; } = DefaultSizeIndex;
    public int RowHeight => RowHeights[SizeIndex];

    public event Action? Changed;

    /// <summary>
    /// Loads the cookie value, e.g. "r=gs;s=2". Unknown or broken values fall back to the defaults.
    /// </summary>
    public void Load(string? cookie)
    {
        foreach (var part in (cookie ?? String.Empty).Split(';', StringSplitOptions.RemoveEmptyEntries))
        {
            var pair = part.Split('=', 2);
            if (pair.Length != 2)
            {
                continue;
            }

            switch (pair[0])
            {
                case "r":
                    var ratings = pair[1].Select(x => RatingExtensions.FromLetter(x.ToString())).Aggregate(Rating.None, (all, x) => all | x);
                    Ratings = ratings == Rating.None ? DefaultRatings : ratings;
                    break;
                case "s" when int.TryParse(pair[1], out var size):
                    SizeIndex = Math.Clamp(size, 0, RowHeights.Length - 1);
                    break;
            }
        }
    }

    public string Serialize() => $"r={String.Concat(RatingExtensions.Each.Where(x => Ratings.HasFlag(x)).Select(x => x.Letter()))};s={SizeIndex}";

    /// <summary>
    /// Turns one rating on or off. The last rating cannot be turned off, since an empty set would mean nothing to show.
    /// </summary>
    public Task ToggleRatingAsync(Rating rating)
    {
        var next = Ratings ^ rating;
        return next == Rating.None ? Task.CompletedTask : SaveAsync(() => Ratings = next);
    }

    public Task ChangeSizeAsync(int delta) => SaveAsync(() => SizeIndex = Math.Clamp(SizeIndex + delta, 0, RowHeights.Length - 1));

    private async Task SaveAsync(Action change)
    {
        change();
        Changed?.Invoke();
        await js.SetCookieAsync(CookieName, Serialize());
    }
}
