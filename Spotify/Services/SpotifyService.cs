using Application.Contracts;
using Misc;
using Spotify.Models;
using Stats.Models;

namespace Spotify.Services;

internal sealed class SpotifyService(SpotifyClient api, Settings settings) : ITracksService
{
    private const int MaxSpotifyLimit = 50;

    public async Task<IReadOnlyCollection<TrackInfoDto>> GetLikedTracksAsync(
        IProgressTracker progressTracker,
        int tracksToFetch)
    {
        var result = new List<TrackInfoDto>();

        string? nextBunch = new UrlBuilder(settings.GetUserSavedTracksUrl)
            .WithQueryParam("limit", MaxSpotifyLimit)
            .WithQueryParam("offset", 0)
            .ToString();

        while (nextBunch is not null && result.Count < tracksToFetch)
        {
            SpotifyUserSavedTracksResponse response = await api.GetAsync<SpotifyUserSavedTracksResponse>(nextBunch);

            if (result.Count == 0)
            {
                progressTracker.SetGoal(Math.Min(response.TotalSavedTracks, tracksToFetch));
            }

            progressTracker.IncreaseProgress(response.SavedTracks.Length);

            nextBunch = response.NextBunch;
            result.AddRange(
                response.SavedTracks.Select(MapTrackInfo));
        }

        return result;
    }

    private static TrackInfoDto MapTrackInfo(SpotifySavedTrack track)
    {
        return new TrackInfoDto(
            track.Track.Id,
            track.Track.Name,
            MapArtistsInfo(track.Track.Artists),
            MapAlbumInfo(track.Track.Album));

        AlbumInfoDto MapAlbumInfo(SpotifySimplifiedAlbum album)
            => new(album.Id, album.Name, MapArtistsInfo(album.Artists));

        ArtistInfoDto[] MapArtistsInfo(IReadOnlyCollection<SpotifySimplifiedArtist> artists)
            => artists.Select(a => new ArtistInfoDto(a.Id, a.Name)).ToArray();
    }
}