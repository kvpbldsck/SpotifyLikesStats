namespace Stats.Models;

public sealed record StatsDto(
    IReadOnlyCollection<string> TopArtists,
    IReadOnlyCollection<string> TopAlbums);