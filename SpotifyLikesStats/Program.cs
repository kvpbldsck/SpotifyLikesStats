using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spotify;
using Spotify.Services;
using Stats;

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
            .AddSpotify(config)
            .AddStats();

        var provider = services.BuildServiceProvider();

        Task.WaitAll(DoStuff(provider));
    }

    private static async Task DoStuff(ServiceProvider services)
    {
        var spotifyService = services.GetRequiredService<ISpotifyService>();
        var statsService = services.GetRequiredService<IStatsService>();

        var tracks = await spotifyService.GetLikedTracksAsync(2);
        var stats = statsService.GetStats(tracks);

        Console.WriteLine(tracks.Count);
        Console.WriteLine("Top artists:");
        foreach ((int index, string item) in stats.TopArtists.Index())
        {
            Console.WriteLine($"{index + 1}. {item}");
        }
        Console.WriteLine("Top albums:");
        foreach ((int index, string item) in stats.TopAlbums.Index())
        {
            Console.WriteLine($"{index + 1}. {item}");
        }
    }
}
