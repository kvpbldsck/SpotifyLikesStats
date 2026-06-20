namespace Stats.Models;

public sealed record StatsDto(
    IReadOnlyCollection<RatedItem> TopArtists,
    IReadOnlyCollection<RatedItem> TopAlbums);

public sealed record RatedItem(string ItemName, int Rate);