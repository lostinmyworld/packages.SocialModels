using Social.Models.YouTube;

namespace Social.Overthinkers.Extensions;

public static class YouTubeExtensions
{
    public static bool HasVideoId(this YouTubeVideo? youTubeVideo, string videoId)
    {
        if (youTubeVideo is null
            || string.IsNullOrWhiteSpace(youTubeVideo.VideoId)
            || string.IsNullOrWhiteSpace(videoId))
        {
            return false;
        }

        return youTubeVideo.VideoId
            .Equals(videoId, StringComparison.OrdinalIgnoreCase);
    }
}
