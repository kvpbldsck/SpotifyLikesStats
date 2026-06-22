using Application.Contracts;
using Stats;

namespace Application;

public sealed class CalcLikesStatsUseCase(ITracksService spotifyService, IStatsService statsService, IViewService viewService)
{
    public async Task RunAsync()
    {
        var tracks = await viewService.ShowProgressBarAsync(
            async (setProgressBarGoal, incrementProgressBar) => await spotifyService.GetLikedTracksAsync(
                10,
                onTracksAmountFetched: value => setProgressBarGoal(value),
                onPageFetched: value => incrementProgressBar(value)));

        var stats = statsService.GetStats(tracks);

        viewService.ShowStats(stats);
    }
}