using Microsoft.JSInterop;

namespace Danbooru.UI.Services;

/// <summary>
/// Page-wide key presses, registered once by the layout. Pages subscribe to the keys they care about.
/// Keys arrive normalized: "mod+k" for Ctrl/Cmd+K, otherwise KeyboardEvent.key with letters lower case.
/// Typing in a text field only reports Escape and mod+k.
/// </summary>
public class KeyboardShortcuts
{
    public event Func<string, Task>? KeyPressed;

    [JSInvokable]
    public async Task OnKey(string key)
    {
        if (KeyPressed is null)
        {
            return;
        }

        foreach (var handler in KeyPressed.GetInvocationList().Cast<Func<string, Task>>())
        {
            await handler(key);
        }
    }
}
