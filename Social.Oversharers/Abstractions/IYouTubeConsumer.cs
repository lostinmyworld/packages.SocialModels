using Social.Models.YouTube;

namespace Social.Oversharers.Abstractions;

public interface IYouTubeConsumer
{
    Task<YouTubeFeed?> RetrieveFeed(string channelId);
}
