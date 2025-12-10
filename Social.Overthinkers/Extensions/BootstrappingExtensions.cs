using Microsoft.Extensions.DependencyInjection;
using Social.Overthinkers.Abstractions;

namespace Social.Overthinkers.Extensions;

public static class BootstrappingExtensions
{
    public static IServiceCollection AddSocialOverSharers(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IYouTubeParser, YouTubeParser>();

        return services;
    }
}
