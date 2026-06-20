using Stats.Models;

namespace Spotify.Services;

internal sealed class SpotifyService(SpotifyApi api) : ISpotifyService
{
    public async Task<IReadOnlyCollection<TrackInfoDto>> GetLikedTracksAsync(int pagesToFetch)
    {
        var spotifyResponse = await api.GetSavedTracksAsync(pagesToFetch);
        return spotifyResponse
            .Select(t => new TrackInfoDto(
                t.Id,
                t.Name,
                Artists: t.Artists.Select(a => new ArtistInfoDto(a.Id, a.Name)).ToArray(),
                Album: new (t.Album.Id, t.Album.Name, t.Album.Artists.Select(a => new ArtistInfoDto(a.Id, a.Name)).ToArray())))
            .ToArray();
    }
}