using System.Xml.Serialization;
using Social.Models.Extensions;

namespace Social.Models.YouTube;

public record YouTubeVideo
{
    [XmlElement("id", Namespace = YouTubeExtensions.AtomNamespace)]
    public string? Id { get; set; }

    /// <summary>
    /// Pure Video Id cleaned from 'yt:' prefixes
    /// </summary>
    [XmlElement("videoId", Namespace = YouTubeExtensions.YtNamespace)]
    public string? VideoId { get; set; }

    [XmlElement("title", Namespace = YouTubeExtensions.AtomNamespace)]
    public string? Title { get; set; }

    [XmlElement("published", Namespace = YouTubeExtensions.AtomNamespace)]
    public DateTime? PublishedAt { get; set; }

    [XmlElement("link", Namespace = YouTubeExtensions.AtomNamespace)]
    public List<YouTubeLink>? Links { get; set; }
}
