using Danbooru.ApiWrapper.Enums;
using Danbooru.ApiWrapper.Helpers;
using Danbooru.ApiWrapper.Interfaces;
using Danbooru.ApiWrapper.Models;
using Danbooru.ApiWrapper.Models.Url;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Danbooru.ApiWrapper.Services;

public class PostsService(ILogger<PostsService> logger, IHttpClientFactory httpClientFactory) : BaseService(logger, httpClientFactory), IPostsService
{
    public async Task<List<Post>> GetPostsAsync()
    {
        _logger.LogInformation("Retrieving posts.");

        string uri = "posts.json";
        var result = await GetFromApiAsync<List<Post>>(uri);
        return result ?? [];
    }

    public async Task<List<Post>> GetPostsAsync(UrlParameters parameters)
    {
        _logger.LogInformation("Retrieving posts using parameters.");

        string uri = "posts.json" + parameters.ToString();
        var result = await GetFromApiAsync<List<Post>>(uri);
        return result ?? [];
    }

    public async Task<List<Post>> GetPostsByIdAsync(int postId, int postsToGet)
    {
        _logger.LogInformation("Retriving {postsToGet} based on the post Id {postId}", postsToGet, postId);

        string uri = $"posts.json?page=b{postId}&limit={postsToGet}";
        var result = await GetFromApiAsync<List<Post>>(uri);
        return result ?? [];
    }

    public async Task<List<Post>> GetPostsByTagAsync(string tag)
    {
        return await GetPostsByTagsAsync([tag]);
    }

    public async Task<List<Post>> GetPostsByTagAsync(string tag, UrlParameters parameters)
    {
        return await GetPostsByTagsAsync([tag], parameters);
    }

    public async Task<List<Post>> GetPostsByTagsAsync(IEnumerable<string> tags)
    {
        string tagStringConcatenated = String.Join('+', tags);
        string tagStringEscaped = UrlHelper.TransformTagStringToUrlFormat(tagStringConcatenated);

        string uri = $"posts.json?tags={tagStringEscaped}";
        var result = await GetFromApiAsync<List<Post>>(uri);
        return result ?? [];
    }

    public async Task<List<Post>> GetPostsByTagsAsync(IEnumerable<string> tags, UrlParameters parameters)
    {
        string tagStringConcatenated = String.Join('+', tags);
        string tagStringEscaped = UrlHelper.TransformTagStringToUrlFormat(tagStringConcatenated);

        string uri = $"posts.json{parameters.ToString()}&tags={tagStringEscaped}";
        var result = await GetFromApiAsync<List<Post>>(uri);
        return result ?? [];
    }

    public async Task<Post> GetPostByIdAsync(int id)
    {
        _logger.LogInformation("Retrieving post with id of {id} from API.", id);

        var result = await GetFromApiAsync<Post>($"posts/{id}.json");
        return result ?? new();
    }

    public async Task<List<Post>> SearchAsync(string tagQuery, string page, int limit, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Searching posts for {tagQuery}, page {page}.", tagQuery, page);

        string uri = $"posts.json?page={Uri.EscapeDataString(page)}&limit={limit}&tags={ToTagParameter(tagQuery)}";
        return await GetFromApiAsync<List<Post>>(uri, cancellationToken) ?? [];
    }

    public async Task<int?> CountAsync(string tagQuery, CancellationToken cancellationToken = default)
    {
        var result = await GetFromApiAsync<PostCounts>($"counts/posts.json?tags={ToTagParameter(tagQuery)}", cancellationToken);
        return result?.Counts.Posts;
    }

    private static string ToTagParameter(string tagQuery)
    {
        return UrlHelper.TransformTagStringToUrlFormat(String.Join('+', tagQuery.Split(' ', StringSplitOptions.RemoveEmptyEntries)));
    }

    public async Task<List<Post>> PerformSearchAfterPostsAsync(List<TagAutocomplete> tagsThatMustBeIncluded, ContentRating? contentRating, int lastPostId = -1, CancellationToken cancellationToken = default)
    {
        int postsToGet = 10;

        _logger.LogInformation("Performing search after posts.");

        List<string> tagNames = tagsThatMustBeIncluded.Select(x => x.Value).ToList();

        // Generate the search URI.
        var tagsToUseForSearch = tagNames.Take(2);
        var tagsToUseConcatenated = String.Join('+', tagsToUseForSearch);

        if (contentRating is not null)
        {
            tagsToUseConcatenated += $"+rating:{contentRating}";
        }

        var tagsToUseEscaped = UrlHelper.TransformTagStringToUrlFormat(tagsToUseConcatenated);
        string uri = $"posts.json?page=b{lastPostId}&limit={postsToGet}&tags={tagsToUseEscaped}";

        // Perform search.
        return await GetFromApiAsync<List<Post>>(uri, cancellationToken) ?? [];
    }
}
