using System.Text.Json;
using Octokit;
using Social.Models.Enums;
using Social.Models.Gist;
using Social.Oversharers.Abstractions;

namespace Social.Oversharers;

public class GistConsumer : IGistConsumer
{
    private const string DefaultUserAgent = "Social.OverSharers";

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
    };

    public Task<TState> LoadPreviousState<TState>(
        GistOptions options,
        string userAgent = DefaultUserAgent)
            where TState : class, new()
    {
        return LoadStateInternal<TState>(options, userAgent);
    }

    public async Task<TState> LoadPreviousState<TState>(
        SocialGistOptions socialGistOptions,
        SocialMedia socialMedia,
        string userAgent = DefaultUserAgent)
            where TState : class, new()
    {
        if (!socialGistOptions.GistPerSocial.TryGetValue(socialMedia, out var options)
            || options is null)
        {
            Console.WriteLine($"Gist not found for social: {socialMedia}.");
            return new();
        }

        return await LoadStateInternal<TState>(options, userAgent);
    }

    public Task SaveCurrentState<TState>(
        TState state,
        GistOptions options,
        string userAgent = DefaultUserAgent)
            where TState : class, new()
    {
        return SaveStateInternal(state, options, userAgent);
    }

    public Task SaveCurrentState<TState>(
        TState state,
        SocialGistOptions socialGistOptions,
        SocialMedia socialMedia,
        string userAgent = DefaultUserAgent)
            where TState : class, new()
    {
        if (!socialGistOptions.GistPerSocial.TryGetValue(socialMedia, out var options)
            || options is null)
        {
            Console.WriteLine($"Gist not found for social: {socialMedia}...");
            return Task.CompletedTask;
        }

        return SaveStateInternal(state, options, userAgent);
    }

    #region private helpers
    private static async Task<TState> LoadStateInternal<TState>(GistOptions options, string userAgent)
        where TState : class, new()
    {
        Console.WriteLine("Retrieving state...");
        try
        {
            var client = GetGitHubClient(options.GistToken, userAgent);
            var gist = await client.Gist.Get(options.GistId);

            if (gist.Files is not null
                && gist.Files.TryGetValue(options.GistStateFileName, out var stateFile)
                && !string.IsNullOrWhiteSpace(stateFile?.Content))
            {
                return JsonSerializer.Deserialize<TState>(stateFile.Content, _jsonOptions)
                    ?? new();
            }
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync($"Failed to fetch/deserialize state from Gist: {ex.Message}");
        }

        return new();
    }

    private static async Task SaveStateInternal<TState>(
        TState state,
        GistOptions options,
        string userAgent)
            where TState : class, new()
    {
        Console.WriteLine("Saving state...");

        var stateJson = JsonSerializer.Serialize(state, _jsonOptions);
        var gistFileUpdate = new GistFileUpdate
        {
            Content = stateJson,
        };
        var gistUpdate = new GistUpdate();

        gistUpdate.Files.Add(options.GistStateFileName, gistFileUpdate);

        var client = GetGitHubClient(options.GistToken, userAgent);
        await client.Gist.Edit(options.GistId, gistUpdate);

        Console.WriteLine("State saved successfully.");
    }

    private static GitHubClient GetGitHubClient(string token, string userAgent)
    {
        return new(new ProductHeaderValue(userAgent))
        {
            Credentials = new Credentials(token),
        };
    }
    #endregion
}
