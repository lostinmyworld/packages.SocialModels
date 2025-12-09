using System.Text.Json.Serialization;

namespace Social.Models.Discord;

public class DiscordRequest
{
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("embeds")]
    public Dictionary<string, object?>[] Embeds { get; set; } = [];
}
