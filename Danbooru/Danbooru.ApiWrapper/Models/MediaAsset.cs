using System.Text.Json.Serialization;

namespace Danbooru.ApiWrapper.Models;

public class MediaAsset
{
    [JsonPropertyName("Id")]
    public int Id { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [JsonPropertyName("md5")]
    public string Md5 { get; set; } = String.Empty;

    [JsonPropertyName("file_ext")]
    public string FileExt { get; set; } = String.Empty;

    [JsonPropertyName("file_size")]
    public int FileSize { get; set; }

    [JsonPropertyName("image_width")]
    public int Width { get; set; }

    [JsonPropertyName("image_height")]
    public int Height { get; set; }

    [JsonPropertyName("duration")]
    public object Duration { get; set; } = String.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = String.Empty;

    [JsonPropertyName("file_key")]
    public string FileKey { get; set; } = String.Empty;

    [JsonPropertyName("is_public")]
    public bool IsPublic { get; set; }

    [JsonPropertyName("pixel_hash")]
    public string PixelHash { get; set; } = String.Empty;

    [JsonPropertyName("variants")]
    public IEnumerable<Variant> Variants { get; set; } = new List<Variant>();
}
