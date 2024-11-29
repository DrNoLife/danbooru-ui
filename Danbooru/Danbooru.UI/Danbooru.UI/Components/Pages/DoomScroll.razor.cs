using Danbooru.ApiWrapper.Enums;
using Danbooru.ApiWrapper.Interfaces;
using Danbooru.ApiWrapper.Models;
using Danbooru.UI.Interfaces;
using Danbooru.UI.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Danbooru.UI.Components.Pages;

public partial class DoomScroll(
    IDanbooruWrapper danbooruWrapper,
    ILogger<DoomScroll> logger,
    IDoomScrollService doomScrollService,
    IMediaDownloaderService mediaDownloaderService,
    IOptions<DanbooruSettings> settings)
{
    private readonly IDanbooruWrapper _danbooruWrapper = danbooruWrapper;
    private readonly ILogger<DoomScroll> _logger = logger;
    private readonly IDoomScrollService _doomScrollService = doomScrollService;
    private readonly IMediaDownloaderService _mediaDownloaderService = mediaDownloaderService;
    private readonly DanbooruSettings _settings = settings.Value;

    private List<Post> _posts = [];

    private int _lastIdThatWasRetrieved = -1;
    private bool _shouldDisplayTagContainer;

    private CancellationTokenSource _cancellationTokenSource = new();
    private const int _debouncePeriod = 250;
    private string _currentInputValue = String.Empty;
    private string _searchQuery = String.Empty;
    private List<TagAutocomplete>? _tags;
    private List<TagAutocomplete> _selectedTags = [];
    private ContentRating? _contentRating;

    protected override void OnInitialized()
    {
        _doomScrollService.TagContainerToggled += HandleTagContainerToggled;
    }

    private void HandleTagContainerToggled(object? sender, EventArgs e)
    {
        _shouldDisplayTagContainer = _doomScrollService.DisplayTagContainer;
        StateHasChanged();
    }

    private void RemoveSelectedTag(TagAutocomplete tag)
    {
        _logger.LogDebug("Removing selected tag.");

        _selectedTags?.Remove(tag);
    }

    private void HandleOnRadioButtonClick(ContentRating contentRating)
    {
        _logger.LogDebug("Changing the selected content rating.");

        if (_contentRating == contentRating)
        {
            _contentRating = null;
            return;
        }

        _contentRating = contentRating;
    }

    private async Task HandleSearchButtonClicked()
    {
        _logger.LogDebug("Search button has been pressed.");

        _posts.Clear();
        await FetchImages();
    }

    private async Task OnSearchInputChanged(ChangeEventArgs args)
    {
        string inputValue = args.Value?.ToString()?.Trim() ?? String.Empty;

        if (String.IsNullOrEmpty(inputValue))
        {
            _tags?.Clear();
            return;
        }

        _currentInputValue = inputValue;

        // If user started typing before we could process the previous request, then cancel that request.
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            await Task.Delay(_debouncePeriod, _cancellationTokenSource.Token);
            await PerformSearchAsync();
        }
        catch (TaskCanceledException) { }
    }

    private async Task PerformSearchAsync()
    {
        if (String.IsNullOrEmpty(_currentInputValue))
        {
            return;
        }

        _tags = await _danbooruWrapper.Tags.AutocompleteAfterTag(_currentInputValue);
    }

    private void HandleTagSelected(TagAutocomplete selectedTag)
    {
        if (_selectedTags.Contains(selectedTag))
        {
            return;
        }

        _selectedTags.Add(selectedTag);
        _tags?.Clear();

        _searchQuery = String.Empty;

        StateHasChanged();
    }

    public void Dispose() => _cancellationTokenSource?.Cancel();

    /// <summary>
    /// Yoink a bunch of images.
    /// </summary>
    /// <returns></returns>
    private async Task FetchImages()
    {
        var posts = await _danbooruWrapper.Posts.PerformSearchAfterPostsAsync(_selectedTags, _contentRating, _lastIdThatWasRetrieved);

        var distinctPosts = posts.Where(p => !_posts.Any(existingPost => existingPost.Id == p.Id)).ToList();

        if (distinctPosts.Count == 0)
        {
            _logger.LogInformation("Found no more new posts!");
            return;
        }

        _posts.AddRange(distinctPosts);

        _lastIdThatWasRetrieved = _posts.Min(x => x.Id);

        if (_settings.AutoDownloadDoomscrollImages)
        {
            // Use the media downloader service to download media.
            await _mediaDownloaderService.DownloadMediaAsync(distinctPosts, _selectedTags, _contentRating);
        }

        _logger.LogInformation("Found a total of {newUniquePostsFound} new posts, bringing the new total up to {newPostTotal}. The last id used was {lastIdUsed}", distinctPosts.Count, _posts.Count, _lastIdThatWasRetrieved);
    }

}
