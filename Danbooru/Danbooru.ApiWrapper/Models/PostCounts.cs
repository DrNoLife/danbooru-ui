using System.Text.Json.Serialization;

namespace Danbooru.ApiWrapper.Models;

public class PostCounts
{
    [JsonPropertyName("counts")]
    public PostCountValues Counts { get; set; } = new();
}

public class PostCountValues
{
    [JsonPropertyName("posts")]
    public int? Posts { get; set; }
}
