using System.Xml.Serialization;
using Social.Models.YouTube;
using Social.Overthinkers.Abstractions;
using Social.Overthinkers.Extensions;

namespace Social.Overthinkers;

public class YouTubeParser : IYouTubeParser
{
    public YouTubeFeed? DeserializeFeed(string xml)
    {
        var serializer = new XmlSerializer(typeof(YouTubeFeed));
        using var reader = new StringReader(xml);

        return (YouTubeFeed?)serializer.Deserialize(reader);
    }

    public YouTubeVideo? RetrieveOldestVideo(YouTubeFeed? feed, string lastSharedVideoId)
    {
        if (feed?.Entries is null || feed.Entries.Count == 0)
        {
            return null;
        }

        var lastSharedIndex = !string.IsNullOrWhiteSpace(lastSharedVideoId)
            ? feed.Entries.FindIndex(e => e.HasVideoId(lastSharedVideoId))
            : -1;

        // no previous videos or not found => return oldest video
        if (lastSharedIndex < 0)
        {
            return feed.Entries[^1];
        }

        // no new videos, all are shared => nothing to do
        if (lastSharedIndex == 0)
        {
            return null;
        }

        return feed.Entries[lastSharedIndex - 1];
    }
}
