using Social.Models;
using Social.Models.YouTube;
using Social.Oversharers.Abstractions;
using Social.Overthinkers.Abstractions;

namespace Social.Oversharers;

public class YouTubeConsumer : IYouTubeConsumer
{
    private readonly IYouTubeParser _youTubeParser;
    private readonly IHttpClientFactory _httpClientFactory;

    public YouTubeConsumer(
        IYouTubeParser youTubeParser,
        IHttpClientFactory httpClientFactory)
    {
        ArgumentNullException.ThrowIfNull(youTubeParser);
        ArgumentNullException.ThrowIfNull(httpClientFactory);

        _youTubeParser = youTubeParser;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<YouTubeFeed?> RetrieveFeed(string channelId)
    {
        var url = Endpoints.YouTubeRssUrl.Replace("{channelId}", channelId);

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
