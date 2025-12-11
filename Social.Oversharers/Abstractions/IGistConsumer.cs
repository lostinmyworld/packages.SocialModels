using Social.Models.Enums;
using Social.Models.Gist;

namespace Social.Oversharers.Abstractions;

public interface IGistConsumer
{
    Task<LastState> LoadPreviousState(SocialMedia socialMedia);

    Task SaveCurrentState(LastState state, SocialMedia socialMedia);
}
