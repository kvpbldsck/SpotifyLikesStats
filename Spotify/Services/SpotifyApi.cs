using System.Net.Http.Json;
using Application.Contracts;
using Misc;
using Spotify.Models;

namespace Spotify.Services;

internal sealed partial class SpotifyApi(Settings settings, HttpReceiver httpReceiver)
{
    private const int MaxSpotifyLimit = 50;
    private static HttpClient? _httpClient = null;

    public async Task<IReadOnlyCollection<SpotifyTrack>> GetSavedTracksAsync(
        int tracksToFetch,
        IProgressTracker progressTracker)
    {
        var httpClient = await PrepareHttpClient();

        var result = new List<SpotifyTrack>();

        string? nextBunch = new UriBuilder(settings.GetUserSavedTracksUrl)
            .SetQuery(new()
            {
                {"limit", MaxSpotifyLimit},
                {"offset", 0}
            })
            .Uri
            .ToString();

        while (nextBunch is not null && result.Count < tracksToFetch)
        {
            var response = await httpClient.GetAsync(nextBunch);
            response.EnsureSuccessStatusCode();
            SpotifyUserSavedTracksResponse bunch = (await response.Content.ReadFromJsonAsync<SpotifyUserSavedTracksResponse>())!;

            if (result.Count == 0)
            {
                progressTracker.SetGoal(Math.Min(bunch.TotalSavedTracks, tracksToFetch));
            }

            progressTracker.IncreaseProgress(bunch.SavedTracks.Length);

            nextBunch = bunch?.NextBunch;
            result.AddRange(bunch?.SavedTracks.Select(t => t.Track) ?? Array.Empty<SpotifyTrack>());
        }

        return result;
    }

    private async Task<HttpClient> PrepareHttpClient()
    {
        if (_httpClient is null)
        {
            _httpClient = new();
            _httpClient.DefaultRequestHeaders.Authorization = await GenerateAccessToken(_httpClient);
        }

        return _httpClient;
    }
}