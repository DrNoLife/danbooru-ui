using Danbooru.UI.Interfaces;
using Danbooru.UI.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace Danbooru.UI.Services;

public class GallerySettingsService : IGallerySettingsService
{
    private readonly NavigationManager _navigationManager;
    private GalleryOptionsModel _currentOptions;

    public GallerySettingsService(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
        LoadCurrentSettings();
    }

    private void LoadCurrentSettings()
    {
        var uri = new Uri(_navigationManager.Uri);
        var query = QueryHelpers.ParseQuery(uri.Query);

        _currentOptions = new GalleryOptionsModel();

        if (query.Count > 0)
        {
            _currentOptions.AspectRatio = query.TryGetValue("aspect-ratio", out var ar) ? ar : "1";
            _currentOptions.DisplayType = query.TryGetValue("display", out var dp) ? dp : "gallery";
            _currentOptions.ImageWidth = query.TryGetValue("image-width", out var iw) && int.TryParse(iw, out var width) ? width : 250;
        }
    }

    public GalleryOptionsModel GetGalleryOptions()
    {
        return _currentOptions;
    }

    public void UpdateSetting(Action<GalleryOptionsModel> updateAction)
    {
        updateAction?.Invoke(_currentOptions);
    }

    public string GenerateQueryString()
    {
        var queryParams = new Dictionary<string, string>
        {
            { "aspect-ratio", _currentOptions.AspectRatio },
            { "display", _currentOptions.DisplayType },
            { "image-width", _currentOptions.ImageWidth.ToString() }
        };

        return QueryHelpers.AddQueryString("", queryParams);
    }

    public void NavigateWithUpdatedSettings()
    {
        var queryString = GenerateQueryString();
        _navigationManager.NavigateTo(_navigationManager.Uri.Split('?')[0] + queryString);
    }

    public T GetSetting<T>(Func<GalleryOptionsModel, T> getAction)
    {
        return getAction(_currentOptions);
    }
}
