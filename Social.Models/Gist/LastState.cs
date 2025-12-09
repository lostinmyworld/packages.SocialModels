using System.Text.Json.Serialization;

namespace Social.Models.Gist;

public class LastState
{
    [JsonPropertyName("lastPostId")]
    public string? LastPostId { get; set; }
}