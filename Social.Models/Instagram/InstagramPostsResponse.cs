using System.Text.Json.Serialization;

namespace Social.Models.Instagram;

public class InstagramPostsResponse
{
    [JsonPropertyName("data")]
    public List<InstagramPost> Data { get; set; } = [];
}
