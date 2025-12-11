using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Social.Models;
using Social.Models.Instagram;
using Social.Oversharers.Abstractions;
using Social.Oversharers.Extensions;
using Social.Overthinkers.Abstractions;

namespace Social.Oversharers;

public class InstagramConsumer : IInstagramConsumer
{
    private readonly IInstagramParser _instagramParser;
    private readonly IHttpClientFactory _httpClientFactory;

    public InstagramConsumer(IInstagramParser instagramParser, IHttpClientFactory httpClientFactory)
    {
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        _instagramParser = instagramParser;

        _instagramParser = instagramParser;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<List<InstagramPost>> RetrievePostsAsync(InstagramRequest request)
    {
        Console.WriteLine("Retrieving Instagram Posts...");

        if (request is null)
        {
            await Console.Error.WriteLineAsync($"Request is null in {nameof(InstagramConsumer)}-{nameof(RetrievePostsAsync)}");

            return [];
        }

        var queryParams = new Dictionary<string, string>
        {
            ["fields"] = request.InstagramFieldsToRetrieve,
            ["limit"] = request.HowManyPostsToFetch.ToString(),
            ["access_token"] = request.Token,
        };

        using var httpClient = _httpClientFactory.CreateClient(nameof(InstagramConsumer));
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(request.App);

        var uriToCall = QueryHelpers.AddQueryString(Endpoints.InstagramGraphApiUri, queryParams!);

        var response = await httpClient.GetStringAsync(uriToCall);
        var deserializedResponse = JsonSerializer.Deserialize<InstagramPostsResponse>(response, JsonExtensions.Options);

        var instagramPosts = deserializedResponse?.Data ?? [];

        foreach (var post in instagramPosts)
        {
            post.TimeOffset = _instagramParser.ParseTimestamp(post.Timestamp);
        }

        return instagramPosts.OrderByDescending(post => post.TimeOffset)
           .ToList();
    }
}
