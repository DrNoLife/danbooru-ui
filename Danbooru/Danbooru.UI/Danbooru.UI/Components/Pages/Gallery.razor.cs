using Danbooru.ApiWrapper.Interfaces;
using Danbooru.ApiWrapper.Models.Url;
using Danbooru.ApiWrapper.Models;
using Danbooru.UI.Interfaces;
using Danbooru.UI.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace Danbooru.UI.Components.Pages;

public partial class Gallery(
    IDanbooruWrapper danbooruWrapper, 
    IGallerySettingsService gallerySettingsService)
{
    private readonly IDanbooruWrapper _danbooruWrapper = danbooruWrapper;
    private readonly IGallerySettingsService _gallerySettingsService = gallerySettingsService;

    [Parameter]
    public int PageNumber { get; set; }

    [Parameter]
    public string? SearchTerm { get; set; }

    private List<Post> _posts = [];

    private int _paginationMinPageNumber;
    private int _paginationMaxPageNumber;
    private int _internalPageNumber = 1;
    private int _paginationStep = 5;

    private GalleryOptionsModel _options = new();

    protected override async Task OnInitializedAsync()
    {
        _options = _gallerySettingsService.GetGalleryOptions();

        CalculatePaginationStuff();

        UrlParameters parameters = new ()
        {
            PageItems = 50,
            Page = _internalPageNumber
        };

        _posts = String.IsNullOrEmpty(SearchTerm)
            ? await _danbooruWrapper.Posts.GetPostsAsync(parameters)
            : await _danbooruWrapper.Posts.GetPostsByTagsAsync(SearchTerm.Split(' '), parameters);
    }

    private void CalculatePaginationStuff()
    {
        _internalPageNumber = PageNumber == 0 ? 1 : PageNumber;
        _paginationMinPageNumber = Math.Max(_internalPageNumber - _paginationStep, 1);
        _paginationMaxPageNumber = _internalPageNumber + _paginationStep;
    }
}
