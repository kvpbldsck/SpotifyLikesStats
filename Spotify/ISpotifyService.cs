using Stats.Models;

namespace Spotify;

public interface ISpotifyService
{
    Task<IReadOnlyCollection<TrackInfoDto>> GetLikedTracksAsync(int pagesToFetch);
}