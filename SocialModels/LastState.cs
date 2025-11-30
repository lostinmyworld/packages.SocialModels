using System.Text.Json.Serialization;

namespace SocialModels;

public class LastState
{
    [JsonPropertyName("lastPostId")]
    public string? LastPostId { get; set; }
}