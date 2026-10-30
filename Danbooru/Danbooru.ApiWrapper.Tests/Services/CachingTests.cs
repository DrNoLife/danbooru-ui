using Danbooru.ApiWrapper.Services;
using FluentAssertions;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;

namespace Danbooru.ApiWrapper.Tests.Services;

public class CachingTests
{
    private readonly Mock<HttpMessageHandler> _handler = new();
    private readonly PostsService _postsService;

    public CachingTests()
    {
        _handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("""{ "id": 7, "file_ext": "png" }""") });

        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(() => new HttpClient(_handler.Object, disposeHandler: false) { BaseAddress = new Uri("https://danbooru.donmai.us") });

        var cache = new ServiceCollection().AddHybridCache().Services.BuildServiceProvider().GetRequiredService<HybridCache>();
        _postsService = new PostsService(Mock.Of<ILogger<PostsService>>(), factory.Object, cache);
    }

    [Fact]
    public async Task GetPostByIdAsync_ShouldAnswerRepeatsFromTheCache()
    {
        var first = await _postsService.GetPostByIdAsync(7);
        var second = await _postsService.GetPostByIdAsync(7);

        first.Id.Should().Be(7);
        second.Id.Should().Be(7);
        _handler.Protected().Verify("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetPostByIdAsync_ShouldNotShareCacheEntriesBetweenPosts()
    {
        await _postsService.GetPostByIdAsync(7);
        await _postsService.GetPostByIdAsync(8);

        _handler.Protected().Verify("SendAsync", Times.Exactly(2), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
    }
}
