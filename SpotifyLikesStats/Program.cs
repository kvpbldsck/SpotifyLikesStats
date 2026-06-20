using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spotify;
using Spotify.Services;
using Stats;
using Stats.Models;

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

        var tracks = await spotifyService.GetLikedTracksAsync(4);
        var stats = statsService.GetStats(tracks);

        Console.WriteLine(tracks.Count);
        Console.WriteLine("Top artists:");
        foreach ((int index, RatedItem item) in stats.TopArtists.Index())
        {
            Console.WriteLine($"{index + 1}. {item.ItemName} ({item.Rate} {(item.Rate == 1 ? "track" : "tracks")} liked)");
        }
        Console.WriteLine("Top albums:");
        foreach ((int index, RatedItem item) in stats.TopAlbums.Index())
        {
            Console.WriteLine($"{index + 1}. {item.ItemName} ({item.Rate} {(item.Rate == 1 ? "track" : "tracks")} liked)");
        }
    }
}
