using Stats.Models;

namespace Stats;

public interface IStatsService
{
    const int DefaultTopAmount = 5;

    StatsDto GetStats(IReadOnlyCollection<TrackInfoDto> tracks, int topArtists = DefaultTopAmount, int topAlbums = DefaultTopAmount);
}