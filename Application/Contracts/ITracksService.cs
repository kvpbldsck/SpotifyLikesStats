using Application.Models;
using CSharpFunctionalExtensions;
using Stats.Models;

namespace Application.Contracts;

public interface ITracksService
{
    Task<Result<IReadOnlyCollection<TrackInfoDto>, Error>> GetLikedTracksAsync(IProgressTracker progressTracker, int tracksToFetch = int.MaxValue);
}