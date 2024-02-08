using Danbooru.ApiWrapper.Models;
using Danbooru.UI.Interfaces;

namespace Danbooru.UI.Services;

public class DoomScrollService : IDoomScrollService
{
    public event EventHandler<EventArgs>? TagContainerToggled;

    public bool DisplayTagContainer { get; private set; }

    public Post? PostOfInterest { get; private set; }

    public void ToggleTagDisplay(Post? post)
    {
        if(post is null || PostOfInterest?.Id == post.Id)
        {
            PostOfInterest = null;
            DisplayTagContainer = false;
        }
        else
        {
            PostOfInterest = post;
            DisplayTagContainer = true;
        }

        TagContainerToggled?.Invoke(this, EventArgs.Empty);
    }

}
