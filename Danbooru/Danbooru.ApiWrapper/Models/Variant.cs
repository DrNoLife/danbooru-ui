using System.Text.Json.Serialization;

namespace Danbooru.ApiWrapper.Models;

public class Variant
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = String.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = String.Empty;

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("file_ext")]
    public string FileExt { get; set; } = String.Empty;
}
