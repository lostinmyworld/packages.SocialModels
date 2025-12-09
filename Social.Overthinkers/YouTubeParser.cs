using System.Xml.Serialization;
using Social.Models.YouTube;

namespace Social.Overthinkers;

public static class YouTubeParser
{
    public static YouTubeFeed? DeserializeFeed(string xml)
    {
        var serializer = new XmlSerializer(typeof(YouTubeFeed));
        using var reader = new StringReader(xml);

        return (YouTubeFeed?)serializer.Deserialize(reader);
    }
}
