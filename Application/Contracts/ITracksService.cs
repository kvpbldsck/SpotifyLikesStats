using Stats.Models;

namespace Application.Contracts;

public interface ITracksService
{
    Task<IReadOnlyCollection<TrackInfoDto>> GetLikedTracksAsync(IProgressTracker progressTracker, int tracksToFetch = int.MaxValue);
}