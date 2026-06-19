using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spotify;
using Spotify.Services;

namespace SpotifyLikesStats;

public class Program
{
    public static void Main(string[] args)
    {
        var services = new ServiceCollection();

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Local.json", optional: true)
            .Build();

        services
            .AddSingleton(config)
            .AddSpotify(config);

        var provider = services.BuildServiceProvider();

        Task.WaitAll(DoStuff(provider));
    }

    private static async Task DoStuff(ServiceProvider services)
    {
        var spotifyService = services.GetRequiredService<ISpotifyService>();

        var tracks = await spotifyService.GetLikedTracksAsync();

        Console.WriteLine(tracks.Count);
    }
}
