using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Social.Models.Discord;
using Social.Oversharers.Abstractions;
using Social.Oversharers.Extensions;

namespace Social.Oversharers;

public class DiscordSharer : IDiscordSharer
{
    private readonly IHttpClientFactory _httpClientFactory;

    public DiscordSharer(IHttpClientFactory httpClientFactory)
    {
        ArgumentNullException.ThrowIfNull(httpClientFactory);

        _httpClientFactory = httpClientFactory;
    }

    public async Task SendToDiscord(DiscordRequest discordRequest, string webHook)
    {
        Console.WriteLine("Posting to Discord...");

        var payload = JsonSerializer.Serialize(discordRequest, JsonExtensions.Options);
        var content = new StringContent(payload, Encoding.UTF8, MediaTypeNames.Application.Json);

        using var httpClient = _httpClientFactory.CreateClient(nameof(DiscordSharer));
        using var response = await httpClient.PostAsync(webHook, content);

        response.EnsureSuccessStatusCode();

        Console.WriteLine($"Discord response: {(int)response.StatusCode} {response.StatusCode}");
    }
}
