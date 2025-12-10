using Social.Models;
using Social.Models.YouTube;
using Social.Overthinkers.Abstractions;

namespace Social.Oversharers;

public class YouTubeConsumer(IYouTubeParser _youTubeParser, IHttpClientFactory _httpClientFactory)
{
    public async Task<YouTubeFeed?> GetFeedAsync(string channelId)
    {
        var url = Endpoints.YouTubeRssUrl.Replace("channelId", channelId);

        try
        {
            using var httpClient = _httpClientFactory.CreateClient(nameof(YouTubeConsumer));
            var xml = await httpClient.GetStringAsync(url);

            return _youTubeParser.DeserializeFeed(xml);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not retrieve YouTube feed. ChannelId: {channelId}. Message: {ex.Message}");
        }

        return null;
    }
}
