using Social.Models.Enums;
using Social.Models.Gist;

namespace Social.Oversharers.Abstractions;

public interface IGistConsumer
{
    Task<TState> LoadPreviousState<TState>(
        GistOptions options,
        string userAgent = "Social.OverSharers")
            where TState: class, new();

    Task<TState> LoadPreviousState<TState>(
        SocialGistOptions socialGistOptions,
        SocialMedia socialMedia,
        string userAgent = "Social.OverSharers")
            where TState: class, new();

    Task SaveCurrentState<TState>(
        TState state,
        GistOptions options,
        string userAgent = "Social.OverSharers")
            where TState : class, new();

    Task SaveCurrentState<TState>(
        TState state,
        SocialGistOptions socialGistOptions,
        SocialMedia socialMedia,
        string userAgent = "Social.OverSharers")
            where TState : class, new();
}
