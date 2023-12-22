using Danbooru.ApiWrapper.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Danbooru.ApiWrapper.Services;

public abstract class BaseService : IBaseService
{
    protected readonly ILogger<IBaseService> _logger;
    protected readonly IHttpClientFactory _httpClientFactory;
    protected readonly string _httpClientName = "danbooru";

    public BaseService(ILogger<IBaseService> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<T?> GetFromApiAsync<T>(string uri)
    {
        var client = _httpClientFactory.CreateClient(_httpClientName);
        var response = await client.GetAsync(uri);
        response.EnsureSuccessStatusCode();
        var contentString = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<T>(contentString);

        if (result is null)
        {
            _logger.LogWarning("Result from the API was null.");
        }

        return result;
    }
}
