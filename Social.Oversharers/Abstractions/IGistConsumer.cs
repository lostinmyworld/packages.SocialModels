using Social.Models.Enums;
using Social.Models.Gist;

namespace Social.Oversharers.Abstractions;

public interface IGistConsumer
{
    Task<LastState> LoadPreviousStateAsync(SocialMedia socialMedia);

    Task SaveCurrentStateAsync(LastState state, SocialMedia socialMedia);
}
