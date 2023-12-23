using Danbooru.UI.Interfaces;
using Danbooru.UI.Models;

namespace Danbooru.UI.Services;

public class OptionsFactory : IOptionsFactory
{
    private readonly ICookieService _cookieService;

    public OptionsFactory(ICookieService cookieService)
    {
        _cookieService = cookieService;
    }

    public GalleryOptionsModel GetGalleryOptionsAsync()
    {
        return GalleryOptionsModel.GetSettingsAsync(_cookieService);
    }

}
