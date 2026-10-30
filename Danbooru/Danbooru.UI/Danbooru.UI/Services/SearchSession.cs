using Danbooru.UI.Models;
using System.Text.Json;

namespace Danbooru.UI.Services;

/// <summary>
/// What the viewer is searching for in this tab: the current query, the posts it has loaded (so the viewer can step
/// through them), the search palette, and saved and recent searches (kept in local storage).
/// </summary>
public class SearchSession(ImejiJs js)
{
    private const string SavedKey = "imeji-saved";
    private const string RecentKey = "imeji-recent";
    private const int MaxRecent = 8;

    private List<string> _saved = [];
    private List<string> _recent = [];
    private bool _storageLoaded;

    public SearchQuery Query { get; private set; } = SearchQuery.Empty;

    /// <summary>
    /// The posts the grid or feed loaded for <see cref="Query"/>, in order. The viewer uses them for previous/next.
    /// </summary>
    public IReadOnlyList<PostSummary> Results { get; private set; } = [];
    public int? ResultTotal { get; private set; }

    public bool IsPaletteOpen { get; private set; }

    /// <summary>
    /// The grid's loaded pages, kept so going back from the viewer shows the same pages instead of starting over.
    /// </summary>
    public BrowseCache? BrowseCache { get; set; }

    /// <summary>
    /// The posts the feed loaded, kept for the same reason as <see cref="BrowseCache"/>.
    /// </summary>
    public FeedCache? FeedCache { get; set; }

    /// <summary>
    /// The post the viewer showed last, so the grid can scroll back to it.
    /// </summary>
    public int? LastViewedPostId { get; set; }

    public IReadOnlyList<string> Saved => _saved;
    public IReadOnlyList<string> Recent => _recent;

    public event Action? Changed;

    public void SetQuery(SearchQuery query)
    {
        if (query == Query)
        {
            return;
        }

        Query = query;
        Results = [];
        ResultTotal = null;
        Changed?.Invoke();
    }

    public void SetResults(IReadOnlyList<PostSummary> results, int? total)
    {
        Results = results;
        ResultTotal = total;
    }

    public void OpenPalette()
    {
        IsPaletteOpen = true;
        Changed?.Invoke();
    }

    public void ClosePalette()
    {
        IsPaletteOpen = false;
        Changed?.Invoke();
    }

    public async Task EnsureStorageLoadedAsync()
    {
        if (_storageLoaded)
        {
            return;
        }

        _saved = await ReadListAsync(SavedKey);
        _recent = await ReadListAsync(RecentKey);
        _storageLoaded = true;
    }

    public bool IsSaved(SearchQuery query) => _saved.Contains(query.ToString());

    public async Task ToggleSavedAsync(SearchQuery query)
    {
        await EnsureStorageLoadedAsync();

        var text = query.ToString();
        if (!_saved.Remove(text))
        {
            _saved.Insert(0, text);
        }

        await js.SetStorageAsync(SavedKey, JsonSerializer.Serialize(_saved));
        Changed?.Invoke();
    }

    public async Task AddRecentAsync(SearchQuery query)
    {
        if (query.IsEmpty)
        {
            return;
        }

        await EnsureStorageLoadedAsync();

        var text = query.ToString();
        _recent.Remove(text);
        _recent.Insert(0, text);
        _recent = _recent.Take(MaxRecent).ToList();

        await js.SetStorageAsync(RecentKey, JsonSerializer.Serialize(_recent));
    }

    private async Task<List<string>> ReadListAsync(string key)
    {
        try
        {
            var json = await js.GetStorageAsync(key);
            return String.IsNullOrEmpty(json) ? [] : JsonSerializer.Deserialize<List<string>>(json) ?? [];
        }
        catch (JsonException)
        {
            // Someone edited local storage by hand. Start over rather than fail the palette.
            return [];
        }
    }
}
