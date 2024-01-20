using Danbooru.ApiWrapper.Helpers;
using Danbooru.ApiWrapper.Interfaces;
using Danbooru.ApiWrapper.Models;
using Danbooru.ApiWrapper.Models.Url;
using Microsoft.Extensions.Logging;

namespace Danbooru.ApiWrapper.Services;

public class TagsService : BaseService, ITagsService
{
    public TagsService(ILogger<TagsService> logger, IHttpClientFactory httpClientFactory) : base(logger, httpClientFactory) { }

    public async Task<Tag> GetTagAsyncById(int id)
    {
        _logger.LogInformation("Searching for a specific tag, with id of {id}.", id);

        string uri = $"tags/{id}.json";
        var result = await GetFromApiAsync<Tag>(uri);
        return result ?? new();
    }

    public async Task<List<Tag>> GetTagsAsync()
    {
        _logger.LogInformation("Retriving tags.");

        string uri = "tags.json";
        var result = await GetFromApiAsync<List<Tag>>(uri);
        return result ?? new();
    }

    public async Task<List<Tag>> GetTagsAsync(UrlParameters parameters)
    {
        _logger.LogInformation("Retriving tags.");

        string uri = "tags.json" + parameters.ToString();
        var result = await GetFromApiAsync<List<Tag>>(uri);
        return result ?? new();
    }

    public async Task<List<Tag>> SearchAfterTagByNameAsync(string name)
    {
        _logger.LogInformation("Searching after tags based on name.");

        string uri = $"tags.json?commit=Search&search%5Bhide_empty%5D=yes&search%5Bname_or_alias_matches%5D={UrlHelper.TransformTagStringToUrlFormat(name)}";

        _logger.LogInformation(uri);

        var result = await GetFromApiAsync<List<Tag>>(uri);
        return result ?? new();
    }

    public async Task<List<Tag>> SearchAfterTagByNameAsync(string name, UrlParameters parameters)
    {
        _logger.LogInformation("Searching after tags based on name.");

        string uri = $"tags.json{parameters.ToString()}&commit=Search&search%5Bhide_empty%5D=yes&search%5Bname_or_alias_matches%5D={UrlHelper.TransformTagStringToUrlFormat(name)}";
        var result = await GetFromApiAsync<List<Tag>>(uri);
        return result ?? new();
    }

    public async Task<List<TagAutocomplete>> AutocompleteAfterTag(string searchQuery)
    {
        _logger.LogInformation("Trying to search after tags...");
        string uri = $"autocomplete.json?search%5Bquery%5D={searchQuery}&search%5Btype%5D=tag_query&limit=20";
        try
        {
            var result = await GetFromApiAsync<List<TagAutocomplete>>(uri);
            return result ?? new();
        }
        catch (Exception)
        {
            _logger.LogError("Failed to get content from uri: {uri}", uri);
            throw;
        }
    }
}
