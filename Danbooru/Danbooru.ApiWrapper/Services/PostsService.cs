using Danbooru.ApiWrapper.Helpers;
using Danbooru.ApiWrapper.Interfaces;
using Danbooru.ApiWrapper.Models;
using Danbooru.ApiWrapper.Models.Url;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Danbooru.ApiWrapper.Services;

public class PostsService : BaseService, IPostsService
{
    public PostsService(ILogger<PostsService> logger, IHttpClientFactory httpClientFactory) : base(logger, httpClientFactory) { }

    public async Task<List<Post>> GetPostsAsync()
    {
        _logger.LogInformation("Retrieving posts.");

        string uri = "posts.json";
        var result = await GetFromApiAsync<List<Post>>(uri);
        return result ?? new();
    }

    public async Task<List<Post>> GetPostsAsync(UrlParameters parameters)
    {
        _logger.LogInformation("Retrieving posts using parameters.");

        string uri = "posts.json" + parameters.ToString();
        var result = await GetFromApiAsync<List<Post>>(uri);
        return result ?? new();
    }

    public async Task<List<Post>> GetPostsByTagAsync(string tag)
    {
        List<string> tags = new()
        {
            tag
        };

        var result = await GetPostsByTagsAsync(tags);
        return result;
    }

    public async Task<List<Post>> GetPostsByTagAsync(string tag, UrlParameters parameters)
    {
        List<string> tags = new()
        {
            tag
        };

        var result = await GetPostsByTagsAsync(tags, parameters);
        return result;
    }

    public async Task<List<Post>> GetPostsByTagsAsync(IEnumerable<string> tags)
    {
        string tagStringConcatenated = String.Join('+', tags);
        string tagStringEscaped = UrlHelper.TransformTagStringToUrlFormat(tagStringConcatenated);

        string uri = $"posts.json?tags={tagStringEscaped}";
        var result = await GetFromApiAsync<List<Post>>(uri);
        return result ?? new();
    }

    public async Task<List<Post>> GetPostsByTagsAsync(IEnumerable<string> tags, UrlParameters parameters)
    {
        string tagStringConcatenated = String.Join('+', tags);
        string tagStringEscaped = UrlHelper.TransformTagStringToUrlFormat(tagStringConcatenated);

        string uri = $"posts.json{parameters.ToString()}&tags={tagStringEscaped}";
        var result = await GetFromApiAsync<List<Post>>(uri);
        return result ?? new();
    }

    public async Task<Post> GetPostByIdAsync(int id)
    {
        _logger.LogInformation("Retrieving post with id of {id} from API.", id);

        var result = await GetFromApiAsync<Post>($"posts/{id}.json");
        return result ?? new();
    }
}
