using Danbooru.ApiWrapper.Models;
using Danbooru.ApiWrapper.Models.Url;

namespace Danbooru.ApiWrapper.Interfaces;

public interface ITagsService
{
    public Task<Tag> GetTagAsyncById(int id);

    public Task<List<Tag>> GetTagsAsync();
    public Task<List<Tag>> GetTagsAsync(UrlParameters parameters);

    public Task<List<Tag>> SearchAfterTagByNameAsync(string name);
    public Task<List<Tag>> SearchAfterTagByNameAsync(string name, UrlParameters parameters);

    public Task<List<TagAutocomplete>> AutocompleteAfterTag(string searchQuery);
}
