using System.Net;

namespace Danbooru.UI.Models;

public static class SearchErrors
{
    /// <summary>
    /// Danbooru stops anonymous paging here.
    /// </summary>
    public const int MaxPage = 1000;

    public static string Describe(HttpRequestException exception) => exception.StatusCode switch
    {
        HttpStatusCode.UnprocessableEntity => $"Danbooru rejected the search. Without an account it allows {SearchQuery.AnonymousTagLimit} tags per search (excluded tags count) and {MaxPage} pages.",
        HttpStatusCode.TooManyRequests => "Danbooru is rate limiting us. Wait a moment and try again.",
        _ => "Could not reach Danbooru. Try again in a moment."
    };
}
