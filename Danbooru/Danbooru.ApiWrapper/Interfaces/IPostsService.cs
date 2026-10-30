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

    /// <summary>
    /// Searches posts with a raw Danbooru tag query, e.g. "cute_weapon -comic rating:g,s".
    /// </summary>
    /// <param name="page">A page number, or "b{id}" for the posts before an id.</param>
    public Task<List<Post>> SearchAsync(string tagQuery, string page, int limit, CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts the posts matching a raw Danbooru tag query. Null when Danbooru does not report a count.
    /// </summary>
    public Task<int?> CountAsync(string tagQuery, CancellationToken cancellationToken = default);

    public Task<List<Post>> PerformSearchAfterPostsAsync(List<TagAutocomplete> tagsThatMustBeIncluded, ContentRating? contentRating, int lastPostId = -1, CancellationToken cancellationToken = default);
}
