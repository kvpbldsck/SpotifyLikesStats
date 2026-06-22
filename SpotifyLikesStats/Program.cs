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

    private static Application PrepareApplication()
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
            .AddSingleton<Application>()
            .BuildServiceProvider();

        return provider.GetRequiredService<Application>();
    }
}
