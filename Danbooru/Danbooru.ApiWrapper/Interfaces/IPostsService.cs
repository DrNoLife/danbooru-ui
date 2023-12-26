using Danbooru.ApiWrapper.Enums;
using Danbooru.ApiWrapper.Models;
using Danbooru.ApiWrapper.Models.Url;

namespace Danbooru.ApiWrapper.Interfaces;

public interface IPostsService
{
    public Task<List<Post>> GetPostsAsync();
    public Task<List<Post>> GetPostsAsync(UrlParameters parameters);

    public Task<List<Post>> GetPostsByIdAsync(int postId, int postsToGet);

    public Task<List<Post>> GetPostsByTagAsync(string tag);
    public Task<List<Post>> GetPostsByTagAsync(string tag, UrlParameters parameters);

    public Task<List<Post>> GetPostsByTagsAsync(IEnumerable<string> tags);
    public Task<List<Post>> GetPostsByTagsAsync(IEnumerable<string> tags, UrlParameters parameters);

    public Task<Post> GetPostByIdAsync(int id);

    public Task<List<Post>> PerformSearchAfterPostsAsync(List<TagAutocomplete> tagsThatMustBeIncluded, ContentRating? contentRating, int lastPostId = -1);
}
