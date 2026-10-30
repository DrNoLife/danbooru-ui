using Microsoft.JSInterop;

namespace Danbooru.UI.Services;

public static class JsHandleExtensions
{
    /// <summary>
    /// Disposes a handle returned by one of imeji.js's register/attach functions: runs its dispose() in the browser, then
    /// releases the reference. Safe to call with null or after the circuit has gone.
    /// </summary>
    public static async Task DisposeHandleAsync(this IJSObjectReference? handle)
    {
        if (handle is null)
        {
            return;
        }

        try
        {
            await handle.InvokeVoidAsync("dispose");
            await handle.DisposeAsync();
        }
        catch (JSDisconnectedException)
        {
            // The circuit is gone, so there is nothing left to clean up in the browser.
        }
    }
}
