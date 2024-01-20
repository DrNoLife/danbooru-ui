using Danbooru.ApiWrapper.Interfaces;
using Danbooru.ApiWrapper.Models;
using Danbooru.ApiWrapper.Models.Url;
using Danbooru.ApiWrapper.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Runtime.CompilerServices;

namespace Danbooru.ApiWrapper.Tests.Services;

public class PostsServiceTests
{
    private readonly Mock<ILogger<PostsService>> _mockLogger;
    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly IPostsService _postsService;

    private readonly string _getPostsAsyncMockResponse;
    private readonly string _getPostsByIdAsyncMockResponse;
    private readonly string _getPostsByTagAsyncMockResponse;

    public PostsServiceTests()
    {
        _mockLogger = new Mock<ILogger<PostsService>>();
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

        var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://danbooru.donmai.us")
        };

        _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

        _postsService = new PostsService(_mockLogger.Object, _mockHttpClientFactory.Object);

        // Responses.
        _getPostsAsyncMockResponse = @"
        [
            {
                ""id"": 1,
                ""created_at"": ""2024-01-20T00:00:00"",
                ""uploader_id"": 123,
                ""score"": 10,
                ""source"": ""https://example.com"",
                ""md5"": ""abcd1234"",
                ""rating"": ""safe"",
                ""image_width"": 1920,
                ""image_height"": 1080,
                ""tag_string"": ""tag1 tag2"",
                ""fav_count"": 5,
                ""file_ext"": ""jpg"",
                ""is_pending"": false,
                ""is_flagged"": false,
                ""is_deleted"": false,
                ""tag_count"": 2,
                ""updated_at"": ""2024-01-20T00:00:00"",
                ""is_banned"": false,
                ""file_url"": ""https://example.com/image.jpg"",
                ""large_file_url"": ""https://example.com/large_image.jpg"",
                ""preview_file_url"": ""https://example.com/preview_image.jpg""
            }
        ]";
        _getPostsByIdAsyncMockResponse = @"
        [
            {
                ""id"": 123,
                ""created_at"": ""2024-01-21T12:00:00"",
                ""uploader_id"": 456,
                ""score"": 100,
                ""source"": ""https://example.com/image1"",
                ""md5"": ""1a2b3c4d5e"",
                ""rating"": ""safe"",
                ""image_width"": 1920,
                ""image_height"": 1080,
                ""tag_string"": ""nature landscape"",
                ""fav_count"": 50,
                ""file_ext"": ""jpg"",
                ""is_pending"": false,
                ""is_flagged"": false,
                ""is_deleted"": false,
                ""tag_count"": 2,
                ""updated_at"": ""2024-01-21T12:30:00"",
                ""is_banned"": false,
                ""file_url"": ""https://example.com/files/image1.jpg"",
                ""large_file_url"": ""https://example.com/files/large_image1.jpg"",
                ""preview_file_url"": ""https://example.com/files/preview_image1.jpg""
            },
            {
                ""id"": 124,
                ""created_at"": ""2024-01-22T15:00:00"",
                ""uploader_id"": 789,
                ""score"": 85,
                ""source"": ""https://example.com/image2"",
                ""md5"": ""5e4d3c2b1a"",
                ""rating"": ""questionable"",
                ""image_width"": 1280,
                ""image_height"": 720,
                ""tag_string"": ""city night"",
                ""fav_count"": 35,
                ""file_ext"": ""png"",
                ""is_pending"": false,
                ""is_flagged"": false,
                ""is_deleted"": false,
                ""tag_count"": 2,
                ""updated_at"": ""2024-01-22T16:00:00"",
                ""is_banned"": false,
                ""file_url"": ""https://example.com/files/image2.png"",
                ""large_file_url"": ""https://example.com/files/large_image2.png"",
                ""preview_file_url"": ""https://example.com/files/preview_image2.png""
            }
        ]";
        _getPostsByTagAsyncMockResponse = @"
        [
            {
                ""id"": 125,
                ""created_at"": ""2024-01-23T10:00:00"",
                ""uploader_id"": 101,
                ""score"": 95,
                ""source"": ""https://example.com/image3"",
                ""md5"": ""9f8e7d6c5b"",
                ""rating"": ""explicit"",
                ""image_width"": 2048,
                ""image_height"": 1152,
                ""tag_string"": ""mountain sunrise"",
                ""fav_count"": 60,
                ""file_ext"": ""jpg"",
                ""is_pending"": false,
                ""is_flagged"": false,
                ""is_deleted"": false,
                ""tag_count"": 2,
                ""updated_at"": ""2024-01-23T11:00:00"",
                ""is_banned"": false,
                ""file_url"": ""https://example.com/files/image3.jpg"",
                ""large_file_url"": ""https://example.com/files/large_image3.jpg"",
                ""preview_file_url"": ""https://example.com/files/preview_image3.jpg""
            }
        ]";
    }

    private void SetupMockHttpMessageHandler(HttpStatusCode statusCode, string content)
    {
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(content)
            })
            .Verifiable(); 
    }

    [Fact]
    public async Task GetPostsAsync_ShouldReturnPosts_WhenApiCallIsSuccessful()
    {
        // Arrange
        SetupMockHttpMessageHandler(HttpStatusCode.OK, _getPostsAsyncMockResponse);

        // Act
        var result = await _postsService.GetPostsAsync();

        // Assert
        result.Should().BeOfType<List<Post>>();
        result.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task GetPostsAsync_ShouldReturnEmptyList_WhenApiCallFails()
    {
        // Arrange
        SetupMockHttpMessageHandler(HttpStatusCode.BadRequest, "");

        // Act
        Func<Task> act = async () => await _postsService.GetPostsAsync();

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task GetPostsAsync_WithParameters_ShouldReturnPosts()
    {
        // Arrange
        var urlParameters = new UrlParameters { Page = 2, PageItems = 10 };
        SetupMockHttpMessageHandler(HttpStatusCode.OK, _getPostsAsyncMockResponse);

        // Act
        var result = await _postsService.GetPostsAsync(urlParameters);

        // Assert
        result.Should().BeOfType<List<Post>>();
        result.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task GetPostsAsync_WithParameters_ShouldThrowException_OnApiFailure()
    {
        // Arrange
        var urlParameters = new UrlParameters { Page = 2, PageItems = 10 };
        SetupMockHttpMessageHandler(HttpStatusCode.BadRequest, "");

        // Act
        Func<Task> act = async () => await _postsService.GetPostsAsync(urlParameters);

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task GetPostsByIdAsync_ShouldReturnPosts()
    {
        // Arrange
        int postId = 123;
        int postsToGet = 5;
        SetupMockHttpMessageHandler(HttpStatusCode.OK, _getPostsByIdAsyncMockResponse);

        // Act
        var result = await _postsService.GetPostsByIdAsync(postId, postsToGet);

        // Assert
        result.Should().BeOfType<List<Post>>();
        result.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task GetPostsByIdAsync_ShouldThrowException_OnApiFailure()
    {
        // Arrange
        int postId = 123;
        int postsToGet = 5;
        SetupMockHttpMessageHandler(HttpStatusCode.BadRequest, "");

        // Act
        Func<Task> act = async () => await _postsService.GetPostsByIdAsync(postId, postsToGet);

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task GetPostsByTagAsync_ShouldReturnPosts()
    {
        // Arrange
        string tag = "exampleTag";
        SetupMockHttpMessageHandler(HttpStatusCode.OK, _getPostsByTagAsyncMockResponse);

        // Act
        var result = await _postsService.GetPostsByTagAsync(tag);

        // Assert
        result.Should().BeOfType<List<Post>>();
        result.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task GetPostsByTagAsync_ShouldThrowException_OnApiFailure()
    {
        // Arrange
        string tag = "exampleTag";
        SetupMockHttpMessageHandler(HttpStatusCode.BadRequest, "");

        // Act
        Func<Task> act = async () => await _postsService.GetPostsByTagAsync(tag);

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>();
    }

}
