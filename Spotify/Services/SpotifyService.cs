using Application.Contracts;
using Stats.Models;

namespace Spotify.Services;

internal sealed class SpotifyService(SpotifyApi api) : ITracksService
{
    public async Task<IReadOnlyCollection<TrackInfoDto>> GetLikedTracksAsync(
        IProgressTracker progressTracker,
        int tracksToFetch)
    {
        var spotifyResponse = await api.GetSavedTracksAsync(tracksToFetch, progressTracker);
        return spotifyResponse
            .Select(t => new TrackInfoDto(
                t.Id,
                t.Name,
                Artists: t.Artists.Select(a => new ArtistInfoDto(a.Id, a.Name)).ToArray(),
                Album: new (t.Album.Id, t.Album.Name, t.Album.Artists.Select(a => new ArtistInfoDto(a.Id, a.Name)).ToArray())))
            .ToArray();
    }
}