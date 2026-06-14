using Social.Models.YouTube;

namespace Social.Overthinkers.Abstractions;

public interface IYouTubeParser
{
    YouTubeFeed? DeserializeFeed(string xml);
    public YouTubeVideo? RetrieveOldestVideo(List<YouTubeVideo>? videos, string lastSharedVideoId);
}
