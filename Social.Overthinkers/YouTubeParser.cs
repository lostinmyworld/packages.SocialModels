using System.Xml.Serialization;
using Social.Models.YouTube;
using Social.Overthinkers.Abstractions;

namespace Social.Overthinkers;

public class YouTubeParser : IYouTubeParser
{
    public YouTubeFeed? DeserializeFeed(string xml)
    {
        var serializer = new XmlSerializer(typeof(YouTubeFeed));
        using var reader = new StringReader(xml);

        return (YouTubeFeed?)serializer.Deserialize(reader);
    }
}
