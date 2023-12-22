using System.Text.Json.Serialization;

namespace Danbooru.ApiWrapper.Models;

public class TagAutocomplete
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = String.Empty;

    [JsonPropertyName("label")]
    public string Label { get; set; } = String.Empty;

    [JsonPropertyName("value")]
    public string Value { get; set; } = String.Empty;

    [JsonPropertyName("category")]
    public int Category { get; set; }

    [JsonPropertyName("post_count")]
    public int PostCount { get; set; }
}
