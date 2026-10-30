using Danbooru.ApiWrapper.Helpers;
using Danbooru.ApiWrapper.Interfaces;
using Danbooru.ApiWrapper.Models;
using Danbooru.ApiWrapper.Models.Url;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Danbooru.ApiWrapper.Services;

public class PostsService(ILogger<PostsService> logger, IHttpClientFactory httpClientFactory, HybridCache? cache = null) : BaseService(logger, httpClientFactory, cache), IPostsService
{
    public async Task<List<Post>> GetPostsAsync()
    {
        _logger.LogDebug("Retrieving posts.");

        string uri = "posts.json";
        var result = await GetFromApiAsync<List<Post>>(uri);
        return result ?? [];
    }

    public async Task<List<Post>> GetPostsAsync(UrlParameters parameters)
    {
        _logger.LogDebug("Retrieving posts using parameters.");

        string uri = "posts.json" + parameters.ToString();
        var result = await GetFromApiAsync<List<Post>>(uri);
        return result ?? [];
    }

    public async Task<List<Post>> GetPostsByIdAsync(int postId, int postsToGet)
    {
        _logger.LogDebug("Retriving {postsToGet} based on the post Id {postId}", postsToGet, postId);

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
        _logger.LogDebug("Retrieving post with id of {id} from API.", id);

        var result = await GetCachedAsync<Post>($"posts/{id}.json", TimeSpan.FromMinutes(5));
        return result ?? new();
    }

    public async Task<List<Post>> SearchAsync(string tagQuery, string page, int limit, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Searching posts for {tagQuery}, page {page}.", tagQuery, page);

        string uri = $"posts.json?page={Uri.EscapeDataString(page)}&limit={limit}&tags={ToTagParameter(tagQuery)}";
        // Short, so "latest" stays fresh while back/forward and the prerender-then-interactive pass reuse the result.
        return await GetCachedAsync<List<Post>>(uri, TimeSpan.FromSeconds(30), cancellationToken) ?? [];
    }

    public async Task<int?> CountAsync(string tagQuery, CancellationToken cancellationToken = default)
    {
        var result = await GetCachedAsync<PostCounts>($"counts/posts.json?tags={ToTagParameter(tagQuery)}", TimeSpan.FromMinutes(5), cancellationToken);
        return result?.Counts.Posts;
    }

    private static string ToTagParameter(string tagQuery)
    {
        return UrlHelper.TransformTagStringToUrlFormat(String.Join('+', tagQuery.Split(' ', StringSplitOptions.RemoveEmptyEntries)));
    }
}
