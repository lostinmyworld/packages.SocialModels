using System.Xml.Serialization;
using Social.Models.Extensions;

namespace Social.Models.YouTube;

[XmlRoot("feed", Namespace = YouTubeExtensions.AtomNamespace)]
public record YouTubeFeed
{
    [XmlElement("entry", Namespace = YouTubeExtensions.AtomNamespace)]
    public List<YouTubeVideo> Entries { get; set; } = [];
}
