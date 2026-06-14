namespace Social.Models;

public static class Endpoints
{
    public const string InstagramGraphApiUri = "https://graph.instagram.com/me/media";

    public const string GithubGistApiUri = "https://api.github.com/gists/{gistId}";

    public const string YouTubeRssUrl = "https://www.youtube.com/feeds/videos.xml?channel_id={channelId}";
    public const string YouTubeApiUrl = "https://www.googleapis.com/youtube/v3/channels?part=contentDetails&id={channelId}&key={_apiKey}";

    public const string TwitterV2QueryUrl = "https://api.twitter.com/2/tweets";
}
