using Danbooru.ApiWrapper.Helpers;
using Danbooru.ApiWrapper.Interfaces;
using Danbooru.ApiWrapper.Models;
using Danbooru.ApiWrapper.Models.Url;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Danbooru.ApiWrapper.Services;

public class TagsService(ILogger<TagsService> logger, IHttpClientFactory httpClientFactory, HybridCache? cache = null) : BaseService(logger, httpClientFactory, cache), ITagsService
{
    public async Task<Tag> GetTagAsyncById(int id)
    {
        _logger.LogDebug("Searching for a specific tag, with id of {id}.", id);

        string uri = $"tags/{id}.json";
        var result = await GetFromApiAsync<Tag>(uri);
        return result ?? new();
    }

    public async Task<List<Tag>> GetTagsAsync()
    {
        _logger.LogDebug("Retriving tags.");

        string uri = "tags.json";
        var result = await GetFromApiAsync<List<Tag>>(uri);
        return result ?? [];
    }

    public async Task<List<Tag>> GetTagsAsync(UrlParameters parameters)
    {
        _logger.LogDebug("Retriving tags.");

        string uri = "tags.json" + parameters.ToString();
        var result = await GetFromApiAsync<List<Tag>>(uri);
        return result ?? [];
    }

    public async Task<List<Tag>> SearchAfterTagByNameAsync(string name)
    {
        _logger.LogDebug("Searching after tags based on name.");

        string uri = $"tags.json?commit=Search&search%5Bhide_empty%5D=yes&search%5Bname_or_alias_matches%5D={UrlHelper.TransformTagStringToUrlFormat(name)}";

        _logger.LogDebug("Tag search uri: {uri}", uri);

        var result = await GetFromApiAsync<List<Tag>>(uri);
        return result ?? [];
    }

    public async Task<List<Tag>> SearchAfterTagByNameAsync(string name, UrlParameters parameters)
    {
        _logger.LogDebug("Searching after tags based on name.");

        string uri = $"tags.json{parameters.ToString()}&commit=Search&search%5Bhide_empty%5D=yes&search%5Bname_or_alias_matches%5D={UrlHelper.TransformTagStringToUrlFormat(name)}";
        var result = await GetFromApiAsync<List<Tag>>(uri);
        return result ?? [];
    }

    public async Task<List<TagAutocomplete>> AutocompleteAfterTag(string searchQuery, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Trying to search after tags...");
        string uri = $"autocomplete.json?search%5Bquery%5D={Uri.EscapeDataString(searchQuery)}&search%5Btype%5D=tag_query&limit=20";
        try
        {
            var result = await GetCachedAsync<List<TagAutocomplete>>(uri, TimeSpan.FromMinutes(10), cancellationToken);
            return result ?? [];
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to get content from uri: {uri}", uri);
            throw;
        }
    }
}
