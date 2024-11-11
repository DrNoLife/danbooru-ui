using Danbooru.ApiWrapper.Enums;
using Danbooru.ApiWrapper.Models;
using Danbooru.UI.Interfaces;

namespace Danbooru.UI.Services;

public class MediaDownloaderService(ILogger<MediaDownloaderService> logger, IHttpClientFactory httpClientFactory, IWebHostEnvironment env) : IMediaDownloaderService
{
    private readonly ILogger<MediaDownloaderService> _logger = logger;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly IWebHostEnvironment _env = env;

    public async Task DownloadMediaAsync(IEnumerable<Post> posts, List<TagAutocomplete> selectedTags, ContentRating? contentRating)
    {
        // Generate the folder name based on tags and content rating
        string folderName = GenerateFolderName(selectedTags, contentRating);

        // Use ContentRootPath to ensure the folder is created in the application's root directory
        string contentFolder = Path.Combine(_env.ContentRootPath, "content", folderName);

        // Log the folder path for debugging
        _logger.LogInformation("Media will be downloaded to folder: {ContentFolder}", contentFolder);

        if (!Directory.Exists(contentFolder))
        {
            Directory.CreateDirectory(contentFolder);
        }

        // Download each media file asynchronously
        var downloadTasks = posts.Select(post => DownloadMediaAsync(post, contentFolder));
        await Task.WhenAll(downloadTasks);
    }


    private string GenerateFolderName(List<TagAutocomplete> selectedTags, ContentRating? contentRating)
    {
        string tagPart = selectedTags != null && selectedTags.Count > 0
            ? String.Join("_", selectedTags.Select(t => t.Label))
            : "NoTags";
        string ratingPart = contentRating?.ToString() ?? "NoRating";

        string folderName = $"{tagPart}_{ratingPart}";

        // Sanitize folder name to remove invalid characters
        foreach (char c in Path.GetInvalidFileNameChars())
        {
            folderName = folderName.Replace(c, '_');
        }

        return folderName;
    }

    private async Task DownloadMediaAsync(Post post, string folderPath)
    {
        string mediaUrl = GetMediaUrl(post);
        if (String.IsNullOrEmpty(mediaUrl))
        {
            return;
        }

        // Determine the filename to save
        string fileExtension = Path.GetExtension(mediaUrl);
        string fileName = $"{post.Id}{fileExtension}";
        string filePath = Path.Combine(folderPath, fileName);

        if (File.Exists(filePath))
        {
            _logger.LogInformation("Media already exists at {FilePath}", filePath);
            return;
        }

        using var client = _httpClientFactory.CreateClient();

        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (compatible; danbooru-ui/1.0)");

        try
        {
            byte[] mediaBytes = await client.GetByteArrayAsync(mediaUrl);
            await File.WriteAllBytesAsync(filePath, mediaBytes);

            _logger.LogInformation("Downloaded media {MediaUrl} to {FilePath}", mediaUrl, filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download media from {MediaUrl}", mediaUrl);
        }
    }

    private static string GetMediaUrl(Post post)
    {
        if (!String.IsNullOrEmpty(post.LargeFileUrl))
        {
            return post.LargeFileUrl;
        }

        if (!String.IsNullOrEmpty(post.FileUrl))
        {
            return post.FileUrl;
        }

        if (!String.IsNullOrEmpty(post.PreviewFileUrl))
        {
            return post.PreviewFileUrl;
        }

        return null;
    }
}

