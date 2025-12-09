using System.Text.Json.Serialization;

namespace Social.Models.Gist;

public class GistResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = default!;

    [JsonPropertyName("files")]
    public Dictionary<string, GistFile> Files { get; set; } = [];
}
