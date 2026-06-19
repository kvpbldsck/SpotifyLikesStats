using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spotify.Models;
using Spotify.Services;

namespace Spotify;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSpotify(this IServiceCollection services, IConfigurationRoot config)
    {
        services.AddSingleton<SpotifyApi>();
        services.AddSingleton<ISpotifyService, SpotifyService>();
        services.AddSingleton<IHttpReceiver, HttpReceiver>();
        services.AddSingleton(config.GetSection(Settings.SectionName).Get<Settings>()!);

        return services;
    }
}
