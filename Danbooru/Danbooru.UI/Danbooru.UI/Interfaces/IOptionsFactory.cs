using Danbooru.UI.Models;

namespace Danbooru.UI.Interfaces;

public interface IOptionsFactory
{
    GalleryOptionsModel GetGalleryOptionsAsync();
}
