using Microsoft.Extensions.DependencyInjection;
using Social.Oversharers.Abstractions;

namespace Social.Oversharers.Extensions;

public static class BootstrappingExtensions
{
    public static IServiceCollection AddSocialOverSharers(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IEnvironmentLoader, EnvironmentLoader>();
        services.AddSingleton<IGistConsumer, GistConsumer>();
        services.AddSingleton<IYouTubeConsumer, YouTubeConsumer>();
        services.AddSingleton<IInstagramConsumer, InstagramConsumer>();
        services.AddSingleton<IDiscordSharer, DiscordSharer>();

        services.AddHttpClient<YouTubeConsumer>();
        services.AddHttpClient<InstagramConsumer>();
        services.AddHttpClient<DiscordSharer>();

        return services;
    }
}
