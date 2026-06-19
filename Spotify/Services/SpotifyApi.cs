using System.Net.Http.Json;
using Misc;
using Spotify.Models;

namespace Spotify.Services;

internal sealed partial class SpotifyApi(Settings settings, IHttpReceiver httpReceiver)
{
    private static HttpClient? _httpClient = null;

    public async Task<IReadOnlyCollection<SpotifyTrack>> GetSavedTracksAsync(int pagesToFetch)
    {
        var httpClient = await PrepareHttpClient();

        var result = new List<SpotifyTrack>();

        string? nextBunch = new UriBuilder(settings.GetUserSavedTracksUrl)
            .SetQuery(new()
            {
                {"limit", 50},
                {"offset", 0}
            })
            .Uri
            .ToString();

        while (nextBunch is not null && result.Count < pagesToFetch)
        {
            var response = await httpClient.GetAsync(nextBunch);
            response.EnsureSuccessStatusCode();
            var bunch = await response.Content.ReadFromJsonAsync<SpotifyUserSavedTracksResponse>();
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