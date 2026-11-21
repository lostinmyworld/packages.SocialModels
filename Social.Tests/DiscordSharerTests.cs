using System.Net;
using Moq;
using Social.Models.Discord;
using Social.Oversharers;

namespace Social.Tests;

public class DiscordSharerTests
{
    private const string WebHook = "https://discord.example/webhook";

    [Fact]
    public async Task SendToDiscord_Success_PostsJsonToWebHook()
    {
        var handler = new FakeHttpMessageHandler(HttpStatusCode.NoContent);
        var sharer = CreateSharer(handler);

        await sharer.SendToDiscord(new DiscordRequest { Content = "hi" }, WebHook);

        Assert.Equal(HttpMethod.Post, handler.LastRequest?.Method);
        Assert.Equal(WebHook, handler.LastRequest?.RequestUri?.OriginalString);
        Assert.Equal("""{"content":"hi","embeds":[]}""", handler.LastRequestBody);
    }

    [Fact]
    public async Task SendToDiscord_ErrorStatus_Throws()
    {
        var handler = new FakeHttpMessageHandler(HttpStatusCode.BadRequest);
        var sharer = CreateSharer(handler);

        await Assert.ThrowsAsync<HttpRequestException>(
            () => sharer.SendToDiscord(new DiscordRequest { Content = "hi" }, WebHook));
    }

    private static DiscordSharer CreateSharer(FakeHttpMessageHandler handler)
    {
        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory
            .Setup(factory => factory.CreateClient(It.IsAny<string>()))
            .Returns(() => new HttpClient(handler, disposeHandler: false));

        return new DiscordSharer(httpClientFactory.Object);
    }
}
