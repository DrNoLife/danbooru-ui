using Danbooru.ApiWrapper.Interfaces;
using Danbooru.ApiWrapper.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Danbooru.ApiWrapper.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddDanbooruWrapper(this IServiceCollection services)
    {
        services.AddHttpClient("danbooru", options =>
        {
            options.BaseAddress = new Uri("https://danbooru.donmai.us");
            options.DefaultRequestHeaders.Add("User-Agent", "DanbooruWrapper/0.0.1 Testing");
        });

        services.AddSingleton<IPostsService, PostsService>();
        services.AddSingleton<ITagsService, TagsService>();
        services.AddSingleton<IDanbooruWrapper, DanbooruWrapper>();
    }
}
