using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
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

    public async Task<List<YouTubeVideo>> RetrieveAllVideos(
        string channelId,
        string apiKey,
        string callerAppName = "noname")
    {
        try
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = apiKey,
                ApplicationName = "NotAnotherSocialBot"
            });

            // Μετατρέπουμε το UC σε UU για να πάρουμε τη λίστα "Uploads"
            var uploadsPlaylistId = channelId.Replace("UC", "UU");

            var request = youtubeService.PlaylistItems.List("snippet,contentDetails");
            request.PlaylistId = uploadsPlaylistId;
            request.MaxResults = 50;

            var allVideos = new List<YouTubeVideo>();
            string? nextPageToken = null;

            do
            {
                request.PageToken = nextPageToken;
                var response = await request.ExecuteAsync();

                foreach (var item in response.Items)
                {
                    allVideos.Add(new YouTubeVideo
                    {
                        VideoId = item.ContentDetails.VideoId,
                        Title = item.Snippet.Title,
                        Id = item.Id,
                    });
                }

                nextPageToken = response.NextPageToken;
            }
            while (!string.IsNullOrWhiteSpace(nextPageToken));

            return allVideos;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not retrieve YouTube feed. ChannelId: {channelId}. Message: {ex.Message}");
        }

        return [];
    }

    public async Task<YouTubeVideo?> RetrieveOldestNotSharedVideo(
        string channelId,
        string apiKey,
        string lastSharedVideoId,
        string callerAppName = "noname")
    {
        var videos = await RetrieveAllVideos(channelId, apiKey, callerAppName);

        return _youTubeParser.RetrieveOldestVideo(videos, lastSharedVideoId);
    }

    public async Task<YouTubeFeed?> RetrieveRssFeed(string channelId)
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

    public async Task<YouTubeVideo?> RetrieveOldestNotSharedVideoFromRssFeed(string channelId, string lastSharedVideoId)
    {
        var feed = await RetrieveRssFeed(channelId);

        return _youTubeParser.RetrieveOldestVideo(feed?.Entries, lastSharedVideoId);
    }
}
