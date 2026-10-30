using FluentAssertions;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Danbooru.UI.E2ETests;

public class ImejiTests(AppFixture app) : IClassFixture<AppFixture>
{
    [Fact]
    public async Task Grid_ShouldLoadMorePagesWhileScrolling()
    {
        var page = await app.OpenAsync("/");
        await Expect(page.Locator(".tile")).ToHaveCountAsync(50);

        await page.EvaluateAsync("window.scrollTo(0, document.body.scrollHeight)");

        await Expect(page.Locator("[data-page='2']")).ToBeAttachedAsync();
        (await page.Locator(".tile").CountAsync()).Should().BeGreaterThanOrEqualTo(100);
    }

    [Fact]
    public async Task Palette_ShouldAddTheHighlightedTagWithTabAndSearchWithEnter()
    {
        var page = await app.OpenAsync("/");

        await page.Keyboard.PressAsync("Control+k");
        await page.Locator("#palette-input").PressSequentiallyAsync("hats");
        await Expect(page.Locator(".palette-option[aria-selected='true']")).ToContainTextAsync("hatsune_miku");
        await page.Keyboard.PressAsync("Tab");
        await Expect(page.Locator(".palette .token")).ToHaveTextAsync("hatsune_miku×");
        await page.Keyboard.PressAsync("Enter");

        await Expect(page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex("tags=hatsune_miku$"));
        await Expect(page.Locator(".browse-title h1")).ToHaveTextAsync("hatsune_miku");
    }

    [Fact]
    public async Task Palette_ShouldNotLetAHoveredRecentSearchReplaceTheTags()
    {
        var page = await app.OpenAsync("/?tags=smile");
        await page.EvaluateAsync("localStorage.setItem('imeji-recent', JSON.stringify(['long_hair']))");
        await page.ReloadAsync();
        await AppFixture.WaitUntilInteractiveAsync(page);

        await page.Keyboard.PressAsync("Control+k");
        await page.Locator(".palette-option", new PageLocatorOptions { HasText = "long_hair" }).HoverAsync();
        await page.Keyboard.PressAsync("Enter");

        await Expect(page.Locator(".palette")).ToHaveCountAsync(0);
        await Expect(page.Locator(".browse-title h1")).ToHaveTextAsync("smile");
    }

    [Fact]
    public async Task Viewer_ShouldStepWithArrowKeysAndReturnWithEscape()
    {
        var page = await app.OpenAsync("/");
        await page.Locator(".tile").Nth(10).ClickAsync();
        await Expect(page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex($"/post/{FakeDanbooru.FirstId - 10}$"));
        await Expect(page.Locator(".viewer-back")).ToContainTextAsync("11 /");

        await page.Keyboard.PressAsync("ArrowRight");
        await Expect(page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex($"/post/{FakeDanbooru.FirstId - 11}$"));

        await page.Keyboard.PressAsync("Escape");
        await Expect(page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex("/$"));
        await Expect(page.Locator($"#post-{FakeDanbooru.FirstId - 11}")).ToBeInViewportAsync();
    }

    [Fact]
    public async Task BackButton_ShouldReturnToTheSameScrollPosition()
    {
        var page = await app.OpenAsync("/");
        await page.EvaluateAsync("window.scrollTo(0, document.body.scrollHeight)");
        await Expect(page.Locator("[data-page='2']")).ToBeAttachedAsync();
        await page.EvaluateAsync("window.scrollTo(0, 1500)");
        var before = await page.EvaluateAsync<double>("window.scrollY");

        // Click a tile that is already on screen; Playwright's own click would scroll first and move the position.
        await page.EvaluateAsync("[...document.querySelectorAll('.tile')].find(t => t.getBoundingClientRect().top > 100).click()");
        await Expect(page.Locator(".viewer")).ToBeVisibleAsync();

        await page.GoBackAsync();
        await Expect(page.Locator(".browse-grid")).ToBeVisibleAsync();
        await page.WaitForFunctionAsync($"() => Math.abs(window.scrollY - {before}) < 2");
        (await page.Locator(".tile").CountAsync()).Should().BeGreaterThanOrEqualTo(100, "the loaded pages are kept");
    }

    [Fact]
    public async Task RatingMenu_ShouldCloseOnOutsideClickWithoutOpeningAPost()
    {
        var page = await app.OpenAsync("/");
        await page.Locator(".app-header .rating-trigger").Last.ClickAsync();
        await Expect(page.Locator(".rating-menu")).ToBeVisibleAsync();

        await page.Mouse.ClickAsync(640, 500);

        await Expect(page.Locator(".rating-menu")).ToHaveCountAsync(0);
        await Expect(page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex("/$"));
    }

    [Fact]
    public async Task RatingMenu_ShouldCloseOnEscape()
    {
        var page = await app.OpenAsync("/");
        await page.Locator(".app-header .rating-trigger").Last.ClickAsync();
        await Expect(page.Locator(".rating-menu")).ToBeVisibleAsync();

        await page.Keyboard.PressAsync("Escape");

        await Expect(page.Locator(".rating-menu")).ToHaveCountAsync(0);
    }

    [Fact]
    public async Task Phone_ShouldSearchWithTheSearchButton()
    {
        var page = await app.OpenAsync("/", width: 390, height: 844);
        await page.Locator(".bottom-nav button").ClickAsync();
        await page.Locator("#palette-input").PressSequentiallyAsync("hatsune_miku");

        await page.Locator(".palette-footer").GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Search" }).ClickAsync();

        await Expect(page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex("tags=hatsune_miku$"));
    }

    [Fact]
    public async Task Viewer_ShouldCopyTheAiPromptWhenOpenedDirectly()
    {
        // Opened from a link, so there are no neighbouring posts to preload.
        var page = await app.OpenAsync($"/post/{FakeDanbooru.FirstId}");

        await page.Locator(".aside-actions").GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Copy tags as an AI image prompt" }).ClickAsync();

        await Expect(page.Locator(".toast")).ToHaveTextAsync("Prompt copied");
        var clipboard = await page.EvaluateAsync<string>("navigator.clipboard.readText()");
        clipboard.Should().StartWith("1girl, hatsune miku, test artist, long hair, smile");
        await Expect(page.Locator("#blazor-error-ui")).ToBeHiddenAsync();
    }

    [Fact]
    public async Task Feed_ShouldStepWithJ()
    {
        var page = await app.OpenAsync("/feed");
        await Expect(page.Locator(".feed-slide")).ToHaveCountAsync(20);

        await page.Keyboard.PressAsync("j");

        await page.WaitForFunctionAsync("() => window.scrollY >= window.innerHeight - 2");
    }
}
