using Application.Contracts;
using Application.Models;
using CSharpFunctionalExtensions;
using Stats;
using Stats.Models;

namespace Application;

public sealed class CalcLikesStatsUseCase(ITracksService tracksService, IStatsService statsService, IViewService viewService)
{
    public async Task RunAsync()
    {
        await viewService.ShowProgressBarAsync("Fetch Spotify likes", GetLikes)
            .Bind(t => Result.Success<StatsDto, Error>(statsService.GetStats(t)))
            .Bind(viewService.ShowStats)
            .TapError(viewService.ShowError);

        return;

        async Task<Result<IReadOnlyCollection<TrackInfoDto>, Error>> GetLikes(IProgressTracker progressTracker)
            => await tracksService.GetLikedTracksAsync(progressTracker);
    }
}