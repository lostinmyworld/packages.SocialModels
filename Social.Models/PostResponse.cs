using System.Text.Json.Serialization;

namespace Social.Models;

public class PostResponse
{
    [JsonPropertyName("data")]
    public List<Post> Data { get; set; } = [];
}
