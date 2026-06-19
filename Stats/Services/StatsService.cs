using Stats.Models;

namespace Stats.Services;

internal sealed class StatsService : IStatsService
{
    public StatsDto GetStats(IReadOnlyCollection<TrackInfoDto> tracks, int topArtists = 5, int topAlbums = 5)
    {
        string[] artists = tracks
            .SelectMany(t => t.Artists)
            .GroupBy(a => a.Id)
            .Select(g => new RatedItem(g.First().Name, g.Count()))
            .OrderByDescending(r => r.Rate)
            .Take(topArtists)
            .Select(r => r.ItemName)
            .ToArray();

        string[] albums = tracks
            .Select(t => t.Album)
            .GroupBy(a => a.Id)
            .Select(g => new RatedItem(g.First().Name, g.Count()))
            .OrderByDescending(r => r.Rate)
            .Take(topAlbums)
            .Select(r => r.ItemName)
            .ToArray();

        return new StatsDto(artists, albums);
    }

    private sealed record RatedItem(string ItemName, int Rate);
}