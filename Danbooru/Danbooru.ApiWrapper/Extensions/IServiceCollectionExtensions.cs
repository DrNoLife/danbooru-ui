using Danbooru.ApiWrapper.Interfaces;
using Danbooru.ApiWrapper.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Danbooru.ApiWrapper.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddDanbooruWrapper(this IServiceCollection services)
    {
        services.AddHttpClient(BaseService.HttpClientName, options =>
        {
            options.BaseAddress = new Uri("https://danbooru.donmai.us");
            options.DefaultRequestHeaders.UserAgent.ParseAdd("Imeji/1.0 (+https://github.com/DrNoLife/danbooru-ui)");
        })
        // Retries transient failures (5xx, 408, 429) with backoff, and adds timeouts and a circuit breaker.
        .AddStandardResilienceHandler();

        services.AddScoped<IPostsService, PostsService>();
        services.AddScoped<ITagsService, TagsService>();
        services.AddScoped<IDanbooruWrapper, DanbooruWrapper>();

        return services;
    }
}
