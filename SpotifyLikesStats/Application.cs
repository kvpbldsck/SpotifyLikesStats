using Presenter;
using Spotify;
using Stats;

namespace SpotifyLikesStats;

public sealed class Application(ISpotifyService spotifyService, IStatsService statsService, IViewService viewService)
{
    public async Task RunAsync()
    {
        var tracks = await spotifyService.GetLikedTracksAsync(1);
        var stats = statsService.GetStats(tracks);
        viewService.ShowStats(stats);
    }
}