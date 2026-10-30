using Danbooru.ApiWrapper.Interfaces;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Danbooru.ApiWrapper.Services;

public abstract class BaseService(ILogger<IBaseService> logger, IHttpClientFactory httpClientFactory, HybridCache? cache = null) : IBaseService
{
    public const string HttpClientName = "danbooru";

    protected readonly ILogger<IBaseService> _logger = logger;

    public async Task<T?> GetFromApiAsync<T>(string uri, CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient(HttpClientName);
        using var response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var result = await JsonSerializer.DeserializeAsync<T>(contentStream, cancellationToken: cancellationToken);

        if (result is null)
        {
            _logger.LogWarning("Result from the API was null.");
        }

        return result;
    }

    /// <summary>
    /// Like <see cref="GetFromApiAsync{T}"/>, but answers repeated requests for the same uri from the cache for a while.
    /// Failed requests are not cached. Without a cache (e.g. in tests) every call goes to the API.
    /// </summary>
    protected async Task<T?> GetCachedAsync<T>(string uri, TimeSpan duration, CancellationToken cancellationToken = default)
    {
        if (cache is null)
        {
            return await GetFromApiAsync<T>(uri, cancellationToken);
        }

        var options = new HybridCacheEntryOptions { Expiration = duration, LocalCacheExpiration = duration };
        return await cache.GetOrCreateAsync(uri, async token => await GetFromApiAsync<T>(uri, token), options, cancellationToken: cancellationToken);
    }
}
