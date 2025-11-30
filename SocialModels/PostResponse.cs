using System.Text.Json.Serialization;

namespace SocialModels;

public class PostResponse
{
    [JsonPropertyName("data")]
    public List<Post> Data { get; set; } = [];
}
