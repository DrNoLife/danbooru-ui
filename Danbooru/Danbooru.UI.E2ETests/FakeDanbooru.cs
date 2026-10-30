using Danbooru.ApiWrapper.Interfaces;
using Danbooru.ApiWrapper.Models;
using Danbooru.ApiWrapper.Models.Url;

namespace Danbooru.UI.E2ETests;

/// <summary>
/// Stands in for Danbooru, so the browser tests are fast and do not depend on the real site.
/// Serves <see cref="PageCount"/> pages of posts that all use a local image.
/// </summary>
public sealed class FakeDanbooru : IDanbooruWrapper
{
    public const int PageCount = 5;
    public const int FirstId = 100_000;

    public IPostsService Posts { get; init; } = new FakePosts();
    public ITagsService Tags { get; init; } = new FakeTags();

    private sealed class FakePosts : IPostsService
    {
        public Task<List<Post>> SearchAsync(string tagQuery, string page, int limit, CancellationToken cancellationToken = default)
        {
            // "b{id}" asks for the posts below an id; a number asks for a page.
            var startId = page.StartsWith('b') ? int.Parse(page[1..]) - 1 : FirstId - (int.Parse(page) - 1) * limit;
            var lowestId = FirstId - PageCount * limit;
            var posts = Enumerable.Range(0, limit)
                .Select(i => startId - i)
                .Where(id => id > lowestId)
                .Select(CreatePost)
                .ToList();

            return Task.FromResult(posts);
        }

        public Task<int?> CountAsync(string tagQuery, CancellationToken cancellationToken = default) => Task.FromResult<int?>(PageCount * 50);

        public Task<Post> GetPostByIdAsync(int id) => Task.FromResult(CreatePost(id));

        public Task<List<Post>> GetPostsAsync() => throw new NotSupportedException();
        public Task<List<Post>> GetPostsAsync(UrlParameters parameters) => throw new NotSupportedException();
        public Task<List<Post>> GetPostsByIdAsync(int postId, int postsToGet) => throw new NotSupportedException();
        public Task<List<Post>> GetPostsByTagAsync(string tag) => throw new NotSupportedException();
        public Task<List<Post>> GetPostsByTagAsync(string tag, UrlParameters parameters) => throw new NotSupportedException();
        public Task<List<Post>> GetPostsByTagsAsync(IEnumerable<string> tags) => throw new NotSupportedException();
        public Task<List<Post>> GetPostsByTagsAsync(IEnumerable<string> tags, UrlParameters parameters) => throw new NotSupportedException();
    }

    private sealed class FakeTags : ITagsService
    {
        public Task<List<TagAutocomplete>> AutocompleteAfterTag(string searchQuery, CancellationToken cancellationToken = default)
        {
            var tags = new[] { "hatsune_miku", "hatsune_miku_(append)" }
                .Where(x => x.StartsWith(searchQuery, StringComparison.OrdinalIgnoreCase))
                .Select(x => new TagAutocomplete { Value = x, Label = x.Replace('_', ' '), Category = 4, PostCount = 1000 })
                .ToList();

            return Task.FromResult(tags);
        }

        public Task<Tag> GetTagAsyncById(int id) => throw new NotSupportedException();
        public Task<List<Tag>> GetTagsAsync() => throw new NotSupportedException();
        public Task<List<Tag>> GetTagsAsync(UrlParameters parameters) => throw new NotSupportedException();
        public Task<List<Tag>> SearchAfterTagByNameAsync(string name) => throw new NotSupportedException();
        public Task<List<Tag>> SearchAfterTagByNameAsync(string name, UrlParameters parameters) => throw new NotSupportedException();
    }

    private static Post CreatePost(int id)
    {
        const string image = "/favicon/android-icon-192x192.png";

        // Alternate portrait and landscape so the grid has real rows.
        var portrait = id % 2 == 0;
        return new Post
        {
            Id = id,
            Width = portrait ? 600 : 900,
            Height = portrait ? 900 : 600,
            FileExt = "png",
            Rating = "g",
            CreatedAt = DateTime.Now.AddMinutes(-(FirstId - id)),
            FileUrl = image,
            LargeFileUrl = image,
            PreviewFileUrl = image,
            TagStringArtist = "test_artist",
            TagStringCharacter = "hatsune_miku",
            TagStringGeneral = "1girl long_hair smile",
            MediaAsset = new MediaAsset
            {
                Variants = [new Variant { Type = "360x360", Url = image, Width = 192, Height = 192 }]
            }
        };
    }
}
