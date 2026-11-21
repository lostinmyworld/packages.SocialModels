using System.Text.Json;
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
        ArgumentNullException.ThrowIfNull(instagramParser);
        ArgumentNullException.ThrowIfNull(httpClientFactory);

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

        var uriToCall = $"{Endpoints.InstagramGraphApiUri}?{BuildQueryString(queryParams)}";

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

    private static string BuildQueryString(Dictionary<string, string> queryParams)
    {
        var encodedParams = queryParams.Select(param =>
        {
            var key = Uri.EscapeDataString(param.Key);
            var value = Uri.EscapeDataString(param.Value);

            return $"{key}={value}";
        });

        return string.Join("&", encodedParams);
    }
}
