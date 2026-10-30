using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Danbooru.UI.Services;

/// <summary>
/// Typed access to wwwroot/js/imeji.js. The module is imported on first use, so this only works once the page is interactive.
/// </summary>
public sealed class ImejiJs(IJSRuntime js) : IAsyncDisposable
{
    private Task<IJSObjectReference>? _module;

    private Task<IJSObjectReference> Module => _module ??= js.InvokeAsync<IJSObjectReference>("import", "./js/imeji.js").AsTask();

    public async Task SetCookieAsync(string name, string value) => await (await Module).InvokeVoidAsync("setCookie", name, value);

    public async Task<string?> GetStorageAsync(string key) => await (await Module).InvokeAsync<string?>("storageGet", key);

    public async Task SetStorageAsync(string key, string value) => await (await Module).InvokeVoidAsync("storageSet", key, value);

    /// <summary>
    /// Sends page-wide key presses to <see cref="KeyboardShortcuts.OnKey"/>. Dispose the result to stop.
    /// </summary>
    public async Task<IJSObjectReference> RegisterShortcutsAsync(DotNetObjectReference<KeyboardShortcuts> target) =>
        await (await Module).InvokeAsync<IJSObjectReference>("registerShortcuts", target);

    /// <summary>
    /// Reports the page marker nearest the top of the screen through a JSInvokable SetCurrentPage(int). Dispose the result to stop.
    /// </summary>
    public async Task<IJSObjectReference> TrackPagesAsync<T>(DotNetObjectReference<T> target) where T : class =>
        await (await Module).InvokeAsync<IJSObjectReference>("trackPages", target);

    /// <summary>
    /// Handles arrow keys, Tab, Enter, Escape and Backspace in the search palette's input through a JSInvokable
    /// OnPaletteKey(string key, bool shift). Dispose the result to stop.
    /// </summary>
    public async Task<IJSObjectReference> AttachPaletteKeysAsync<T>(ElementReference input, DotNetObjectReference<T> target) where T : class =>
        await (await Module).InvokeAsync<IJSObjectReference>("attachPaletteKeys", input, target);

    /// <summary>
    /// Starts remembering the scroll position of each page when a link is followed. Dispose the result to stop.
    /// </summary>
    public async Task<IJSObjectReference> RememberScrollOnLeaveAsync() =>
        await (await Module).InvokeAsync<IJSObjectReference>("rememberScrollOnLeave");

    /// <summary>
    /// Scrolls back to where the current URL was left. False when there is no saved position.
    /// </summary>
    public async Task<bool> RestoreScrollAsync() => await (await Module).InvokeAsync<bool>("restoreScroll");

    /// <summary>
    /// Reports swipes on the element through a JSInvokable OnSwipe(string direction). Dispose the result to stop.
    /// </summary>
    public async Task<IJSObjectReference> AttachSwipeAsync<T>(ElementReference element, DotNetObjectReference<T> target) where T : class =>
        await (await Module).InvokeAsync<IJSObjectReference>("attachSwipe", element, target);

    // Cast to object: a bare string[] would be taken as the params array itself and spread into separate arguments.
    public async Task PreloadImagesAsync(IEnumerable<string> urls) => await (await Module).InvokeVoidAsync("preloadImages", (object)urls.ToArray());

    /// <summary>
    /// Plays feed videos while they are on screen. Dispose the result to stop.
    /// </summary>
    public async Task<IJSObjectReference> AutoplayVideosAsync() => await (await Module).InvokeAsync<IJSObjectReference>("autoplayVideos");

    public async Task ScrollToTopAsync() => await (await Module).InvokeVoidAsync("scrollToTop");

    /// <param name="block">"start" or "center", as in Element.scrollIntoView.</param>
    public async Task ScrollIntoViewAsync(string selector, string block = "start", bool smooth = true) =>
        await (await Module).InvokeVoidAsync("scrollIntoView", selector, block, smooth);

    public async Task SetScrollLockAsync(bool locked) => await (await Module).InvokeVoidAsync("setScrollLock", locked);

    public async Task ToggleFullscreenAsync(ElementReference element) => await (await Module).InvokeVoidAsync("toggleFullscreen", element);

    public async Task CopyTextAsync(string text) => await (await Module).InvokeVoidAsync("copyText", text);

    /// <summary>
    /// Opens the system share sheet where there is one, else copies the link. Returns "shared" or "copied".
    /// </summary>
    public async Task<string> ShareAsync(string url, string title) => await (await Module).InvokeAsync<string>("share", url, title);

    /// <summary>
    /// Scrolls the feed one slide forward (1) or back (-1).
    /// </summary>
    public async Task StepFeedAsync(int direction) => await (await Module).InvokeVoidAsync("stepFeed", direction);

    public async ValueTask DisposeAsync()
    {
        if (_module is null)
        {
            return;
        }

        try
        {
            await (await _module).DisposeAsync();
        }
        catch (JSDisconnectedException)
        {
            // The circuit is gone, so there is nothing left to clean up in the browser.
        }
    }
}
