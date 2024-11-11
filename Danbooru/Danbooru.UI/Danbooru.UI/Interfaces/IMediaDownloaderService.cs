using Danbooru.ApiWrapper.Enums;
using Danbooru.ApiWrapper.Models;

namespace Danbooru.UI.Interfaces;

public interface IMediaDownloaderService
{
    Task DownloadMediaAsync(IEnumerable<Post> posts, List<TagAutocomplete> selectedTags, ContentRating? contentRating);
}
