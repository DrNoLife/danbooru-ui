namespace Danbooru.UI.Models;

public class GalleryOptionsModel
{
    public string AspectRatio { get; set; } = "1";
    public string DisplayType { get; set; } = "gallery";
    public int ImageWidth { get; set; } = 250;

    public bool IsDisplayTypeCard => DisplayType.Equals("card");
}
