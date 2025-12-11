using Social.Models.Discord;

namespace Social.Oversharers.Abstractions;

public interface IDiscordSharer
{
    Task SendToDiscord(DiscordRequest discordRequest, string webHook);
}
