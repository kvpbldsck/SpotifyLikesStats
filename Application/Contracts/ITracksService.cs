using Stats.Models;

namespace Application.Contracts;

public interface ITracksService
{
    Task<IReadOnlyCollection<TrackInfoDto>> GetLikedTracksAsync(
        int pagesToFetch,
        Action<int> onTracksAmountFetched,
        Action<int> onPageFetched);
}