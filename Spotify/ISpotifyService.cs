using Spotify.Models;

namespace Spotify;

public interface ISpotifyService
{
    Task<ICollection<TrackInfoDto>> GetLikedTracksAsync();
}