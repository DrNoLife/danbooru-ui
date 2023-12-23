using Danbooru.UI.Interfaces;

namespace Danbooru.UI.Models;

public class GalleryOptionsModel
{
    public string AspectRatio { get; set; } = "1";

    public static GalleryOptionsModel GetSettingsAsync(ICookieService cookieService)
    {
        GalleryOptionsModel options = new()
        {
            AspectRatio = cookieService.GetCookie("aspect-ratio")
        };

        return options;
    }
}
