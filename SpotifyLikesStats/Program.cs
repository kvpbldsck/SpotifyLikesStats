using Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Presenter;
using Spotify;
using Stats;

namespace SpotifyLikesStats;

public static class Program
{
    public static void Main()
    {
        PrepareApplication()
            .RunAsync()
            .Wait();
    }

    private static CalcLikesStatsUseCase PrepareApplication()
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
            .AddApplication()
            .BuildServiceProvider();

        return provider.GetRequiredService<CalcLikesStatsUseCase>();
    }
}
