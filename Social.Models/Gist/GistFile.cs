using System.Text.Json.Serialization;

namespace Social.Models.Gist;

public class GistFile
{
    [JsonPropertyName("filename")]
    public string FileName { get; set; } = default!;

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}
