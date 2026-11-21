using Social.Models.YouTube;
using Social.Overthinkers.Extensions;

namespace Social.Tests;

public class YouTubeExtensionsTests
{
    [Fact]
    public void HasVideoId_NullVideo_ReturnsFalse()
    {
        YouTubeVideo? video = null;

        Assert.False(video.HasVideoId("abc"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void HasVideoId_VideoWithoutId_ReturnsFalse(string? videoId)
    {
        var video = new YouTubeVideo { VideoId = videoId };

        Assert.False(video.HasVideoId("abc"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void HasVideoId_NoIdToMatch_ReturnsFalse(string? idToMatch)
    {
        var video = new YouTubeVideo { VideoId = "abc" };

        Assert.False(video.HasVideoId(idToMatch!));
    }

    [Fact]
    public void HasVideoId_SameIdDifferentCase_ReturnsTrue()
    {
        var video = new YouTubeVideo { VideoId = "abc" };

        Assert.True(video.HasVideoId("ABC"));
    }

    [Fact]
    public void HasVideoId_DifferentId_ReturnsFalse()
    {
        var video = new YouTubeVideo { VideoId = "abc" };

        Assert.False(video.HasVideoId("xyz"));
    }
}
