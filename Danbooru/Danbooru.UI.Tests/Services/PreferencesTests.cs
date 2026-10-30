using Danbooru.UI.Models;
using Danbooru.UI.Services;
using FluentAssertions;
using Microsoft.JSInterop;
using Moq;

namespace Danbooru.UI.Tests.Services;

public class PreferencesTests
{
    private readonly Preferences _preferences = new(new ImejiJs(Mock.Of<IJSRuntime>()));

    [Fact]
    public void Defaults_ShouldBeGeneralAndSensitive()
    {
        _preferences.Load(null);

        _preferences.Ratings.Should().Be(Rating.General | Rating.Sensitive);
        _preferences.RowHeight.Should().Be(240);
    }

    [Fact]
    public void Load_ShouldReadWhatSerializeWrote()
    {
        _preferences.Load("r=qe;s=4");

        _preferences.Ratings.Should().Be(Rating.Questionable | Rating.Explicit);
        _preferences.SizeIndex.Should().Be(4);
        _preferences.Serialize().Should().Be("r=qe;s=4");
    }

    [Fact]
    public void Load_ShouldIgnoreBrokenValues()
    {
        _preferences.Load("r=xyz;s=99;garbage");

        _preferences.Ratings.Should().Be(Rating.General | Rating.Sensitive);
        _preferences.SizeIndex.Should().Be(Preferences.RowHeights.Length - 1);
    }

    [Fact]
    public async Task ToggleRating_ShouldNotTurnOffTheLastRating()
    {
        _preferences.Load("r=g");

        await _preferences.ToggleRatingAsync(Rating.General);

        _preferences.Ratings.Should().Be(Rating.General);
    }
}
