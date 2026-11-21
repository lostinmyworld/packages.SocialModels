using System.Net;
using Moq;
using Social.Models.Instagram;
using Social.Oversharers;
using Social.Overthinkers;

namespace Social.Tests;

public class InstagramConsumerTests
{
    private const string PostsJson = """
        {
          "data": [
            { "id": "older", "timestamp": "2026-03-01T10:00:00+0000" },
            { "id": "newer", "timestamp": "2026-03-02T10:00:00+0000" }
          ]
        }
        """;

    [Fact]
    public async Task RetrievePostsAsync_TokenWithSpecialCharacters_BuildsEscapedQuery()
    {
        var handler = new FakeHttpMessageHandler(HttpStatusCode.OK, PostsJson);
        var consumer = CreateConsumer(handler);

        await consumer.RetrievePostsAsync(new InstagramRequest("tok en&x", "app/1.0"));

        Assert.Equal(
            "https://graph.instagram.com/me/media?fields=id%2Ccaption%2Cmedia_type%2Cmedia_url%2Cpermalink%2Ctimestamp&limit=20&access_token=tok%20en%26x",
            handler.LastRequest?.RequestUri?.OriginalString);
    }

    [Fact]
    public async Task RetrievePostsAsync_PostsInAnyOrder_ReturnsNewestFirst()
    {
        var handler = new FakeHttpMessageHandler(HttpStatusCode.OK, PostsJson);
        var consumer = CreateConsumer(handler);

        var posts = await consumer.RetrievePostsAsync(new InstagramRequest("token", "app/1.0"));

        Assert.Equal(["newer", "older"], posts.Select(post => post.Id));
    }

    [Fact]
    public async Task RetrievePostsAsync_ErrorStatus_Throws()
    {
        var handler = new FakeHttpMessageHandler(HttpStatusCode.BadRequest);
        var consumer = CreateConsumer(handler);

        await Assert.ThrowsAsync<HttpRequestException>(
            () => consumer.RetrievePostsAsync(new InstagramRequest("token", "app/1.0")));
    }

    private static InstagramConsumer CreateConsumer(FakeHttpMessageHandler handler)
    {
        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory
            .Setup(factory => factory.CreateClient(It.IsAny<string>()))
            .Returns(() => new HttpClient(handler, disposeHandler: false));

        return new InstagramConsumer(new InstagramParser(), httpClientFactory.Object);
    }
}
