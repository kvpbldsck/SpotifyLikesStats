namespace Stats.Models;

public sealed record TrackInfoDto(
    string Id,
    string Name,
    ArtistInfoDto[] Artists,
    AlbumInfoDto Album);

public sealed record AlbumInfoDto(string Id, string Name, string Url, ArtistInfoDto[] Artists);

public sealed record ArtistInfoDto(string Id, string Name, string Url);
