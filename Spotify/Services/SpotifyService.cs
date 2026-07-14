using Application.Contracts;
using Application.Models;
using CSharpFunctionalExtensions;
using Misc;
using Spotify.Models;
using Stats.Models;

namespace Spotify.Services;

internal sealed class SpotifyService(SpotifyClient api, Settings settings) : ITracksService
{
    private const int MaxSpotifyLimit = 50;

    public async Task<Result<IReadOnlyCollection<TrackInfoDto>, Error>> GetLikedTracksAsync(
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
            var response = await api.GetAsync<SpotifyUserSavedTracksResponse>(nextBunch)
                .TapIf(result.Count == 0, r => progressTracker.SetGoal(Math.Min(r.TotalSavedTracks, tracksToFetch)))
                .Tap(r => progressTracker.IncreaseProgress(r.SavedTracks.Length))
                .Tap(r => nextBunch = r.NextBunch)
                .Map(r => r.SavedTracks.Select(MapTrackInfo))
                .Tap(result.AddRange);

            if (response.IsFailure)
            {
                return response.Error;
            }
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
            => new(album.Id, album.Name, album.ExternalUrls.SpotifyUrl, MapArtistsInfo(album.Artists));

        ArtistInfoDto[] MapArtistsInfo(IReadOnlyCollection<SpotifySimplifiedArtist> artists)
            => artists.Select(a => new ArtistInfoDto(a.Id, a.Name, a.ExternalUrls.SpotifyUrl)).ToArray();
    }
}