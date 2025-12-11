using Social.Models.Enums;
using Social.Models.Gist;

namespace Social.Oversharers.Abstractions;

public interface IGistConsumer
{
    Task<LastState> LoadPreviousState(GistOptions options);
    Task<LastState> LoadPreviousState(
        SocialGistOptions socialGistOptions,
        SocialMedia socialMedia);

    Task SaveCurrentState(LastState state, GistOptions options);
    Task SaveCurrentState(
        LastState state,
        SocialGistOptions socialGistOptions,
        SocialMedia socialMedia);
}
