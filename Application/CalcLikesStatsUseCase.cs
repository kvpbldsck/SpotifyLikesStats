using Application.Contracts;
using Stats;

namespace Application;

public sealed class CalcLikesStatsUseCase(ITracksService tracksService, IStatsService statsService, IViewService viewService)
{
    public async Task RunAsync()
    {
        var tracks = await viewService.ShowProgressBarAsync(
            "Fetch Spotify likes",
            async progressTracker => await tracksService.GetLikedTracksAsync(progressTracker, 500));

        var stats = statsService.GetStats(tracks);

        viewService.ShowStats(stats);
    }
}