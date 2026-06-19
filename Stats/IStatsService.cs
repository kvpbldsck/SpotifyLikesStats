using Stats.Models;

namespace Stats;

public interface IStatsService
{
    StatsDto GetStats(IReadOnlyCollection<TrackInfoDto> tracks, int topArtists = 5, int topAlbums = 5);
}