using Social.Models.Enums;
using Social.Models.Gist;

namespace Social.Oversharers.Abstractions;

public interface IGistConsumer
{
    Task<LastState> LoadPreviousState(
        GistOptions options,
        string userAgent = "Social.OverSharers");

    Task<LastState> LoadPreviousState(
        SocialGistOptions socialGistOptions,
        SocialMedia socialMedia,
        string userAgent = "Social.OverSharers");

    Task SaveCurrentState(
        LastState state,
        GistOptions options,
        string userAgent = "Social.OverSharers");

    Task SaveCurrentState(
        LastState state,
        SocialGistOptions socialGistOptions,
        SocialMedia socialMedia,
        string userAgent = "Social.OverSharers");
}
