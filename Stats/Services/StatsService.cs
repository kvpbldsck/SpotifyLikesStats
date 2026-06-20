using Stats.Models;

namespace Stats.Services;

internal sealed class StatsService : IStatsService
{
    public StatsDto GetStats(IReadOnlyCollection<TrackInfoDto> tracks, int topArtists = 5, int topAlbums = 5)
    {
        var artists = tracks
            .SelectMany(t => t.Artists)
            .GroupBy(a => a.Id)
            .Select(g => new RatedItem(g.First().ToString(), g.Count()))
            .OrderByDescending(r => r.Rate)
            .Take(topArtists)
            .ToArray();

        var albums = tracks
            .Select(t => t.Album)
            .GroupBy(a => a.Id)
            .Select(g => new RatedItem(g.First().ToString(), g.Count()))
            .OrderByDescending(r => r.Rate)
            .Take(topAlbums)
            .ToArray();

        return new StatsDto(artists, albums);
    }
}