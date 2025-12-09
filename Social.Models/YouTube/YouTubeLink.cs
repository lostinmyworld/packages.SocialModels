using System.Xml.Serialization;

namespace Social.Models.YouTube;

public record YouTubeLink
{
    [XmlAttribute("href")]
    public string? Href { get; set; }

    [XmlAttribute("rel")]
    public string? Rel { get; set; }
}
