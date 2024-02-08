using Danbooru.ApiWrapper.Models;

namespace Danbooru.UI.Interfaces;

public interface IDoomScrollService
{
    public event EventHandler<EventArgs>? TagContainerToggled;

    public bool DisplayTagContainer { get; }
    public Post? PostOfInterest { get; }

    public void ToggleTagDisplay(Post? post);
}
