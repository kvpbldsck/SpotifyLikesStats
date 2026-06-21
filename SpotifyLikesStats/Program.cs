using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Presenter;
using Spotify;
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

        var provider = services
            .AddSpotify(config)
            .AddStats()
            .AddView()
            .BuildServiceProvider();

        Task.WaitAll(DoStuff(provider));
    }

    private static async Task DoStuff(ServiceProvider services)
    {
        var spotifyService = services.GetRequiredService<ISpotifyService>();
        var statsService = services.GetRequiredService<IStatsService>();
        var viewService = services.GetRequiredService<IViewService>();

        var tracks = await spotifyService.GetLikedTracksAsync(1);
        var stats = statsService.GetStats(tracks);
        viewService.ShowStats(stats);
    }
}
