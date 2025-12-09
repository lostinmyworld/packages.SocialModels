using Social.Models.YouTube;
using Social.Overthinkers;

namespace Social.Oversharers;

public class YouTubeConsumer(HttpClient httpClient)
{
    private const string BaseUrl = "https://www.youtube.com/feeds/videos.xml?channel_id={channelId}";

    public async Task<YouTubeFeed?> GetFeedAsync(string channelId)
    {
        var url = BaseUrl.Replace("channelId", channelId);

        try
        {
            var xml = await httpClient.GetStringAsync(url);

            return YouTubeParser.DeserializeFeed(xml);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not retrieve YouTube feed. ChannelId: {channelId}. Message: {ex.Message}");
        }

        return null;
    }
}
