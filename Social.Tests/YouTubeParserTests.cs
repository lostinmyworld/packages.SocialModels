using Social.Models.YouTube;
using Social.Overthinkers;

namespace Social.Tests;

public class YouTubeParserTests
{
    private readonly YouTubeParser _parser = new();

    private static readonly List<YouTubeVideo> _videosNewestFirst =
    [
        new() { VideoId = "newest" },
        new() { VideoId = "middle" },
        new() { VideoId = "oldest" },
    ];

    [Fact]
    public void RetrieveOldestVideo_NullVideos_ReturnsNull()
    {
        var result = _parser.RetrieveOldestVideo(null, "middle");

        Assert.Null(result);
    }

    [Fact]
    public void RetrieveOldestVideo_EmptyVideos_ReturnsNull()
    {
        var result = _parser.RetrieveOldestVideo([], "middle");

        Assert.Null(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void RetrieveOldestVideo_NoLastShared_ReturnsOldest(string? lastSharedVideoId)
    {
        var result = _parser.RetrieveOldestVideo(_videosNewestFirst, lastSharedVideoId!);

        Assert.Equal("oldest", result?.VideoId);
    }

    [Fact]
    public void RetrieveOldestVideo_LastSharedNotInList_ReturnsNull()
    {
        var result = _parser.RetrieveOldestVideo(_videosNewestFirst, "deleted");

        Assert.Null(result);
    }

    [Fact]
    public void RetrieveOldestVideo_LastSharedIsNewest_ReturnsNull()
    {
        var result = _parser.RetrieveOldestVideo(_videosNewestFirst, "newest");

        Assert.Null(result);
    }

    [Fact]
    public void RetrieveOldestVideo_LastSharedIsOldest_ReturnsNextNewer()
    {
        var result = _parser.RetrieveOldestVideo(_videosNewestFirst, "oldest");

        Assert.Equal("middle", result?.VideoId);
    }
}
