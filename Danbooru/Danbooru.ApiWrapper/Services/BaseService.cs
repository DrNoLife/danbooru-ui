using Danbooru.ApiWrapper.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Danbooru.ApiWrapper.Services;

public abstract class BaseService(ILogger<IBaseService> logger, IHttpClientFactory httpClientFactory) : IBaseService
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
}
