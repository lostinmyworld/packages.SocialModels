using System.Net;
using Moq;
using Social.Models;
using Social.Models.Threads;
using Social.Oversharers;

namespace Social.Tests;

public class ThreadsSharerTests
{
    private const string Token = "threads-token";
    private const string ContainerJson = """{"id":"container-1"}""";
    private const string PublishedJson = """{"id":"post-1"}""";

    [Fact]
    public async Task SharePost_Success_ReturnsPublishedPostId()
    {
        var handler = new SequenceHttpMessageHandler((HttpStatusCode.OK, ContainerJson), (HttpStatusCode.OK, PublishedJson));
        var sharer = CreateSharer(handler);

        var response = await sharer.SharePost(new ThreadsPostRequest(Token, "hello", "https://example.com/video"));

        Assert.True(response.IsSuccess);
        Assert.Equal("post-1", response.PostId);
    }

    [Fact]
    public async Task SharePost_WithLink_CreatesTextContainerWithLinkAttachment()
    {
        var handler = new SequenceHttpMessageHandler((HttpStatusCode.OK, ContainerJson), (HttpStatusCode.OK, PublishedJson));
        var sharer = CreateSharer(handler);

        await sharer.SharePost(new ThreadsPostRequest(Token, "hello", "https://example.com/video"));

        Assert.Equal(Endpoints.ThreadsCreateContainerUri, handler.Requests[0].RequestUri?.OriginalString);
        Assert.Equal(
            "media_type=TEXT&text=hello&access_token=threads-token&link_attachment=https%3A%2F%2Fexample.com%2Fvideo",
            handler.RequestBodies[0]);
    }

    [Fact]
    public async Task SharePost_WithoutLink_OmitsLinkAttachment()
    {
        var handler = new SequenceHttpMessageHandler((HttpStatusCode.OK, ContainerJson), (HttpStatusCode.OK, PublishedJson));
        var sharer = CreateSharer(handler);

        await sharer.SharePost(new ThreadsPostRequest(Token, "hello"));

        Assert.Equal("media_type=TEXT&text=hello&access_token=threads-token", handler.RequestBodies[0]);
    }

    [Fact]
    public async Task SharePost_ContainerCreated_PublishesThatContainer()
    {
        var handler = new SequenceHttpMessageHandler((HttpStatusCode.OK, ContainerJson), (HttpStatusCode.OK, PublishedJson));
        var sharer = CreateSharer(handler);

        await sharer.SharePost(new ThreadsPostRequest(Token, "hello"));

        Assert.Equal(Endpoints.ThreadsPublishUri, handler.Requests[1].RequestUri?.OriginalString);
        Assert.Equal("creation_id=container-1&access_token=threads-token", handler.RequestBodies[1]);
    }

    [Fact]
    public async Task SharePost_ContainerErrorStatus_ReturnsApiErrorMessage()
    {
        var errorJson = """{"error":{"message":"Invalid OAuth access token."}}""";
        var handler = new SequenceHttpMessageHandler((HttpStatusCode.BadRequest, errorJson));
        var sharer = CreateSharer(handler);

        var response = await sharer.SharePost(new ThreadsPostRequest(Token, "hello"));

        Assert.False(response.IsSuccess);
        Assert.Equal("Threads API returned 400: Invalid OAuth access token.", response.ErrorMessage);
    }

    [Fact]
    public async Task SharePost_ErrorStatusWithoutJson_ReturnsUnknownError()
    {
        var handler = new SequenceHttpMessageHandler((HttpStatusCode.InternalServerError, "oops"));
        var sharer = CreateSharer(handler);

        var response = await sharer.SharePost(new ThreadsPostRequest(Token, "hello"));

        Assert.False(response.IsSuccess);
        Assert.Equal("Threads API returned 500: unknown error", response.ErrorMessage);
    }

    [Fact]
    public async Task SharePost_PublishErrorStatus_ReturnsFailure()
    {
        var errorJson = """{"error":{"message":"Media not ready."}}""";
        var handler = new SequenceHttpMessageHandler((HttpStatusCode.OK, ContainerJson), (HttpStatusCode.BadRequest, errorJson));
        var sharer = CreateSharer(handler);

        var response = await sharer.SharePost(new ThreadsPostRequest(Token, "hello"));

        Assert.False(response.IsSuccess);
        Assert.Equal("Threads API returned 400: Media not ready.", response.ErrorMessage);
    }

    [Fact]
    public async Task SharePost_ResponseWithoutId_ReturnsFailure()
    {
        var handler = new SequenceHttpMessageHandler((HttpStatusCode.OK, "{}"));
        var sharer = CreateSharer(handler);

        var response = await sharer.SharePost(new ThreadsPostRequest(Token, "hello"));

        Assert.False(response.IsSuccess);
        Assert.Equal("Threads API returned no id.", response.ErrorMessage);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task SharePost_EmptyText_ReturnsFailureWithoutCalls(string? text)
    {
        var handler = new SequenceHttpMessageHandler();
        var sharer = CreateSharer(handler);

        var response = await sharer.SharePost(new ThreadsPostRequest(Token, text!));

        Assert.False(response.IsSuccess);
        Assert.Equal("Threads text cannot be empty.", response.ErrorMessage);
        Assert.Empty(handler.Requests);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task SharePost_MissingToken_ReturnsFailureWithoutCalls(string? token)
    {
        var handler = new SequenceHttpMessageHandler();
        var sharer = CreateSharer(handler);

        var response = await sharer.SharePost(new ThreadsPostRequest(token!, "hello"));

        Assert.False(response.IsSuccess);
        Assert.Equal("Threads access token is missing.", response.ErrorMessage);
        Assert.Empty(handler.Requests);
    }

    [Fact]
    public async Task SharePost_NullRequest_ReturnsFailure()
    {
        var handler = new SequenceHttpMessageHandler();
        var sharer = CreateSharer(handler);

        var response = await sharer.SharePost(null!);

        Assert.False(response.IsSuccess);
        Assert.Equal("Threads text cannot be empty.", response.ErrorMessage);
    }

    private static ThreadsSharer CreateSharer(SequenceHttpMessageHandler handler)
    {
        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory
            .Setup(factory => factory.CreateClient(It.IsAny<string>()))
            .Returns(() => new HttpClient(handler, disposeHandler: false));

        return new ThreadsSharer(httpClientFactory.Object, publishDelaySeconds: 0);
    }
}
