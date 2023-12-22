namespace Danbooru.ApiWrapper.Interfaces;

public interface IDanbooruWrapper
{
    public IPostsService Posts { get; init; }
    public ITagsService Tags { get; init; }
}
