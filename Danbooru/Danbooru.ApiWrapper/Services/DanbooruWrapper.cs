using Danbooru.ApiWrapper.Interfaces;

namespace Danbooru.ApiWrapper.Services;

public class DanbooruWrapper : IDanbooruWrapper
{
    public IPostsService Posts { get; init; }
    public ITagsService Tags { get; init; }

    public DanbooruWrapper(IPostsService postsService, ITagsService tagsService)
    {
        Posts = postsService;
        Tags = tagsService;
    }
}
