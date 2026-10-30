using Danbooru.ApiWrapper.Interfaces;

namespace Danbooru.ApiWrapper.Services;

public class DanbooruWrapper(IPostsService postsService, ITagsService tagsService) : IDanbooruWrapper
{
    public IPostsService Posts { get; init; } = postsService;
    public ITagsService Tags { get; init; } = tagsService;
}
