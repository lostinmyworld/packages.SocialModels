using Social.Models.YouTube;

namespace Social.Oversharers.Abstractions;

public interface IYouTubeConsumer
{
    Task<List<YouTubeVideo>> RetrieveAllVideos(
        string channelId,
        string apiKey,
        string callerAppName = "noname");

    Task<YouTubeVideo?> RetrieveOldestNotSharedVideo(
        string channelId,
        string apiKey,
        string lastSharedVideoId,
        string callerAppName = "noname");

    Task<YouTubeFeed?> RetrieveRssFeed(string channelId);
    Task<YouTubeVideo?> RetrieveOldestNotSharedVideoFromRssFeed(string channelId, string lastSharedVideoId);
}
