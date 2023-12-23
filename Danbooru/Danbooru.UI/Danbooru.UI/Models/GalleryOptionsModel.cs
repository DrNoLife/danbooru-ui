namespace Danbooru.UI.Models;

public class GalleryOptionsModel
{
    public string AspectRatio { get; set; } = "0";
    public string DisplayType { get; set; } = "gallery";
    public int ImageWidth { get; set; } = 250;
    public string Rating { get; set; } = String.Empty;

    public bool IsDisplayTypeCard => DisplayType.Equals("card");
}
