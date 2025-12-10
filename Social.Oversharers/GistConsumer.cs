using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Social.Models;
using Social.Models.Enums;
using Social.Models.Gist;
using Social.Oversharers.Abstractions;

namespace Social.Oversharers;

public class GistConsumer : IGistConsumer
{
    private readonly SocialGistOptions _socialGistOptions;
    private readonly IHttpClientFactory _httpClientFactory;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
    };

    public GistConsumer(
        SocialGistOptions socialGistOptions,
        IHttpClientFactory httpClientFactory)
    {
        ArgumentNullException.ThrowIfNull(socialGistOptions);
        ArgumentNullException.ThrowIfNull(httpClientFactory);

        _socialGistOptions = socialGistOptions;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<LastState> LoadPreviousStateAsync(SocialMedia socialMedia)
    {
        Console.WriteLine("Retrieving state...");

        var gistExistForSocial = _socialGistOptions.GistPerSocial.TryGetValue(socialMedia, out var options);
        if (!gistExistForSocial || options is null)
        {
            Console.WriteLine($"Gist not found for social: {socialMedia}...");

            return new();
        }

        var requestUri = Endpoints.GithubGistApiUri.Replace("{gistId}", options.GistId);
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Authorization = new("Bearer", options.GistToken);

        using var httpClient = _httpClientFactory.CreateClient();
        using var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var gistResponse = JsonSerializer.Deserialize<GistResponse>(json, _jsonOptions);

        if (gistResponse?.Files is null
            || gistResponse.Files.Count == 0
            || !gistResponse.Files.TryGetValue(options.GistStateFileName, out var stateFile)
            || string.IsNullOrWhiteSpace(stateFile.Content))
        {
            return new();
        }

        try
        {
            var lastState = JsonSerializer.Deserialize<LastState>(stateFile.Content, _jsonOptions);
            if (lastState is null)
            {
                return new();
            }
            
            return lastState;
        }
        catch (JsonException ex)
        {
            await Console.Error.WriteLineAsync($"Failed to deserialize state from Gist with message {ex.Message}.");
        }

        return new();
    }

    public async Task SaveCurrentStateAsync(LastState state, SocialMedia socialMedia)
    {
        Console.WriteLine("Saving state...");

        var gistExistForSocial = _socialGistOptions.GistPerSocial.TryGetValue(socialMedia, out var options);
        if (!gistExistForSocial || options is null)
        {
            Console.WriteLine($"Gist not found for social: {socialMedia}...");

            return;
        }

        var stateJson = JsonSerializer.Serialize(state, _jsonOptions);

        var payload = new GistResponse
        {
            Files = new()
            {
                [options.GistStateFileName] = new()
                {
                    Content = stateJson
                }
            }
        };

        var json = JsonSerializer.Serialize(payload, _jsonOptions);

        var requestUri = Endpoints.GithubGistApiUri.Replace("{gistId}", options.GistId);
        using var request = new HttpRequestMessage(HttpMethod.Patch, requestUri)
        {
            Content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json)
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", options.GistToken);

        using var httpClient = _httpClientFactory.CreateClient();
        using var response = await httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        Console.WriteLine("State saved successfully.");
    }
}
