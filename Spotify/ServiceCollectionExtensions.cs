using Application.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spotify.Models;
using Spotify.Services;

namespace Spotify;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddSpotify(IConfigurationRoot config)
        {
            services.AddSingleton<SpotifyClient>();
            services.AddSingleton<ITracksService, SpotifyService>();
            services.AddSingleton<LocalHttpServer>();
            services.AddSingleton(config.GetRequiredSection(Settings.SectionName).Get<Settings>()!);

            return services;
        }
    }
}
