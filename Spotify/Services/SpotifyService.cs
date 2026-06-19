using Spotify.Models;

namespace Spotify.Services;

internal sealed class SpotifyService(SpotifyApi api) : ISpotifyService
{
    public async Task<ICollection<TrackInfoDto>> GetLikedTracksAsync()
    {
        var spotifyResponse = await api.GetSavedTracksAsync();
        return spotifyResponse
            .Select(t => new TrackInfoDto(t.Id, t.Name)
            {
                Album = new (t.Album.Id, t.Album.Name),
                Artists = t.Artists.Select(a => new ArtistInfoDto(a.Id, a.Name)).ToArray()
            })
            .ToArray();
    }
}