using Microsoft.JSInterop;

namespace Danbooru.UI.Services;

/// <summary>
/// Page-wide key presses, registered once by the layout. Components subscribe to the keys they care about.
/// Keys arrive normalized: "mod+k" for Ctrl/Cmd+K, otherwise KeyboardEvent.key with letters lower case.
/// Typing in a text field only reports Escape and mod+k.
/// </summary>
public class KeyboardShortcuts
{
    /// <summary>
    /// Handlers run in subscription order (layout first, then popups and pages) and return true to stop the key there,
    /// so Escape closes an open menu before it can also leave the page.
    /// </summary>
    public event Func<string, Task<bool>>? KeyPressed;

    [JSInvokable]
    public async Task OnKey(string key)
    {
        if (KeyPressed is null)
        {
            return;
        }

        foreach (var handler in KeyPressed.GetInvocationList().Cast<Func<string, Task<bool>>>())
        {
            if (await handler(key))
            {
                return;
            }
        }
    }
}
