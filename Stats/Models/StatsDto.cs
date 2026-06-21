namespace Stats.Models;

public sealed record StatsDto(
    IReadOnlyCollection<RatedItem<ArtistInfoDto>> TopArtists,
    IReadOnlyCollection<RatedItem<AlbumInfoDto>> TopAlbums);

public sealed record RatedItem<T>(T Item, int Rate);