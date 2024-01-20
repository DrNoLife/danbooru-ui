using Danbooru.UI.Models;
using Danbooru.UI.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Moq;

namespace Danbooru.UI.Tests.Services;

public class TestNavigationManager : NavigationManager
{
    public TestNavigationManager()
    {
        Initialize("http://localhost/", "http://localhost/test");
    }

    public void SetUri(string uri)
    {
        Uri = uri;
        NotifyLocationChanged(isInterceptedLink: false);
    }

    protected override void NavigateToCore(string uri, bool forceLoad)
    {
        Uri = new Uri(new Uri(BaseUri), uri).ToString();
        NotifyLocationChanged(isInterceptedLink: false);
    }
}

public class GallerySettingsServiceTests
{
    private readonly TestNavigationManager _navigationManager;
    private readonly GallerySettingsService _gallerySettingsService;

    public GallerySettingsServiceTests()
    {
        _navigationManager = new TestNavigationManager();
        _gallerySettingsService = new GallerySettingsService(_navigationManager);
    }

    [Fact]
    public void Constructor_ShouldInitializeSettings_WhenCalled()
    {
        var options = _gallerySettingsService.GetGalleryOptions();
        options.Should().NotBeNull();
    }

    [Fact]
    public void GetGalleryOptions_ShouldReturnCurrentSettings()
    {
        // Arrange (setup if needed)

        // Act
        var options = _gallerySettingsService.GetGalleryOptions();

        // Assert
        options.Should().BeOfType<GalleryOptionsModel>();
    }

    [Fact]
    public void UpdateSetting_ShouldModifySettings()
    {
        // Arrange
        Action<GalleryOptionsModel> updateAction = (opts) => opts.AspectRatio = "1.5";

        // Act
        _gallerySettingsService.UpdateSetting(updateAction);

        // Assert
        var options = _gallerySettingsService.GetGalleryOptions();
        options.AspectRatio.Should().Be("1.5");
    }

    [Fact]
    public void GenerateQueryString_ShouldReturnCorrectQueryString()
    {
        // Arrange

        // Act
        var queryString = _gallerySettingsService.GenerateQueryString();

        // Assert
        queryString.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void NavigateWithUpdatedSettings_ShouldNavigateToNewUri()
    {
        // Arrange
        _navigationManager.SetUri("http://localhost/gallery");

        // Update the settings to reflect what you expect to be in the new URI
        _gallerySettingsService.UpdateSetting(opts =>
        {
            opts.AspectRatio = "1.5";
            opts.DisplayType = "grid";
            opts.ImageWidth = 300;
        });

        // Expected URI contains the updated settings in the query string
        string expectedUri = "http://localhost/gallery?aspect-ratio=1.5&display=grid&image-width=300";

        // Act
        _gallerySettingsService.NavigateWithUpdatedSettings();

        // Assert
        _navigationManager.Uri.Should().Be(expectedUri);
    }

    [Fact]
    public void GetSetting_ShouldReturnSpecificSetting()
    {
        // Arrange
        Func<GalleryOptionsModel, string> getAction = (opts) => opts.AspectRatio;

        // Act
        var aspectRatio = _gallerySettingsService.GetSetting(getAction);

        // Assert
        aspectRatio.Should().NotBeNullOrEmpty();
    }
}
