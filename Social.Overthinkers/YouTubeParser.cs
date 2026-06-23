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

    public YouTubeVideo? RetrieveOldestVideo(List<YouTubeVideo>? videos, string lastSharedVideoId)
    {
        if (videos is null || videos.Count == 0)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(lastSharedVideoId))
        {
            return videos[^1];
        }

        var lastSharedIndex = videos.FindIndex(e => e.HasVideoId(lastSharedVideoId));

        // no previous videos or not found => return oldest video
        if (lastSharedIndex < 0)
        {
            Console.WriteLine($"[WARNING] Last shared ID '{lastSharedVideoId}' not found in the current video list. Halting to prevent restart loop.");

            return null;
        }

        // no new videos, all are shared => nothing to do
        if (lastSharedIndex == 0)
        {
            return null;
        }

        return videos[lastSharedIndex - 1];
    }
}
